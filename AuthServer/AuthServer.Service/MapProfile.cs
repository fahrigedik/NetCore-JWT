using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AutoMapper;

namespace AuthServer.Service;
public class MapProfile : Profile
{

    public MapProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<UserApp, UserAppDto>().ReverseMap();
    }
}
