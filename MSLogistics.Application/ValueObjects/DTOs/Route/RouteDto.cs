using System.ComponentModel.DataAnnotations;
using MSLogistics.Application.ValueObjects.DTOs.Stop;
using MSLogistics.Application.ValueObjects.DTOs.Vehicle;

namespace MSLogistics.Application.ValueObjects.DTOs.Route
{
	public class RouteDto
	{
        public Guid Id { get; set; }
        [StringLength(30)]
        public string? Name { get; set; }

        public VehicleDto? Vehicle { get; set; }

        public List<StopDto> Stops { get; set; } = new List<StopDto>();
    }
}

