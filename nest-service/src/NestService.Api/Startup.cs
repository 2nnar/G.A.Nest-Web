using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NestService.Api.Services;
using NestService.Api.Services.Implementation;

namespace NestService.Api
{
#pragma warning disable CS1591 // Отсутствует комментарий XML дл¤ открытого видимого типа или члена
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
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services
                .AddControllers()
                .AddNewtonsoftJson(opt =>
                {
                    opt.UseCamelCasing(true);
                });

            services.AddSingleton<INester, Nester>();
            services.AddSingleton<IGCodeGenerator, GCodeGenerator>();

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
