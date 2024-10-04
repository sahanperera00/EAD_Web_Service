using AutoMapper;
using EAD_Web_Service.Dtos;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Enums;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class OrderService : IOrderService
{
    private readonly IMongoCollection<Order> _orderCollection;
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMapper _mapper;

    public OrderService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _orderCollection = mongoDatabase.GetCollection<Order>(databaseSettings.Value.OrdersCollectionName);
        _productCollection = mongoDatabase.GetCollection<Product>(databaseSettings.Value.ProductsCollectionName);
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderCollection.Find(order => true).ToListAsync();
        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetOrdersByUserIdAsync(string userId)
    {
        var orders = await _orderCollection.Find(o => o.UserId == userId).ToListAsync();
        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(string orderId)
    {
        var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        return order == null ? null : _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderAsync(string userId, OrderRequestDto orderRequestDto, CartDto cartDto)
    {
        var order = _mapper.Map<Order>(orderRequestDto);
        order.UserId = userId;
        order.Items = _mapper.Map<List<OrderItem>>(cartDto.Items);
        order.TotalPrice = cartDto.TotalPrice.Value;
        order.Note = string.Empty;
        order.CreatedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        if (order.Items != null)
        {
            foreach (var item in order.Items)
            {
                if (string.IsNullOrEmpty(item.Id))
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                }
            }
        }
        await _orderCollection.InsertOneAsync(order);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto?> UpdateOrderAsync(string orderId, OrderDto orderDto)
    {
        var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();

        if (order == null || !order.Status.Equals(OrderStatus.Processing)) { return null; }

        var updateDefinition = Builders<Order>.Update;
        var updates = new List<UpdateDefinition<Order>>();

        if (!string.IsNullOrEmpty(orderDto.PhoneNumber))
        {
            updates.Add(updateDefinition.Set(o => o.PhoneNumber, orderDto.PhoneNumber));
        }
        if (!string.IsNullOrEmpty(orderDto.DeliveryAddress))
        {
            updates.Add(updateDefinition.Set(o => o.DeliveryAddress, orderDto.DeliveryAddress));
        }
        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _orderCollection.UpdateOneAsync(o => o.Id == orderId, combinedUpdate);

            if (result.ModifiedCount > 0)
            {
                var updatedOrder = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
                return _mapper.Map<OrderDto>(updatedOrder);
            }
        }
        return null;
    }

    public async Task<ProductResponseDto?> UpdateProductAsync(string productId, ProductRequestDto productRequestDto)
    {
        var updateDefinition = Builders<Product>.Update;
        var updates = new List<UpdateDefinition<Product>>();

        if (!string.IsNullOrEmpty(productRequestDto.Name))
        {
            updates.Add(updateDefinition.Set(c => c.Name, productRequestDto.Name));
        }
        if (!string.IsNullOrEmpty(productRequestDto.Description))
        {
            updates.Add(updateDefinition.Set(c => c.Description, productRequestDto.Description));
        }
        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _productCollection.UpdateOneAsync(c => c.Id == productId, combinedUpdate);

            if (result.ModifiedCount > 0)
            {
                var updatedCategory = await _productCollection.Find(c => c.Id == productId).FirstOrDefaultAsync();
                return _mapper.Map<ProductResponseDto>(updatedCategory);
            }
        }
        return null;
    }

    public async Task<bool> CancelOrderAsync(string orderId)
    {
        var order = await _orderCollection.Find(o => o.Id == orderId && o.Status == OrderStatus.Processing).FirstOrDefaultAsync();

        if (order == null || !order.IsCancelRequested) return false;

        order.Status = OrderStatus.Canceled;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderCollection.ReplaceOneAsync(o => o.Id == orderId, order);
        return true;
    }

    public async Task<bool> RequestOrderCancellationAsync(string orderId, CancelOrderRequestDto cancelOrderRequestDto)
    {
        var filter = Builders<Order>.Filter.Where(o => o.Id == orderId && o.Status == OrderStatus.Processing);

        var update = Builders<Order>.Update
            .Set(o => o.IsCancelRequested, true)
            .Set(o => o.Note, cancelOrderRequestDto.Note)
            .Set(o => o.UpdatedAt, DateTime.Now);

        var result = await _orderCollection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0) { return false; }
        return true;
    }

    public async Task<List<OrderItemDto>> GetOrderItemsByVendorIdAsync(string vendorId)
    {
        var productIdsForVendor = await _productCollection
            .Find(p => p.VendorId == vendorId)
            .Project(p => p.Id)
            .ToListAsync();

        var filter = Builders<Order>.Filter.ElemMatch(o => o.Items,
            Builders<OrderItem>.Filter.In(oi => oi.ProductId, productIdsForVendor)
        );

        var pendingOrderItems = await _orderCollection
            .Find(filter)
            .Project(order => order.Items
                .Where(item => productIdsForVendor.Contains(item.ProductId))
                .ToList())
            .ToListAsync();

        return _mapper.Map<List<OrderItemDto>>(pendingOrderItems.SelectMany(itemList => itemList).ToList());
    }

    public async Task<bool> UpdateOrderItemStatusAsync(string orderItemId, ItemStatus newStatus)
    {
        var filter = Builders<Order>.Filter.ElemMatch(o => o.Items, Builders<OrderItem>.Filter.Eq(oi => oi.Id, orderItemId));
        var order = await _orderCollection.Find(filter).FirstOrDefaultAsync();

        if (order == null || order.IsCancelRequested) { return false; }

        var update = Builders<Order>.Update
            .Set("items.$.status", newStatus)
            .Set(o => o.UpdatedAt, DateTime.Now);

        var result = await _orderCollection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
            return false;


        if (order != null && order.Items != null)
        {
            bool allDelivered = order.Items.All(item => item.Status == ItemStatus.Delivered);
            bool someDelivered = order.Items.Any(item => item.Status == ItemStatus.Delivered);

            if (allDelivered)
            {
                var updateOrderStatus = Builders<Order>.Update
                    .Set(o => o.Status, OrderStatus.Delivered)
                    .Set(o => o.UpdatedAt, DateTime.Now);
                await _orderCollection.UpdateOneAsync(o => o.Id == order.Id, updateOrderStatus);
            }
            else if (someDelivered)
            {
                var updateOrderStatus = Builders<Order>.Update
                    .Set(o => o.Status, OrderStatus.PartialDelivered)
                    .Set(o => o.UpdatedAt, DateTime.Now);
                await _orderCollection.UpdateOneAsync(o => o.Id == order.Id, updateOrderStatus);
            }
        }
        return true;
    }

    public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
    {
        var order = await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();

        if (newStatus.Equals(OrderStatus.Canceled) && 
            !order.Status.Equals(OrderStatus.Processing) ||
            order.IsCancelRequested) 
        {
            return false;
        }

        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        var update = Builders<Order>.Update
            .Set(o => o.Status, newStatus)
            .Set(o => o.UpdatedAt, DateTime.Now);
        
        var result = await _orderCollection.UpdateOneAsync(filter, update);

        if (newStatus == OrderStatus.Delivered)
        {
            await _orderCollection.UpdateManyAsync(filter, 
                Builders<Order>.Update.Set("items.$[].status", ItemStatus.Delivered));
        }
        else if (newStatus == OrderStatus.Canceled)
        {
            await _orderCollection.UpdateManyAsync(filter, 
                Builders<Order>.Update.Set("items.$[].status", ItemStatus.Cancelled));
        }
        return result.ModifiedCount > 0;
    }
}
