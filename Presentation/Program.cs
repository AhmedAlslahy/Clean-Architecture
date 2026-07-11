using Infrastructure.Persistence;
using Presentation;
using Presentation.EndPoints.Auth;
using Presentation.EndPoints.Role;
using Presentation.EndPoints.User;
using Presentation.EndPoints.UserSetting;
using Presentation.GlobalExceptionHandler;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddProjectConfiguration();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddSignalR();
builder.Services.AddCorsPolicy();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapHub<NotificationHub>("/notificationHub");
app.UseStaticFiles();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapAuthEndpoints();
app.MapUserSettingEndpoints();

app.Run();