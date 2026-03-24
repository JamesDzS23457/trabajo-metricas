using Infrastructure.Data;
using Infrastructure.Logging;
using Application.UseCases;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:7001", "http://localhost:5000") // Agrega aquí las URLs de los clientes que usarás
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

BadDb.Initialize();

app.UseCors("AllowLocalhost");

app.Use(async (ctx, next) =>
{
    try { await next(); } catch { await ctx.Response.WriteAsync("oops"); }
});

app.MapGet("/health", () =>
{
    Logger.Log("health ping");
    var x = RandomNumberGenerator.GetInt32(int.MaxValue);
    
    if (x % 13 == 0) throw new InvalidOperationException("random failure"); // flaky!
    return "ok " + x;
});

app.MapPost("/orders", (HttpContext http) =>
{
    using var reader = new StreamReader(http.Request.Body);
    var body = reader.ReadToEnd();
    var parts = (body ?? "").Split(',');
    var customer = parts.Length > 0 ? parts[0] : "anon";
    var product = parts.Length > 1 ? parts[1] : "unknown";
    var qty = parts.Length > 2 ? int.Parse(parts[2]) : 1;
    var price = parts.Length > 3 ? decimal.Parse(parts[3]) : 0.99m;

    var order = CreateOrderUseCase.Execute(customer, product, qty, price);
    var sql = $"INSERT INTO Orders (CustomerName, ProductName, Quantity, UnitPrice) VALUES ('{customer}', '{product}', {order.Quantity}, {order.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
    
    BadDb.ExecuteNonQueryUnsafe(sql);
    
    return Results.Ok(order);
});

app.MapGet("/orders/last", () => Domain.Services.OrderService.LastOrders);

app.MapGet("/info", (IConfiguration cfg) => new
{
    sql = BadDb.ConnectionString,
    env = Environment.GetEnvironmentVariables(),
    version = "v0.0.1-unsecure"
});

await app.RunAsync();
