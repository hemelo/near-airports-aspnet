using NearAirports;
using NearAirports.Repository;
using NearAirports.Settings;

var builder = WebApplication.CreateBuilder(args);

// Mongo DB Connection
builder.Services.Configure<AirportsDatabaseSettings>(
    builder.Configuration.GetSection("AirportsDatabase"));

builder.Services.Configure<MapsApiSettings>(
    builder.Configuration.GetSection("MapsApi"));

// Mongo DB Services DAO
builder.Services.AddSingleton<AirportRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();
}

app.UseAuthorization();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
