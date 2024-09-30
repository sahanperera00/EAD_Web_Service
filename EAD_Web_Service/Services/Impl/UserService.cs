using AutoMapper;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMapper _mapper;
    private readonly IVendorService _vendorService;

    public UserService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper, IVendorService vendorService)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _userCollection = mongoDatabase.GetCollection<User>(databaseSettings.Value.UsersCollectionName);
        _mapper = mapper;
        _vendorService = vendorService;
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userCollection.Find(user => !user.Role.Equals("Admin")).ToListAsync();
        return _mapper.Map<List<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(string id)
    {
        var user = await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> CreateUserAsync(UserRequestDto userRequestDto)
    {
        var userEntity = _mapper.Map<User>(userRequestDto);

        if (!string.IsNullOrEmpty(userEntity.Password))
        {
            userEntity.Password = BCrypt.Net.BCrypt.HashPassword(userEntity.Password);
        }

        userEntity.IsPending = true;
        userEntity.IsActive = false;
        userEntity.CreatedAt = DateTime.UtcNow;
        userEntity.UpdatedAt = DateTime.UtcNow;

        await _userCollection.InsertOneAsync(userEntity);

        if (userEntity.Role == "Vendor")
        {
            var vendor = await _vendorService.CreateVendorAsync(userRequestDto.Username, userEntity.Id);
            
            if (vendor == null)
            {
                await _userCollection.DeleteOneAsync(userEntity.Id);
                return null;
            }
        }
        var response = _mapper.Map<UserResponseDto>(userEntity);
        return response;
    }

    public async Task<UserResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto)
    {
        var updateDefinition = Builders<User>.Update;
        var updates = new List<UpdateDefinition<User>>();

        if (!string.IsNullOrEmpty(userRequestDto.Username))
        {
            updates.Add(updateDefinition.Set(c => c.Username, userRequestDto.Username));
        }
        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _userCollection.UpdateOneAsync(c => c.Id == id, combinedUpdate);

            if (result.ModifiedCount > 0)
            {
                var updatedUser = await _userCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
                return _mapper.Map<UserResponseDto>(updatedUser);
            }
        }
        return null;
    }

        public async Task<UserResponseDto> ActivateUserAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsActive, true)
            .Set(u => u.IsPending, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var updatedUser = await _userCollection.FindOneAndUpdateAsync(filter, update);
        return _mapper.Map<UserResponseDto>(updatedUser);
    }

    public async Task<UserResponseDto> DeactivateUserAsync(string id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsActive, false)
            .Set(u => u.IsPending, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var updatedUser = await _userCollection.FindOneAndUpdateAsync(filter, update);
        return _mapper.Map<UserResponseDto>(updatedUser);
    }

    public async Task<User?> GetUserByEmailAsync(string email) =>
        await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
}
