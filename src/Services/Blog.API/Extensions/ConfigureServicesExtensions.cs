using Blog.API.Application.Behaviors;
using Blog.API.Controllers;
using Blog.API.Data;
using Blog.API.Infrastructure.Filters;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Extensions
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            // Added for functional tests
            .AddApplicationPart(typeof(PostsController).Assembly)
            .AddNewtonsoftJson()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            ;

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
            //hcBuilder.AddCheck<BlogDbHealthCheck>(name: "blogDB-check", failureStatus: HealthStatus.Degraded, tags: new string[] { "blogdb" });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            Action<DbContextOptionsBuilder> dbContextOptionsBuilder = (options) =>
            {
                var connectionStrings = configuration.GetSection(nameof(BlogSettings.ConnectionStrings)).Get<ConnectionStringsOptions>();
                if (configuration["DbType"].Equals("InMemoryDatabase", StringComparison.OrdinalIgnoreCase))
                {
                    options.UseInMemoryDatabase(connectionStrings.InMemoryConnection);
                }
                else
                {
                    options.UseSqlServer(connectionStrings.DefaultConnection, b => b.MigrationsAssembly("Blog.API"));
                }
            };

            services.AddDbContext<IDbContext, BlogContext>(dbContextOptionsBuilder,
               ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MediatRSample - blog HTTP API",
                    Version = "v1",
                    Description = "The blog Service HTTP API"
                });
            });

            /**
            services.AddSwaggerGen(options =>
            {
                var assembly = typeof(Startup).Assembly;
                var assemblyProduct = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
                var assemblyDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

                options.DescribeAllParametersInCamelCase();

                options.OperationFilter<Blog.API.Infrastructure.Auth.AuthorizationHeaderParameterOperationFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var apiVersionDescription in provider.ApiVersionDescriptions)
                {
                    if (apiVersionDescription.IsDeprecated)
                        continue;

                    var info = new OpenApiInfo()
                    {
                        Title = assemblyProduct,
                        Description = apiVersionDescription.IsDeprecated
                        ? $"{assemblyDescription} This API version has been deprecated."
                        : assemblyDescription,
                        Version = apiVersionDescription.ApiVersion.ToString(),
                    };
                    options.SwaggerDoc(apiVersionDescription.GroupName, info);
                }

                //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1295
                //https://stackoverflow.com/questions/58197244/swaggerui-with-netcore-3-0-bearer-token-authorization
                //options.OperationFilter<TokenOperationFilter>();
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                options.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securitySchema, new[] { "Bearer" });
                options.AddSecurityRequirement(securityRequirement);

            });
            */

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services
                .Configure<BlogSettings>(configuration)
                .Configure<ConnectionStringsOptions>(configuration.GetSection(nameof(BlogSettings.ConnectionStrings)))
                .Configure<JwtOptions>(configuration.GetSection(nameof(BlogSettings.Jwt)));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            var jwtSettings = configuration.GetSection(nameof(BlogSettings.Jwt)).Get<JwtOptions>();
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.ValidIssuer,
                        ValidAudience = jwtSettings.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey))
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomMediatR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(Startup));
            services.AddMediatR(typeof(IMediator).GetTypeInfo().Assembly);

            //TODO:..
            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            return services;
        }

        public static IServiceCollection AddCustomAutoMapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(Startup));
            ///services.AddSingleton<Blog.API.Infrastructure.Mappers.MapperProfile>();

            return services;
        }
    }

    public class BlogDbHealthCheck : IHealthCheck
    {
        public readonly IDbContext _dbContext;
        public BlogDbHealthCheck(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return (await _dbContext.GetDatabase.ExecuteSqlRawAsync("SELECT 1;")) > 0 
                ? HealthCheckResult.Healthy("A healthy result.") 
                : HealthCheckResult.Unhealthy("An unhealthy result.");
        }
    }
}
