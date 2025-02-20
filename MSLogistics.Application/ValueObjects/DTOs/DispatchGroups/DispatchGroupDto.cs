using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Application.ValueObjects.DTOs.DispatchGroups
{
	public class DispatchGroupDto
	{
        public Guid Id { get; set; }

        [StringLength(30)]
        public string? Name { get; set; }

        public DateTime DispatchDate { get; set; }

        public List<Guid> RoutesIds { get; set; } = new List<Guid>();
    }
}