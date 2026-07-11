using Application.Common;
using Application.Features.Authentication.Register;
using FluentValidation;
using Infrastructure.Interceptors;
using Infrastructure.Persistence;
using Infrastructure.Service.Email;
using Infrastructure.Service.JWT;
using Infrastructure.Service.Shared;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Presentation;

public static class DependencyInjection
{
    //configuration
    public static WebApplicationBuilder AddProjectConfiguration(
  this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true)
            .AddEnvironmentVariables();

        return builder;
    }

    //Database
    public static IServiceCollection AddDatabase(
      this IServiceCollection services,
      IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<SarhneDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("ProjectConnection"));

            options.AddInterceptors(
                sp.GetRequiredService<AuditInterceptor>());
        });

        return services;
    }

    // Services
    public static IServiceCollection AddApplicationServices(
     this IServiceCollection services,
     IConfiguration configuration)
    {
        services.Configure<JwtInformations>(
                configuration.GetSection("JWTInformations"));
        services.Configure<EmailInformations>(
            configuration.GetSection("EmailInformations"));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
        });

        return services;
    }

    //cors
    public static IServiceCollection AddCorsPolicy(
      this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("MyPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    //validation
    public static IServiceCollection AddValidationServices(
     this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    //JWT
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWTInformations:issuerIP"],

                    ValidateAudience = true,
                    ValidAudience = configuration["JWTInformations:audienceIP"],

                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["JWTInformations:SecretKey"]!)),

                    ClockSkew = TimeSpan.Zero
                };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["jwt"];
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    //Authorization
    public static IServiceCollection AddAuthorizationServices(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });

            options.AddPolicy("UserOnly", policy =>
            {
                policy.RequireRole("User");
            });
        });

        return services;
    }

    ////IDentity
    //public static IServiceCollection AddIdentityServices(
    //    this IServiceCollection services)
    //{
    //    services.AddIdentity<User, IdentityRole>(options =>
    //    {
    //        options.Password.RequireDigit = true;
    //        options.Password.RequireLowercase = true;
    //        options.Password.RequireUppercase = true;
    //        options.Password.RequireNonAlphanumeric = false;
    //        options.Password.RequiredLength = 8;
    //    })
    //    .AddEntityFrameworkStores<SarhneDbContext>();

    //    return services;
    //}
}