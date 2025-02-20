using AutoMapper;
using MSLogistics.Application.ValueObjects.DTOs.DispatchGroups;
using MSLogistics.Application.ValueObjects.DTOs.Route;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;
using MSLogistics.Domain;

namespace MSLogistics.Application.MapperProfile
{
	public class ObjectsMapper : Profile
	{
		public ObjectsMapper()
		{
			//Vehicles
            CreateMap<VehicleDto, Vehicle>().ReverseMap();

            //Stops
            CreateMap<StopDto, Stop>().ReverseMap();

            //Routes
            CreateMap<RouteDto, Route>().ReverseMap();

            //DispatchGroups
            CreateMap<DispatchGroup, DispatchGroupDto>()
                .ForMember(dest => dest.RoutesIds, opt => opt.MapFrom(src => src.Routes.Select(r => r.Id).ToList()))
                .ReverseMap()
                .ForMember(dest => dest.Routes, opt => opt.Ignore());

        }
    }
}

