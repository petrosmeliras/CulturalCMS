using AutoMapper;
using CulturalCMS.Application.DTO;
using CulturalCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {

            CreateMap<User, UserReadOnlyDTO>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.Role!.Name));

            CreateMap<UserSignupDTO, User>();

            CreateMap<ItemMetadata, MetadataDTO>().ReverseMap();

            CreateMap<CulturalItemCreateDTO, CulturalItem>();

            CreateMap<CulturalItemUpdateDTO, CulturalItem>()
                .ForMember(dest => dest.Metadata, opt => opt.Ignore());

            CreateMap<AuditLog, AuditLogReadOnlyDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Unknown"));

            CreateMap<CulturalItem, CulturalItemReadOnlyDTO>();

        }
    }
}
