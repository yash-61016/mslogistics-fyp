using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Domain
{
	public class Stop : BaseEntity<Guid>
	{
		[StringLength(40)]
		public string? Name { get; set; }  //Roshies

        public int Sequencenumber { get; set; }

        public Guid CustomerId { get; set; }

        public Guid? RouteId { get; set; }
        public Route? Route { get; set; }
    }
}

