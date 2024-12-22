
using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Domain
{
	public class Route : BaseEntity<Guid>
	{
		[StringLength(30)]
		public string? Name { get; set; }

        public Guid? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public Guid? DispatchGroupId { get; set; }
        public DispatchGroup? DispatchGroup { get; set; }

        public IEnumerable<Stop> Stops { get; set; } = new List<Stop>();
	}
}