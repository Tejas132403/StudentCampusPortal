using StudentCampusPortal.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Register DatabaseHelper as a singleton (ADO.NET — Lab 8)
builder.Services.AddSingleton<DatabaseHelper>();

// Add Session support (Lab 5)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add API controllers support (Lab 10)
builder.Services.AddControllers();

var app = builder.Build();

// Initialize database tables on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseHelper>();
    db.InitializeDatabase();
}

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();   // Must be before MapControllers
app.UseAuthorization();

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// API routes (Lab 10)
app.MapControllers();

app.Run();
