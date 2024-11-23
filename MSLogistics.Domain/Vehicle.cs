using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Domain
{
	public class Vehicle : BaseEntity<Guid>
	{
		[StringLength(20)]
		public string? RegistrationNumber { get; set; }

        public decimal LoadCapacity { get; set; }

        [StringLength(40)]
        public string? VehicleModel { get; set; }

        [StringLength(50)]
        public string? VehicleMake { get; set; }
    }
}

