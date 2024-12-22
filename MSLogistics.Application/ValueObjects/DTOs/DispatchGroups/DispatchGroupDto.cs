using System.ComponentModel.DataAnnotations;
using MSLogistics.Application.ValueObjects.DTOs.Route;

namespace MSLogistics.Application.ValueObjects.DTOs.DispatchGroups
{
	public class DispatchGroupDto
	{
        public Guid Id { get; set; }

        [StringLength(30)]
        public string? Name { get; set; }

        public DateTime DispatchDate { get; set; }

        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();
    }
}