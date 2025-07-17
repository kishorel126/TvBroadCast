using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TvBroadCast.DataAccess.DbContext;
using TvBroadCast.DataAccess.Seed;
using TvBroadCast.Domain.Entities;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();


//Registering the DBContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=.;Database=TvBroadCast;Trusted_Connection=True;TrustServerCertificate=True");
});


//Add IdentityServices
builder.Services.AddIdentity<User, IdentityRole>(options =>
{

    //Customizing the Identity options
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Adding the dependencies
builder.Services.AddTransient<DataSeeder>();



//Configuring the cookie settings ---> Handles authentication cookies and configurations
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    //Catches the exception and generates HTMl error responses ---> only for development
    app.UseDeveloperExceptionPage(); 
}


// Must be called before UseRouting and UseEndpoints to ensure seesion state is available in the request pipeline
app.UseSession();

// For authenticating users based on the configured authentication scheme
app.UseAuthentication();

// For authorizing requests based on the configured roles
app.UseAuthorization();

//For redirecting the HTTP requests to HTTPS
app.UseHttpsRedirection();

//For routing requests to the appropraite endpoints
app.UseRouting();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


//Data is seeded at the startup of the application
using (var scope = app.Services.CreateScope())
{

    var services = scope.ServiceProvider;
    try
    {
        var dataSeeder = services.GetRequiredService<DataSeeder>();
        await dataSeeder.SeedDataAsync();
    }
    catch (Exception ex)
    {
        // Logging the error in the console for debugging purposes
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }

}


app.Run();
