using ChatHistory.Core.Models;
using ChatHistory.Subscribers;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));

builder.Configuration.AddJsonFile("/apiServers.json");
builder.Services.Configure<ApiServers>(builder.Configuration.GetSection("apiServers"));

//add redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn")!));
builder.Services.AddSingleton<ChatMessageSubscriberService>();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

//add chat message subscribe background service
builder.Services.AddHostedService<ChatMessageSubscriberService>();

builder.Services.AddCors(p =>
{
    p.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});


var app = builder.Build();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
