using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using PVG.Application;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Infrastucture;
using PVG.Web.Extensions;
using System.Net.Mime;

namespace PVG.Web
{
    public class Startup
    {
        private const string _policyName = "PVGServicePolicy"; // "localhost"
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(_policyName, builder =>
                {
                    builder.WithOrigins()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            // add config appsettings
            services.AddConfigureAppSetting(_configuration);
            services.AddSingleton(_configuration);

            // add other services here
            services.AddApplicationServices();
            services.AddInfrastructure(_configuration);

            // handle Validation failure error response
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var invalid = context.ModelState.Values.FirstOrDefault(e => e.ValidationState == ModelValidationState.Invalid);

                        var response = new BaseResponse
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Error = new ErrorModel()
                            {
                                ErrorCode = ErrorCodeConst.ERROR_SYS_ERR,
                                ErrorMessage = invalid?.Errors.FirstOrDefault()?.ErrorMessage,
                            },
                        };
                        var result = new BadRequestObjectResult(response);

                        // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                        result.ContentTypes.Add(MediaTypeNames.Application.Json);
                        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                        return result;
                    };
                });

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(f => f.FullName.StartsWith("PVG")).ToList();

            services.AddAutoMapper(assemblies);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PVG Services", Version = "v1" });

                foreach (var assembly in assemblies)
                {
                    var filePath = Path.Combine(AppContext.BaseDirectory, $"{assembly.GetName().Name}.xml");
                    if (File.Exists(filePath))
                        c.IncludeXmlComments(filePath, true);
                }

                // Define the BearerAuth scheme that's in use
                //var securitySchema = new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                //    BearerFormat = "JWT",
                //    Reference = new OpenApiReference
                //    {
                //        Type = ReferenceType.SecurityScheme,
                //        Id = JwtBearerDefaults.AuthenticationScheme
                //    }
                //};
                //c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);

                //var securityRequirement = new OpenApiSecurityRequirement
                //{
                //    { securitySchema, new[] { JwtBearerDefaults.AuthenticationScheme } }
                //};
                //c.AddSecurityRequirement(securityRequirement);
            });

            //add healthcheck
            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHealthChecks("/health");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(_policyName);

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PVG Services v1"); c.RoutePrefix = "swagger"; });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}