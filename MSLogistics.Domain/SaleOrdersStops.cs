namespace MSLogistics.Domain
{
	public class SaleOrdersStops : BaseEntity<Guid>
	{
		public Guid StopId { get; set; }

        public Guid OrderId { get; set; }		
	}
}

