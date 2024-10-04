using AutoMapper;
using EAD_Web_Service.Dtos;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class CartService : ICartService
{
    private readonly IMongoCollection<Cart> _cartCollection;
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMapper _mapper;

    public CartService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _cartCollection = mongoDatabase.GetCollection<Cart>(databaseSettings.Value.CartsCollectionName);
        _productCollection = mongoDatabase.GetCollection<Product>(databaseSettings.Value.ProductsCollectionName);
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartByUserIdAsync(string userId)
    {
        var cart = await _cartCollection.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> CreateCartAsync(string userId)
    {
        var cartEntity = new Cart
        {
            UserId = userId,
            Items = [],
            TotalPrice = (decimal?)0.00,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _cartCollection.InsertOneAsync(cartEntity);
        return _mapper.Map<CartDto>(cartEntity);
    }

    public async Task<CartDto?> AddToCartAsync(string userId, string productId)
    {
        var cart = await _cartCollection.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();

        cart ??= new Cart
        {
            UserId = userId,
            Items = [],
            TotalPrice = 0.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var product = await _productCollection.Find(p => p.Id == productId).FirstOrDefaultAsync();
        const int quantity = 1;
        var existingItem = cart.Items?.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.InventoryCount = product.InventoryCount.Value;
            existingItem.Quantity += quantity;
            cart.TotalPrice += product.Price * quantity;
        }
        else
        {
            var newCartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Quantity = quantity,
                InventoryCount = product.InventoryCount.Value,
                Price = product.Price,
                Images = product.Images
            };
            cart.Items?.Add(newCartItem);
            cart.TotalPrice += product.Price * quantity;
        }
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart, new ReplaceOptions { IsUpsert = true });
        return _mapper.Map<CartDto>(cart);
    }


    public async Task<CartDto?> RemoveFromCartAsync(string userId, string productId)
    {
        var cart = await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();

        if (cart == null || cart.Items == null)
        {
            throw new InvalidOperationException("Cart not found or empty.");
        }

        var itemToRemove = cart.Items.FirstOrDefault(item => item.ProductId == productId);

        if (itemToRemove != null)
        {
            var product = await _productCollection.Find(p => p.Id == productId).FirstOrDefaultAsync() ?? throw new InvalidOperationException("Product not found.");
            
            if (itemToRemove.Quantity > 1)
            {
                itemToRemove.Quantity -= 1;
            }
            else
            {
                cart.Items.Remove(itemToRemove);
            }
            cart.TotalPrice = cart.Items.Sum(item => product.Price * item.Quantity);
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartCollection.ReplaceOneAsync(c => c.UserId == userId, cart);
            return _mapper.Map<CartDto>(cart);
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> DeleteCartAsync(string cartId)
    {
        var result = await _cartCollection.DeleteOneAsync(c => c.Id == cartId);
        return result.DeletedCount > 0;
    }

}
