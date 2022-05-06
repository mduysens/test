using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapRazorPages();


Meter MyMeter = new("MyCompany.MyProduct.MyLibrary", "1.0");
Counter<long> MyFruitCounter = MyMeter.CreateCounter<long>("MyFruitCounter");

// http://localhost:9006/add/apple/purple
app.MapGet("/add/{fruit}/{color}", async  (string fruit, string color) =>
{
    MyFruitCounter.Add(1, new("name", fruit), new("color", color));
    return "increased";
});

using var meterProvider = Sdk.CreateMeterProviderBuilder()
       .AddMeter("MyCompany.MyProduct.MyLibrary")
        .AddPrometheusExporter(options => { options.StartHttpListener = true; })
       .Build();

// http://localhost:9006/metrics
app.UseOpenTelemetryPrometheusScrapingEndpoint(meterProvider);

MyFruitCounter.Add(1, new("name", "apple"), new("color", "red"));
MyFruitCounter.Add(2, new("name", "lemon"), new("color", "yellow"));
MyFruitCounter.Add(1, new("name", "lemon"), new("color", "yellow"));
MyFruitCounter.Add(2, new("name", "apple"), new("color", "green"));
MyFruitCounter.Add(5, new("name", "apple"), new("color", "red"));
MyFruitCounter.Add(4, new("name", "lemon"), new("color", "yellow"));

app.Run();

