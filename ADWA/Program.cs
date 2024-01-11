using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using ADWA.Models;
using ADWA.Db;
using ADWA.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ContextDb>();
builder.Services.AddScoped<UsrSvc>();
//builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ActiveDirectoryService>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Index"; // Set the login path
                options.AccessDeniedPath = "/Auth/403"; // Set the access denied path
            });
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    //options.FallbackPolicy = options.DefaultPolicy;
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
app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "auth",
    pattern: "Auth/{action=Login}/{id?}",
    defaults: new { controller = "Auth" });

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=User}/{action=Index}/{id?}");
    
//    endpoints.MapControllerRoute(
//        name: "login",
//        pattern: "{controller=Auth}/{action=Index}/{id?}",
//        controller: "Auth");
    
//    endpoints.MapControllerRoute(
//        name: "logout",
//        pattern: "Auth/{action=Login}/{id?}",
//        controller: "Auth");
//});
app.Run();
