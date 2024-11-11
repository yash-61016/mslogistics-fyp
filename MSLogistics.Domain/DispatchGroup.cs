using System.ComponentModel.DataAnnotations;

namespace MSLogistics.Domain
{
	public class DispatchGroup : BaseEntity<Guid>
	{
        public DateTime _dispatchDate;

        [StringLength(40)]
        public string? Name { get; set; }

        public DateTime DispatchDate
        {
            get => _dispatchDate;
            set => _dispatchDate = value.ToUniversalTime();
        }

        public IEnumerable<Route> Routes { get; set; } = new List<Route>();
    }
}

