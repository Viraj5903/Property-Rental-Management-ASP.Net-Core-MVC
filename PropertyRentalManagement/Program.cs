using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("PropertyRentalManagementDB");
builder.Services.AddDbContext<PropertyRentalManagement.Models.PropertyRentalManagementDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(connection));

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
    {
        option.LoginPath = "/Access/Login";
        option.LogoutPath = "/Home/Index";
        option.AccessDeniedPath = "/Access/Denied"; // Redirects unauthorized users to the Denied page
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Custom 404 handling
app.UseStatusCodePagesWithReExecute("/Home/NotFound");

app.Run();
