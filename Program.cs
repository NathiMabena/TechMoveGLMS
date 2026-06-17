using TechMoveGLMS.Services;
using TechMoveGLMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Read API base URL from config or environment variable
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? "https://localhost:7222/";

// HttpClient for currency service
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

// HttpClient for API calls
builder.Services.AddHttpClient("GlmsApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Session for JWT token storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();