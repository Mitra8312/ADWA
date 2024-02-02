using ADWA.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Запуск фонового процесса
builder.Services.AddHostedService<DisconnectService>();

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

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    _ = app.UseHsts();
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
