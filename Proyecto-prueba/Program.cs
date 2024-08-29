using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Proyecto_prueba.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregue servicios al contenedor.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PruebafContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion"));
});
// Configuracion de politicas de autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acces/Login"; //Ruta de la p�gina de inicio de sesi�n
        options.AccessDeniedPath = "/Acces/AccessDenied"; // Ruta de la p�gina de acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);// Tiempo de expiraci�n de la cookie

    }
    );
// Configurar pol�ticas de autorizaci�n
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrEmployedOnly", policy => policy.RequireRole("Admin,Employed"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("Employed"));
    options.AddPolicy("EmployedOnly", policy => policy.RequireRole("User"));
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

app.Run();
