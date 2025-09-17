using AutoMapper;
using DemoBackend.Dto;
using DemoBackend.Models;

namespace DemoBackend.Mapping
{
    public class LoginMapping : Profile
    {

        public LoginMapping()
        {
            CreateMap<LoginDto, Login>();
            CreateMap<Login, LoginDto>();
        }
    }
}
