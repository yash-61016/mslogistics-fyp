using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Domain
{
	public class Stop : BaseEntity<Guid>
	{
		[StringLength(40)]
		public string? Name { get; set; }  //Roshies

		public Guid CustomerId { get; set; }
	}
}

