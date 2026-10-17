using API.Hubs.Services;
using Application.Common.Abstraction;
using Application.Common.Interfaces;
using Application.Common.Middleware;
using Application.Features.Auth;
using FluentValidation;
using Hangfire;
using Infrastructure.BackgroundJobs;
using Infrastructure.BackgroundJobs.Jops;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Data;
using Infrastructure.Services;
using Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

namespace API;

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
        IConfiguration configuration
    )
    {
        services.AddSingleton<AuditInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddDbContext<SarhneDbContext>(
            (sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ProjectConnection"));

                options.AddInterceptors(
                    sp.GetRequiredService<AuditInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>()
                );
            });

        services.AddScoped<IApplicationDbContext>(sp =>
         sp.GetRequiredService<SarhneDbContext>());

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
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IFilePathService, FilePathService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationHub, SignalRNotificationPublisher>();
        services.AddScoped<IMessageHub, SignalRMessagePublisher>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Register.Command).Assembly);
        });

        // Jobs
        services.AddScoped<DeleteOldNotificationsJob>();
        services.AddScoped<ClearExpiredOtpJob>();

        return services;
    }

    //Jops
    public static IServiceCollection AddHangfireServices(
           this IServiceCollection services,
           IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(
                      configuration.GetConnectionString("ProjectConnection"));
        });

        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddQuartzServices(
       this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(SendEmailsJob));

            q.AddJob<SendEmailsJob>(options =>
                options.WithIdentity(jobKey));

            q.AddTrigger(options => options
                .ForJob(jobKey)
                .WithIdentity("SendEmailsJob-trigger")
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(10))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
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
        services.AddValidatorsFromAssembly(typeof(Register.Command).Assembly);

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
            //For Cookies
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

    //redis cache
    public static IServiceCollection AddRedisCache(
     this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        var configuration =
            provider.GetRequiredService<IConfiguration>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetConnectionString("Redis");

            options.InstanceName = "Sarhne:";
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