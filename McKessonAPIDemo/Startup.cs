using McKessonAPIDemo.Dependencies.Infrastructure;
using McKessonAPIDemo.Dependencies.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors;

namespace DIinCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Registering a singleton service

            // Other dependency injections can be added here using AddTransient or AddScoped
           
            services.AddMvc();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin() // Allow any origin
                           .AllowAnyHeader()  // Allow any header
                           .AllowAnyMethod(); // Allow any method
                });
            });

            services.AddControllers();
            services.AddSingleton<ILocationService>(provider =>
            new LocationService(Path.Combine(Directory.GetCurrentDirectory(), "Data", "locationData2.csv")));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(); // Enable CORS
            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseMvc();
        }
    }
}