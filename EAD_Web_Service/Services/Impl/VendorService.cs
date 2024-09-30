using AutoMapper;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EAD_Web_Service.Services.Impl;

public class VendorService : IVendorService
{
    private readonly IMongoCollection<Vendor> _vendorCollection;
    private readonly IMapper _mapper;
    
    public VendorService(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
        _vendorCollection = mongoDatabase.GetCollection<Vendor>(databaseSettings.Value.VendorsCollectionName);
        _mapper = mapper;
    }

    public async Task<List<VendorResponseDto>> GetAllVendorsAsync()
    {
        var vendors = await _vendorCollection.Find(vendor => true).ToListAsync();
        return _mapper.Map<List<VendorResponseDto>>(vendors);
    }

    public async Task<VendorResponseDto> GetVendorByIdAsync(string id)
    {
        var vendor = await _vendorCollection.Find(vendor => vendor.Id == id).FirstOrDefaultAsync();
        return _mapper.Map<VendorResponseDto>(vendor);
    }

    public async Task<VendorResponseDto> CreateVendorAsync(string userName, string userId)
    {
        var vendorEntity = new Vendor
        {
            Name = userName,
            Owner = userId,
            Products = [],
            Ratings = new VendorRatings { Average = 0, TotalReviews = 0 },
            Comments = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _vendorCollection.InsertOneAsync(vendorEntity);
        return _mapper.Map<VendorResponseDto>(vendorEntity);
    }

    public async Task<VendorResponseDto> GetVendorByUserIdAsync(string userId)
    {
        var vendor = await _vendorCollection.Find(vendor => vendor.Owner == userId).FirstOrDefaultAsync();
        return _mapper.Map<VendorResponseDto>(vendor);
    }

    public async Task<VendorResponseDto?> UpdateVendorAsync(string id, string name)
    {
        var updateDefinition = Builders<Vendor>.Update;
        var updates = new List<UpdateDefinition<Vendor>>();

        if (!string.IsNullOrEmpty(name))
        {
            updates.Add(updateDefinition.Set(c => c.Name, name));
        }
        updates.Add(updateDefinition.Set(c => c.UpdatedAt, DateTime.UtcNow));

        if (updates.Count > 0)
        {
            var combinedUpdate = updateDefinition.Combine(updates);
            var result = await _vendorCollection.UpdateOneAsync(c => c.Id == id, combinedUpdate);

            if (result.ModifiedCount > 0)
            {
                var updatedVendor = await _vendorCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
                return _mapper.Map<VendorResponseDto>(updatedVendor);
            }
        }
        return null;
    }

    public async Task<bool> AddVendorRatingAsync(string vendorId, string customerId, double rating, string comment)
    {
        var vendor = await _vendorCollection.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
        if (vendor == null) return false;

        var hasAlreadyRated = vendor.Comments.Any(c => c.CustomerId == new ObjectId(customerId));
        if (hasAlreadyRated)
        {
            return false; 
        }

        var newComment = new VendorComment
        {
            CustomerId = new ObjectId(customerId),
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        var update = Builders<Vendor>.Update
            .Push(v => v.Comments, newComment)
            .Inc(v => v.Ratings.TotalReviews, 1)
            .Set(v => v.UpdatedAt, DateTime.Now);

        var newAverage = ((vendor.Ratings.Average * vendor.Ratings.TotalReviews) + rating) / (vendor.Ratings.TotalReviews + 1);
        update = update.Set(v => v.Ratings.Average, newAverage);

        var result = await _vendorCollection.UpdateOneAsync(v => v.Id == vendorId, update);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateVendorCommentAsync(string vendorId, string customerId, string updatedComment)
    {
        var filter = Builders<Vendor>.Filter.Where(v => v.Id == vendorId && v.Comments.Any(c => c.CustomerId == new ObjectId(customerId)));

        var update = Builders<Vendor>.Update
            .Set(v => v.Comments[-1].Comment, updatedComment) 
            .Set(v => v.UpdatedAt, DateTime.Now); 

        var result = await _vendorCollection.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }


}