using AutoMapper;
using EAD_Web_Service.Dtos;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;
using MongoDB.Bson;

namespace EAD_Web_Service.Util;

public class MappingProfile : Profile
{
    public MappingProfile() 
    {
        CreateMap<User, UserRequestDto>().ReverseMap();
        CreateMap<User, UserResponseDto>().ReverseMap();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Product, ProductRequestDto>().ReverseMap();
        CreateMap<Product, ProductResponseDto>().ReverseMap();
        CreateMap<Vendor, VendorResponseDto>().ReverseMap();
        CreateMap<Cart, CartDto>().ReverseMap();
        CreateMap<CartItem, CartItemDto>().ReverseMap();
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<Order, OrderRequestDto>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Order, CartDto>().ReverseMap();
        CreateMap<OrderItem, CartItemDto>().ReverseMap();
        CreateMap<BsonDocument, ProductResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["_id"].AsObjectId.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["name"].AsString))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src["description"].AsString))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src["category"]))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src["price"].ToDecimal()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src["isActive"].AsBoolean))
            .ForMember(dest => dest.InventoryCount, opt => opt.MapFrom(src => src["inventoryCount"].AsInt32))
            .ForMember(dest => dest.LowStockAlert, opt => opt.MapFrom(src => src["lowStockAlert"].AsInt32))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src["images"].AsBsonArray.Select(i => i.AsString).ToList()))
            .ForMember(dest => dest.Vendor, opt => opt.MapFrom(src => src.Contains("vendor") ? src["vendor"] : null));
        CreateMap<BsonValue, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["_id"].AsObjectId.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["name"].AsString))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src["isActive"].AsBoolean));
        CreateMap<BsonValue, ProductVendorResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["_id"].AsObjectId.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["name"].AsString))
            .ForMember(dest => dest.Ratings, opt => opt.MapFrom(src => src["ratings"]))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src["comments"].AsBsonArray));
        CreateMap<BsonValue, VendorRatings>()
            .ForMember(dest => dest.Average, opt => opt.MapFrom(src => src["average"].ToDouble()))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src["total_reviews"].AsInt32));
        CreateMap<BsonDocument, VendorComment>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src["customerId"].AsObjectId))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src["comment"].AsString))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src["createdAt"].ToUniversalTime()));
    }
}
