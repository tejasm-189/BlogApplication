using BlogApplication.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Driver;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDB");

if (string.IsNullOrWhiteSpace(mongoDbConnectionString))
{
    throw new ArgumentNullException(nameof(mongoDbConnectionString), "MongoDB connection string is missing from configuration.");
}

builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(mongoDbConnectionString));
builder.Services.AddScoped(s => new MongoDBContext(s.GetRequiredService<IMongoClient>(), "BlogAppDB"));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MongoDBService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<NotificationUpdateService>();

builder.Services.AddAuthorizationCore();
Console.WriteLine($"MongoDB Connection String: {mongoDbConnectionString ?? "NULL"}");


var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7188/";
builder.Services.AddHttpClient("BlogApplication", client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
