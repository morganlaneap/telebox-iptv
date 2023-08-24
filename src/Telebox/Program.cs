using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using Telebox.Data;
using Telebox.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.None);

var sqliteDbPath = builder.Configuration["SqliteFilePath"];

if (!File.Exists(sqliteDbPath))
{
    File.Copy(Environment.CurrentDirectory + @"\Telebox.blank.db", Environment.CurrentDirectory + @"\Telebox.db");
    sqliteDbPath = Environment.CurrentDirectory + @"\Telebox.db";
}

var sqliteConnectionString = $"Data Source={sqliteDbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(sqliteConnectionString));

builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(p =>
    {
        p.UseNewtonsoftJsonSerializer();
        p.UseSQLite(sqliteConnectionString);
    });
});
builder.Services.AddQuartzServer(o =>
{
    o.WaitForJobsToComplete = true;
});
builder.Services.AddTransient<RecordJob>();

builder.Services.AddControllersWithViews();
builder.Services.AddCors(o => o.AddDefaultPolicy(policyBuilder =>
{
    policyBuilder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();