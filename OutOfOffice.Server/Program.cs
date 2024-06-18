using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OutOfOffice.Server.Config;
using OutOfOffice.Server.Config.ConfigureOptions;
using OutOfOffice.Server.Core;
using OutOfOffice.Server.Data;
using OutOfOffice.Server.Repositories;
using OutOfOffice.Server.Repositories.Implementation;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// Configuration
builder.Configuration.AddJsonFile("appsettings.secret.json", optional:true);

builder.Services.AddSingleton<JwtConfiguration>();
builder.Services.AddSingleton<DatabaseConfiguration>();
builder.Services.AddSingleton<RedisConfiguration>();

builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtOptions>();

builder.Services.AddDbContext<DataContext>((provider, optionsBuilder) =>
{
    var configuration = provider.GetRequiredService<DatabaseConfiguration>();
    var connectionString = configuration.GetConnectionString();

    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    optionsBuilder.UseSnakeCaseNamingConvention();
}, ServiceLifetime.Scoped, ServiceLifetime.Singleton);

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = provider.GetRequiredService<RedisConfiguration>();

    var options = new ConfigurationOptions()
    {
        EndPoints = configuration.EndPoints
    };
    return ConnectionMultiplexer.Connect(options);
});

// Services

builder.Services.AddSingleton<ExceptionHandlingService>();
builder.Services.AddScoped<IApprovalRequestService, ApprovalRequestService>();

// Repositories

builder.Services.AddScoped<DbUnitOfWork>();

builder.Services.AddScoped<IAuthRepository, DbAuthRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RedisRefreshTokenRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, DbLeaveRequestRepository>();
builder.Services.AddScoped<IEmployeeRepository, DbEmployeeRepository>();
builder.Services.AddScoped<IApprovalRequestRepository, DbApprovalRequestRepository>();
builder.Services.AddScoped<IProjectRepository, DbProjectRepository>();

// Authentification

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    // Employee
    options.AddPolicy(Policies.EmployeePolicy, policy => policy
        .RequireRole(Policies.EmployeePolicy, Policies.HrManagerPolicy,
            Policies.ProjectManagerPolicy, Policies.AdministratorPolicy));
    // Hr manager
    options.AddPolicy(Policies.HrManagerPolicy,
        policy => policy.RequireRole(Policies.HrManagerPolicy, Policies.AdministratorPolicy));
    // Project manager
    options.AddPolicy(Policies.ProjectManagerPolicy,
        policy => policy.RequireRole(Policies.ProjectManagerPolicy, Policies.AdministratorPolicy));
    // Administrator
    options.AddPolicy(Policies.AdministratorPolicy, policy => policy.RequireRole(Policies.AdministratorPolicy));
    // Hr and project manager
    options.AddPolicy(Policies.HrAndProjectManagerPolicy,
        policy => policy.RequireRole(Policies.HrManagerPolicy, Policies.ProjectManagerPolicy,
            Policies.AdministratorPolicy));
});

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
