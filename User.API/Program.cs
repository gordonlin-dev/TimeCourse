
using Shared.Model;
using Shared.Services;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<UserDatabaseSettings>(builder.Configuration.GetSection("UserDatabase"));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("UserDatabase"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
//builder.Services.Configure<APIOptions.OrganizationApi>(builder.Configuration.GetSection("OrganizationApi"));

builder.Services.AddHttpClient();
//builder.Services.AddSingleton<IOrganizationServiceClient, OrganizationServiceClient>();
builder.Services.AddSingleton<IMongoDBService,MongoDBService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddControllers();
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

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/User"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtAuthMiddleware>();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
