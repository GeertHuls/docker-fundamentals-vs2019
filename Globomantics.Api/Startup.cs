using System.Linq;
using Globomantics.Api.Extenstions;
using Globomantics.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Globomantics.Api
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
            services.AddControllers();
            services.AddHealthChecks();

            services.AddMvcCore()
                .AddCors()
                .AddApiExplorer();

            services.AddApiVersioning(options => options.ReportApiVersions = true)
                .AddVersionedApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthN:Authority");
                    options.Audience = Configuration.GetValue<string>("AuthN:ApiName");
                });

            services.AddSwaggerDocumentation();  // defined locally
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            var virtualPath = "/api";
            app.Map(virtualPath, builder =>
            {
                builder.UseApiExceptionHandler();  // defined locally

                // We have similar but not exactly the same code that applies the forwarded HTTP headers.
                // Note that we're not clearing known networks and proxies here, that's important to help
                // swashbuckle to work properly.
                builder.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                var corsOrigins = Configuration.GetValue<string>("CORSOrigins").Split(",");
                if (corsOrigins.Any())
                {
                    builder.UseCors(builder => builder
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
                }

                builder.UseSwaggerDocumentation(virtualPath, Configuration, provider);
                builder.UseAuthentication();
                builder.UseGlobomanticsStyleRequestLogging();
                builder.UseRouting();
                builder.UseAuthorization();
                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();
                    endpoints.MapHealthChecks("/health");
                });
            });
        }
    }
}
