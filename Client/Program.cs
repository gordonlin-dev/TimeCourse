using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Client.Model;
using Client.Services;
using Blazored.Modal;
using Shared.Services;
using Shared.Model;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

var initialScopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<UserApi>(builder.Configuration.GetSection("UserApi"));
builder.Services.Configure<AppointmentApi>(builder.Configuration.GetSection("AppointmentApi"));
builder.Services.Configure<OrganizationApi>(builder.Configuration.GetSection("OrganizationApi"));

builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddAntDesign();
builder.Services.AddHttpServices();
builder.Services.AddScoped<DialogService>();

builder.Services.AddBlazoredModal();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
