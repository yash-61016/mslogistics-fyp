using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Application.ValueObjects.DTOs.Stop
{
	public class StopDto
	{
        public Guid Id { get; set; }

        [StringLength(20)]
        public string? Name { get; set; }

        public Guid CustomerId { get; set; }
    }
}