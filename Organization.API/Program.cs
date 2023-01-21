using Organization.API.Model;
using Organization.API.Services;
using Shared.Model;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("OrganizationDatabase"));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("OrganizationDatabase"));
builder.Services.Configure<APIOptions.UserApi>(builder.Configuration.GetSection("UserApi"));

builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IMongoDBService, MongoDBService>();
builder.Services.AddSingleton<IUserServiceClient, UserServiceClient>();


builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtAuthMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
