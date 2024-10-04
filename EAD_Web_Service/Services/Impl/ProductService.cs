using AutoMapper;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class ProductService : IProductService
{
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMongoCollection<Vendor> _vendorCollection;
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMapper _mapper;

    public ProductService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _productCollection = mongoDatabase.GetCollection<Product>(databaseSettings.Value.ProductsCollectionName);
        _vendorCollection = mongoDatabase.GetCollection<Vendor>(databaseSettings.Value.VendorsCollectionName);
        _categoryCollection = mongoDatabase.GetCollection<Category>(databaseSettings.Value.CategoriesCollectionName);
        _mapper = mapper;
    }

    public ProductService()
    {
    }

    public async Task<List<ProductResponseDto>> GetAllProductsAsync()
    {
        var aggregationPipeline = new[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Categories" },
                { "localField", "categoryId" },
                { "foreignField", "_id" },
                { "as", "category" }
            }),
            new BsonDocument("$unwind", "$category"),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Vendors" },
                { "localField", "vendorId" },
                { "foreignField", "_id" },
                { "as", "vendor" }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", "$vendor").Add("preserveNullAndEmptyArrays", true))
        };
        var products = await _productCollection.Aggregate<BsonDocument>(aggregationPipeline).ToListAsync();
        var productResponseDtos = _mapper.Map<List<ProductResponseDto>>(products);
        return productResponseDtos;
    }


    public async Task<ProductResponseDto> GetProductByIdAsync(string productId)
    {
        var aggregationPipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("_id", new ObjectId(productId))),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Categories" },
                { "localField", "categoryId" },
                { "foreignField", "_id" },
                { "as", "category" }
            }),
            new BsonDocument("$unwind", "$category"),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Vendors" },
                { "localField", "vendorId" },
                { "foreignField", "_id" },
                { "as", "vendor" }
            }),
            new BsonDocument("$unwind", "$vendor")
        };
        var product = await _productCollection.Aggregate<BsonDocument>(aggregationPipeline).FirstOrDefaultAsync();
        if (product == null) { return null; }
        var productDto = _mapper.Map<ProductResponseDto>(product);
        return productDto;
    }


    public async Task<ProductResponseDto?> CreateProductAsync(ProductRequestDto productRequestDto, string userId)
    {
        var vendor = await _vendorCollection.Find(vendor => vendor.Owner == userId).FirstOrDefaultAsync();

        if (vendor == null)
        {
            return null;
        }

        var productEntity = _mapper.Map<Product>(productRequestDto);
        productEntity.VendorId = vendor.Id;

        productEntity.Images ??= [];
        productEntity.CreatedAt = DateTime.UtcNow;
        productEntity.UpdatedAt = DateTime.UtcNow;

        await _productCollection.InsertOneAsync(productEntity);
        var filter = Builders<Vendor>.Filter.Eq(v => v.Id, vendor.Id);
        var updateDefinition = Builders<Vendor>.Update.AddToSet(v => v.Products, productEntity.Id);
        var updateResult = await _vendorCollection.UpdateOneAsync(filter, updateDefinition);

        if (updateResult.ModifiedCount == 0)
        {
            await _productCollection.DeleteOneAsync(Builders<Product>.Filter.Eq(p => p.Id, productEntity.Id));
            return null;
        }
        return _mapper.Map<ProductResponseDto>(productEntity);
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
        if (!string.IsNullOrEmpty(productRequestDto.CategoryId))
        {
            updates.Add(updateDefinition.Set(c => c.CategoryId, productRequestDto.CategoryId));
        }
        if (productRequestDto.Price != null)
        {
            updates.Add(updateDefinition.Set(c => c.Price, productRequestDto.Price.Value));
        }
        if (productRequestDto.IsActive != null)
        {
            updates.Add(updateDefinition.Set(c => c.IsActive, productRequestDto.IsActive.Value));
        }
        if (productRequestDto.InventoryCount != null)
        {
            updates.Add(updateDefinition.Set(c => c.InventoryCount, productRequestDto.InventoryCount.Value));
        }
        if (productRequestDto.LowStockAlert != null)
        {
            updates.Add(updateDefinition.Set(c => c.LowStockAlert, productRequestDto.LowStockAlert.Value));
        }

        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _productCollection.UpdateOneAsync(c => c.Id == productId, combinedUpdate);

            if (result.ModifiedCount > 0)
            {
                var aggregationPipeline = new[]
                {
                    new BsonDocument("$match", new BsonDocument("_id", new ObjectId(productId))),
                
                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "Categories" },
                        { "localField", "categoryId" },
                        { "foreignField", "_id" },
                        { "as", "category" }
                    }),
                    new BsonDocument("$unwind", "$category")
                };

                var updatedProductDoc = await _productCollection.Aggregate<BsonDocument>(aggregationPipeline).FirstOrDefaultAsync();

                if (updatedProductDoc != null)
                {
                    var productResponse = _mapper.Map<ProductResponseDto>(updatedProductDoc);
                    return productResponse;
                }
            }
        }
        return null;
    }

    public async Task<bool> DeleteProductAsync(string productId, string userId)
    {
        var filter = Builders<Vendor>.Filter.Eq(v => v.Owner, userId);
        var updateDefinition = Builders<Vendor>.Update.Pull(v => v.Products, productId);
        var result = await _vendorCollection.UpdateOneAsync(filter, updateDefinition);

        if (result.ModifiedCount == 0)
        {
            var update = Builders<Vendor>.Update.AddToSet(v => v.Products, productId);
            await _vendorCollection.UpdateOneAsync(filter, update);
            return false;
        }
        await _productCollection.DeleteOneAsync(product => product.Id == productId);
        return true;
    }

    public async Task<ProductResponseDto?> GetProductByProductIdAndVendorIdAsync(string productId, string vendorId)
    {
        var productFilter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, productId),
            Builders<Product>.Filter.Eq(p => p.VendorId, vendorId)
        );
        var productEntity =  await _productCollection.Find(productFilter).FirstOrDefaultAsync();
        return _mapper.Map<ProductResponseDto>(productEntity);
    }

    public async Task<List<ProductResponseDto>> GetProductsByVendorIdAsync(string vendorId)
    {
        var products = await _productCollection.Find(product => product.VendorId == vendorId).ToListAsync();
        return _mapper.Map<List<ProductResponseDto>>(products);
    }
    public async Task<List<ProductResponseDto>> GetProductsByCategoryIdAsync(string categoryId)
    {
        var products = await _productCollection.Find(product => product.CategoryId == categoryId).ToListAsync();
        return _mapper.Map<List<ProductResponseDto>>(products);
    }
}
