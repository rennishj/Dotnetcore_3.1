using DataAccess.ConnectionFactory;
using DataAccess.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;

                //Add xml formatters
                options.OutputFormatters.Insert(0, new XmlSerializerOutputFormatter());
                options.InputFormatters.Insert(0, new XmlSerializerInputFormatter(options));
            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddTransient<IDatabaseConnectionProvider>(c =>
            {
                return new SqlConnectionProvider(Configuration.GetConnectionString("MovieDatabase"));
            });
            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IPosterRepository, PosterRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(sc =>
            {
                //sc.IncludeXmlComments(string.Format(@"{0}\Movies API.xml", AppDomain.CurrentDomain.BaseDirectory));
                sc.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {

                    Title = "Movies API",
                    Version = "1.0"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies API");
                // serve UI at root
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
