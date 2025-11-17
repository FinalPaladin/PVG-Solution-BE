using AutoMapper;
using PVG.Domain.Models;
using PVG.Infrastucture.Entities;

namespace PVG.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add your mapping configurations here
            // Example: CreateMap<SourceModel, DestinationModel>();
            // You can also use ReverseMap() for bi-directional mapping

            CreateMap<ConfigurationModel, Configuration>();
            CreateMap<Configuration, ConfigurationModel>();
        }
    }
}