using BlazorApp.Components;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ExportService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = factory.CreateDbContext();
    // 👆 Program.cs mein directly db inject nahi hoti
    // isliye manually scope banate hain aur db lete hain

    if (!db.Users.Any())
    // 👆 Any() = koi bhi user hai database mein?
    // ! = nahi — matlab agar koi user NAHI hai to andar jao
    {
        db.Users.Add(new BlazorApp.Models.User
        {
            Username = "admin",
            Password = "admin123",
            Role = "Admin"
        });
        // 👆 pehla admin user banao

        db.SaveChanges();
        // 👆 database mein save karo
        // SaveChanges() = commit — jaise SQL ka INSERT
    }
}
app.Run();
