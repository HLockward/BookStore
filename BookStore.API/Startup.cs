using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore.API.DbContexts;
using BookStore.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace BookStore.API
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
            services.AddHttpCacheHeaders( expirationModelOptions => {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },(validationModelOptions) => {
                validationModelOptions.MustRevalidate = true;
            });

            services.AddResponseCaching();

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.CacheProfiles.Add("240SecondsCacheProfile",
                                                new CacheProfile()
                                                {
                                                    Duration = 240
                                                });

            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                            context.HttpContext,
                            context.ModelState);

                    // add additional info not added by default
                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    var actionExecutingContext =
                          context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if ((context.ModelState.ErrorCount > 0) &&
                        (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsofJsonOutputFormatter = config.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if(newtonsofJsonOutputFormatter != null)
                {
                    newtonsofJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
                }
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            services.AddTransient<IPropertycheckerService, PropertycheckerService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IBookLibraryRepository, BookLibraryRepository>();

            services.AddDbContext<BookLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=localhost,1433;Database=BookStore;Persist Security Info=False;User ID=sa;Password=HLockward2018;Trusted_Connection=False;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            //app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
