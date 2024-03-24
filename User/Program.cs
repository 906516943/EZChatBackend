using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using User.Core.Repos;
using User.Core.Services;
using User.Persistence;
using User.Persistence.Contexts;
using User.Persistence.Contexts.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));

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

var app = builder.Build();

CreateDb(app);


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

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

}
