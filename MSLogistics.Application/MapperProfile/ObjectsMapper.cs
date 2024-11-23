using AutoMapper;
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
			//Vehicle mapper
            CreateMap<VehicleDto, Vehicle>().ReverseMap();

            //Stops
            CreateMap<StopDto, Stop>().ReverseMap();

            //Stops
            CreateMap<RouteDto, Route>().ReverseMap();
        }
	}
}

