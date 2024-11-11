using System;
using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Application.ValueObjects.DTOs.Vehicle
{
	public class VehicleDto
	{
        public Guid Id { get; set; }

        [StringLength(20)]
        public string? RegisterationNumber { get; set; }

        public decimal LoadCapacity { get; set; }

        [StringLength(40)]
        public string? VehicleModel { get; set; }

        [StringLength(50)]
        public string? VehicleMake { get; set; }
    }
}

