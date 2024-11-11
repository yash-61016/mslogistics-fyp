


using Microsoft.EntityFrameworkCore;
using MSLogistics.Application.MapperProfile;
using MSLogistics.Application.Repositories.IDispatchGroupRepository;
using MSLogistics.Application.Repositories.IRouteRepository;
using MSLogistics.Application.Repositories.IStopRepository;
using MSLogistics.Application.Repositories.IVehicleRepository;
using MSLogistics.Application.Services.VehicleService;
using MSLogistics.Persistence;
using MSLogistics.Repository.DispatchGroupRepository;
using MSLogistics.Repository.RouteRepository;
using MSLogistics.Repository.StopRepository;
using MSLogistics.Repository.VehicleRepository;


namespace mslogistiscs_fyp
{
	public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC controllers
            services.AddControllers();

            // Add IHttpContextAccessor to DI container
            services.AddHttpContextAccessor();

            // **Registering Services** // 
            // Transient Services

            //Repos
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IRouteRepository, RouteRepository>();
            services.AddTransient<IStopRepository, StopRepository>();
            services.AddTransient<IDispatchGroupRepository, DispatchGroupRepository>();
            //Services
            services.AddTransient<IVehicleService, VehicleService>();

            //SQL Db Context Configuration
            services.AddDbContext<DomainContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("ConnectionString")));

            // Add API endpoint exploration and Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Mapper
            services.AddAutoMapper(typeof(ObjectsMapper));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Serve static files from wwwroot or configured directory
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Dev settings
            if (env.IsDevelopment())
            {
                // Open Swagger
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DomainContext>();
                context.Database.EnsureCreated();
            }
        }
    }
}