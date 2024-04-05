using ImageService.Core.Models;
using ImageService.Core.Repos;
using ImageService.Core.Services;
using ImageService.Persistence;
using ImageService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddKeyedScoped<IImageRepo, ImageRepoDb>("db");
builder.Services.AddKeyedScoped<IImageRepo, ImageRepoRedis>("redis");
builder.Services.AddKeyedScoped<IImageStorageRepo, DiskImageStorage>("disk");
builder.Services.AddKeyedScoped<IImageStorageRepo, RedisImageStorage>("redis");
builder.Services.AddScoped<IImageService, ImageService.Core.Services.ImageService>();

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn")!));
builder.Services.AddDbContext<IImageContext, ImageContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DbConn")!, ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DbConn")!));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p =>
{
    p.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.Configure<ImageConfig>(options =>
{
    options.ThumbnailJpgQuality = 100;
    options.ThumbnailMaxSize = 512;
    options.BaseDirectory = "/db-images/";
});

builder.Services.Configure<RedisConfig>(options =>
{
    options.TTL = new TimeSpan(0, 1, 0);
});

builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    //limit max request body to 5MB 
    serverOptions.Limits.MaxRequestBodySize = 5242880;
});

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

CreateDb(app);

app.MapControllers();
app.Run();

void CreateDb(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var imageContext = scope.ServiceProvider.GetService<IImageContext>();

    imageContext!.Ctx.Database.EnsureCreated();
}

