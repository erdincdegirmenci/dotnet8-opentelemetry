using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region OpenTelemetry Integration


builder.Services.AddOpenTelemetry()
    // Trace ayarlarý
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WeatherForecastAPI")) // Servis bilgisi: Telemetri verilerinde bu servis adý görünecek
            .AddAspNetCoreInstrumentation() // ASP.NET Core uygulama içi HTTP isteklerini izler
            .AddHttpClientInstrumentation() // HttpClient üzerinden yapýlan HTTP çaðrýlarýný izler
            .AddSqlClientInstrumentation()  // SQL sorgularýný izler
            .AddConsoleExporter()           // Trace verilerini konsola yazdýrýr (geliþtirme ve debug için)

            // Zipkin kullanýcaksa; Zipkin servisine trace gönderi
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            })

            // Jaeger kullanýcaksa; Jaeger'a gönderir
            .AddJaegerExporter(jaegerOptions =>
            {
                jaegerOptions.AgentHost = "localhost";
                jaegerOptions.AgentPort = 6831; // Jaeger agent default port (udp protocol)
            });
    })
    // Metrics ayarlarý
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()  // .NET runtime performans metriklerini toplar (CPU, GC, bellek vb.)
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter(); /// Prometheusa gönderir. 
    });

var app = builder.Build();

app.MapPrometheusScrapingEndpoint(); //prometheus endpoint mapler http://localhost:<port>/metrics

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
