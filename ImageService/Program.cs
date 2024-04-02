using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn")!));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p =>
{
    p.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
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

app.MapControllers();
app.Run();
