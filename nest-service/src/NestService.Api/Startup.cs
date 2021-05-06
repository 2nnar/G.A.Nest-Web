using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NestService.Api
{
    public class Startup
    {
        const string _allowAllPolicy = "AllowAll";

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(_allowAllPolicy,
                    builder => builder
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod());
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors(_allowAllPolicy);
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
