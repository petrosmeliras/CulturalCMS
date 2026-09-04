
using CulturalCMS.API.Helpers;
using CulturalCMS.Application.BusinessServices;
using CulturalCMS.Application.Configuration;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Infrastructure.Data;
using CulturalCMS.Infrastructure.FileStorage;
using CulturalCMS.Infrastructure.Interceptors;
using CulturalCMS.Infrastructure.Repositories;
using CulturalCMS.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace CulturalCMS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((hostingContext, configuration) =>
            {
                configuration.ReadFrom.Configuration(hostingContext.Configuration);
            });

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<CulturalDbContext>(
                (serviceProvider, options) =>
                {
                    options.UseNpgsql(connString);

                    options.AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
                });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICulturalItemService, CulturalItemService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();
            builder.Services.AddScoped<AuditInterceptor>();

            builder.Services.AddRepositories();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                policy.WithOrigins(builder.Configuration["Cors:Origin"]!)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CulturalCMS API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
                options.OperationFilter<AuthorizeOperationFilter>();
            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("StrictPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddMemoryCache();


            var app = builder.Build();

            app.UseExceptionHandler();

           
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowClient");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseRateLimiter();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CulturalDbContext>();

                await context.Database.MigrateAsync();

                await DbSeeder.SeedAsync(context);
            }

            app.Run();
        }
    }
}
