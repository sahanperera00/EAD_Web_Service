using AutoMapper;
using EAD_Web_Service.Dtos;
using EAD_Web_Service.Dtos.request;
using EAD_Web_Service.Dtos.response;
using EAD_Web_Service.Models;

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
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Order, CartDto>().ReverseMap();
        CreateMap<OrderItem, CartItemDto>().ReverseMap();
    }
}
