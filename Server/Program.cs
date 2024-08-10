using System.Reflection;
using Microsoft.OpenApi.Models;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TTLock API",
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<Configuration>(builder.Configuration);

builder.Services.AddSingleton<IEspCommunicationManagerService, MqttCommunicationService>();
builder.Services.AddSingleton<ILockManagerService, EspLockManagerService>();
builder.Services.AddSingleton<IBluetoothDeviceManagerService, EspBluetoothDeviceManagerService>();
builder.Services.AddHostedService<ManagerHostedService>();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();