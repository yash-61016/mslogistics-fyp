using System;
using AutoMapper;
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
        }
	}
}

