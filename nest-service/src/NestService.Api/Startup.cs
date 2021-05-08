using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NestService.Api.Services;
using NestService.Api.Services.Implementation;
using Newtonsoft.Json.Converters;

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

            services
                .AddControllers()
                .AddNewtonsoftJson(opt =>
                {
                    opt.UseCamelCasing(true);
                });

            services.AddSingleton<INester, Nester>();

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors(_allowAllPolicy);
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}
