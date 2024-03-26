using ChatSender.Core.Externals;
using ChatSender.Core.Services;
using ChatSender.Hubs;
using Microsoft.Extensions.Configuration;
using User.Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));

builder.Services.AddSignalR();

builder.Configuration.AddJsonFile("/apiServers.json");
builder.Services.Configure<ApiServers>(builder.Configuration.GetSection("apiServers"));


//services
builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<IAuthApi, AuthApi>();
builder.Services.AddSingleton<GlobalService>();

//configure http clients
var configuration = builder.Configuration;

builder.Services.AddHttpClient<IUserApi, UserApi>(x =>
{
    x.BaseAddress = new Uri(configuration.GetSection("apiServers").GetValue<string>("UserApi")!);
});

builder.Services.AddHttpClient<IAuthApi, AuthApi>(x =>
{
    x.BaseAddress = new Uri(configuration.GetSection("apiServers").GetValue<string>("AuthApi")!);
});



var app = builder.Build();

app.UseRouting();
app.MapHub<ChatHub>("/Chat");

app.Run();
