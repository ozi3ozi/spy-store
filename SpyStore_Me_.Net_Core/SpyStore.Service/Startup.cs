using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SpyStore.Dal.EfStructures;
using SpyStore.Dal.Initialization;
using SpyStore.Dal.Repos;
using SpyStore.Dal.Repos.Interfaces;
using SpyStore.Service.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace SpyStore.Service
{
    //The Startup class configures how the application will handle HTTP requests and response, initiates the 
    //configuration system, and sets up the dependency injection container.
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        public IConfiguration Configuration { get; }

        //The constructor takes an instance of the IConfiguration interface that was created by the 
        //WebHost.CreateDefaultBuilder method in the Program.cs file and assigns it to the Configuration property for 
        //use elsewhere in the class. The constructor can also take an instance of the IHostingEnvironment and/or the 
        //ILoggerFactory, although these are not added in the default template.
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        //The ConfigureServices method is used to configure any services needed by the application and insert them 
        //into the dependency injection container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //config.Filters.Add(new SpyStoreExceptionFilter(_env)) sets the SpyStoreExceptionFilter for all 
            //actions in the application.
            services.AddMvcCore(config => config.Filters.Add(new SpyStoreExceptionFilter(_env)))
                .AddJsonFormatters(j => 
                {
                    j.ContractResolver = new DefaultContractResolver();
                    j.Formatting = Formatting.Indented;
                });
            // http://docs.asp.net/en/latest/security/cors.html
            //This configuration will allow any request from any origin. This works for demo purposes, but production 
            //applications need to be more specific.
            services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin()
                                .AllowCredentials();
                    });
                });
            //The AddDbContextPool call adds the StoreContext to the DI Framework, passing an instance of 
            //DbContextOptions into the constructor along with the connection string retrieved through the configuration 
            //framework. 
            services.AddDbContext<StoreContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("SpyStore")));
            //The next block of code will add all of the data access library(DAL) repositories into the DI container.
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<ICustomerRepo, CustomerRepo>();
            services.AddScoped<IShoppingCartRepo, ShoppingCartRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IOrderDetailRepo, OrderDetailRepo>();
            //This block of code adds SwaggerGen into the DI container and configures the options for this application.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                  new Microsoft.OpenApi.Models.OpenApiInfo
                  {
                      Title = "SpyStore Service",
                      Version = "v1",
                      Description = "Service to support the SpyStore sample eCommerce site",
                      TermsOfService = null,
                      License = new Microsoft.OpenApi.Models.OpenApiLicense
                      {
                          Name = "Freeware",
            //Url = "https://en.wikipedia.org/wiki/Freeware"
            Url = new System.Uri("http://localhost:23741/LICENSE.txt")
                      }
                  });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //This method is used to set up the application to respond to HTTP requests.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //Database Initialization Code
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<StoreContext>();
                    SampleDataInitializer.InitializeData(context);
                }
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpyStore Service v1");
                });
            app.UseStaticFiles();
            //Needed by JavaScript frameworks used later in the book. For more information on Cross-Origin Requests CORS 
            //support: https://docs.microsoft.com/en-us/aspnet/core/security/cors.
            app.UseCors("AllowAll"); //has to go before UseMVC
            app.UseMvc();
        }
    }
}
