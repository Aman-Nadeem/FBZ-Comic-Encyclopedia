using ComicApp.Core.Interfaces;
using ComicApp.Core.Repositories;
using ComicApp.Core.Services;
using ComicApp.Core.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// REQUIRED: session backing store
builder.Services.AddDistributedMemoryCache();

// REQUIRED: session support (login persistence)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Entity Framework Database Context - SQLite for cloud deployment
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Core data repository
builder.Services.AddScoped<IComicRepository, ComicRepository>();

// Analytics service (persist counters across requests)
builder.Services.AddSingleton<IComicSearchService, ComicSearchService>();

// Saved searches module
builder.Services.AddScoped<ISavedSearchService, SavedSearchService>();

// User accounts service
builder.Services.AddScoped<IUserService, UserService>();

// HTTP client for dataset update service
builder.Services.AddHttpClient();

// Memory cache for comic dataset
builder.Services.AddMemoryCache();

// Background service for live dataset updates
builder.Services.AddHostedService<ComicApp.Web.Services.ComicUpdateService>();

// ComicVine API service
builder.Services.AddHttpClient<ComicVineService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "FBZComicApp/1.0");
});
builder.Services.AddScoped<ComicVineService>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(ComicVineService));
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["ComicVine:ApiKey"]!;
    return new ComicVineService(httpClient, apiKey);
});

// Open Library service
builder.Services.AddHttpClient<OpenLibraryService>();
builder.Services.AddScoped<OpenLibraryService>();

// SendGrid email service
builder.Services.AddScoped<EmailService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new EmailService(
        config["SendGrid:ApiKey"]!,
        config["SendGrid:FromEmail"]!,
        config["SendGrid:FromName"]!
    );
});

var app = builder.Build();

// Auto-create SQLite database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session MUST come before Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
