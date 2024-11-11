using MSLogistics.Application.ValueObjects.DTOs.Vehicle;

namespace MSLogistics.Application.Services.VehicleService
{
	public interface IVehicleService
	{
		/// <summary>
		/// Get all vehicle from the database
		/// </summary>
		/// <returns></returns>
		public Task<IEnumerable<VehicleDto>> GetVehicles();

        /// <summary>
        /// Retrieves a vehicle by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the vehicle.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the vehicle with the specified Id.</returns>
        public Task<VehicleDto> GetVehicleById(Guid Id);

        /// <summary>
        /// Adds a collection of vehicles to the database.
        /// </summary>
        /// <param name="vehicleList">The collection of vehicles to add.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result is true if the vehicle(s) were successfully added; otherwise, false.</returns>
        public Task<bool> AddVehicles(IEnumerable<VehicleDto> vehicleList);

        /// <summary>
        /// Deletes a list of vehicles specified by their unique identifiers.
        /// </summary>
        /// <param name="Ids">A list of unique identifiers for the vehicles to be deleted.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result is true if the vehicle(s) were successfully deleted; otherwise, false.</returns>
        public Task<bool> DeleteVehicles(List<Guid> Ids);

        /// <summary>
        /// Updates a collection of vehicles in the database.
        /// </summary>
        /// <param name="vehicleList">The collection of vehicles to update.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result is true if the vehicle(s) were successfully updated; otherwise, false.</returns>
        public Task<bool> UpdateVehicles(IEnumerable<VehicleDto> vehicleList);

    }
}

