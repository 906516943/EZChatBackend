using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using User.Core.Models;
using User.Core.Repos;
using User.Core.Services;
using User.Persistence;
using User.Persistence.Contexts;
using User.Persistence.Contexts.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn")!));
builder.Services.AddDbContext<IUserContext, UserContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DbConn")!, ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DbConn")!));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p =>
{
    p.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));

builder.Configuration.AddJsonFile("/apiServers.json");
builder.Services.Configure<ApiServers>(builder.Configuration.GetSection("apiServers"));

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

CreateDb(app);

// Configure the HTTP request pipeline.
app.MapControllers();
app.Run();


void CreateDb(WebApplication app) 
{
    using var scope = app.Services.CreateScope();
    var userContext = scope.ServiceProvider.GetService<IUserContext>();

    userContext!.Ctx.Database.EnsureCreated();


    //create world channel
    var worldChannel = userContext.Groups.Where(x => x.Name == "World Channel").FirstOrDefault();
    if (worldChannel is null) 
    {
        worldChannel = new EzGroup()
        {
            Name = "World Channel"
        };

        userContext.Groups.Add(worldChannel);

        userContext.Ctx.SaveChanges();
    }


    //create another channel for testing purpose
    var worldChannelChinese = userContext.Groups.Where(x => x.Name == "世界频道").FirstOrDefault();
    if (worldChannelChinese is null)
    {
        worldChannelChinese = new EzGroup()
        {
            Name = "世界频道"
        };

        userContext.Groups.Add(worldChannelChinese);

        userContext.Ctx.SaveChanges();
    }
}
