using FreelanceFusion.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CategoryService>(_ => new CategoryService("Server=(localdb)\\MylocalDB;Database=FreelanceFusion;Trusted_Connection=True;MultipleActiveResultSets=True")); // Register CategoryService here

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourCookieNameHere";
        options.LoginPath = "/Login/Index"; // Replace with your login path
        options.LogoutPath = "/LoOut/Index"; // Replace with your logout path
        options.AccessDeniedPath = "/Login/Index"; // Replace with your access denied path
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "logout",
    pattern: "Logout",
    defaults: new { controller = "Logout", action = "Index" });

    app.MapControllerRoute(
    name: "jobDetails",
    pattern: "JobDetails/{JobID}",
    defaults: new { controller = "JobDetails", action = "Index" });

app.Run();
