using Makro.IMS.Infra.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection.Metadata;

namespace Makro.IMS.POServices.Api.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            //if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Makro IMS Project",
                    Description = "Makro Inbound Management System API Swagger surface",
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Input the JWT like: Bearer {your token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                s.OperationFilter<AddCustomHeaderParameter>();


            });

            //return services;
        }

        public static void UseSwaggerSetup(this IApplicationBuilder app)
        {
            //if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
                c.RoutePrefix = string.Empty;
            });

            //return app;
        }
    }

    public class AddCustomHeaderParameter : IOperationFilter
    {
        //public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        //{
        //    if (operation.parameters == null)
        //    {
        //        operation.parameters = new List<Parameter>();
        //    }

        //    // Add a custom header parameter field
        //    operation.parameters.Add(new Parameter
        //    {
        //        name = "Your-Header-Name",
        //        @in = "header",
        //        description = "Description of Your-Header-Name",
        //        required = false, // Set to true if this header is mandatory
        //        type = "string"
        //    });
        //}

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "secret_key",
                In = ParameterLocation.Header,
                Description = "Secret key",
                Required = true
            });

            //throw new NotImplementedException();
        }
    }

}