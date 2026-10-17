using API;
using API.EndPoints.Admin;
using API.EndPoints.Auth;
using API.EndPoints.Users;
using API.GlobalExceptionHandler;
using API.Hubs;
using Application.Common.Interfaces;
using Hangfire;
using Infrastructure.BackgroundJobs;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.AddProjectConfiguration();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddRedisCache();
builder.Services.AddCorsPolicy();
builder.Services.AddQuartzServices();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
       type.FullName?
           .Replace("+", ".")
           .Replace("`", "_")
       ?? type.Name);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAuthorizationServices();
//------------------------------------------------------------------------------------------
var app = builder.Build();

//Auto update database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AutomatedMigration.MigrateAsync(services);

    var passwordService = scope.ServiceProvider
      .GetRequiredService<IPasswordService>();
    await SeedData.SeedAsync(scope.ServiceProvider, passwordService);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseStaticFiles();

app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapAdminEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<MessageHub>("/messageHub");

app.UseHangfireDashboard("/dashboard");
HangfireScheduler.RegisterJobs();

app.Run();