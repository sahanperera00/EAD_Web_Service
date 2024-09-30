using AutoMapper;
using EAD_Web_Service.Dtos;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class CategoryService : ICategoryService
{
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMapper _mapper;

    public CategoryService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _categoryCollection = mongoDatabase.GetCollection<Category>(databaseSettings.Value.CategoriesCollectionName);
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryCollection.Find(category => true).ToListAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(string id)
    {
        var category = await _categoryCollection.Find(category => category.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var categoryEntity = _mapper.Map<Category>(categoryDto);

        categoryEntity.IsActive = false;
        categoryEntity.CreatedAt = DateTime.UtcNow;
        categoryEntity.UpdatedAt = DateTime.UtcNow;

        await _categoryCollection.InsertOneAsync(categoryEntity);
        return _mapper.Map<CategoryDto>(categoryEntity);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(string id, CategoryDto categoryDto)
    {
        var updateDefinition = Builders<Category>.Update;
        var updates = new List<UpdateDefinition<Category>>();

        if (!string.IsNullOrEmpty(categoryDto.Name))
        {
            updates.Add(updateDefinition.Set(c => c.Name, categoryDto.Name));
        }
        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _categoryCollection.UpdateOneAsync(c => c.Id == id, combinedUpdate);
            
            if (result.ModifiedCount > 0)
            {
                var updatedCategory = await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
                return _mapper.Map<CategoryDto>(updatedCategory);
            }
        }
        return null;
    }

    public async Task DeleteCategoryAsync(string id) =>
        await _categoryCollection.DeleteOneAsync(category => category.Id == id);

    public async Task<CategoryDto> ActivateCategoryAsync(string id)
    {
        var filter = Builders<Category>.Filter.Eq(u => u.Id, id);
        var update = Builders<Category>.Update
            .Set(u => u.IsActive, true)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var updatedCategory = await _categoryCollection.FindOneAndUpdateAsync(filter, update);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }

    public async Task<CategoryDto> DeactivateCategoryAsync(string id)
    {
        var filter = Builders<Category>.Filter.Eq(u => u.Id, id);
        var update = Builders<Category>.Update
            .Set(u => u.IsActive, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var updatedCategory = await _categoryCollection.FindOneAndUpdateAsync(filter, update);
        return _mapper.Map<CategoryDto>(updatedCategory);
    }
}
