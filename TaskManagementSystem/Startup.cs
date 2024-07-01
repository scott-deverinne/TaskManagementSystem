using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using TaskManagementSystem.Models;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();

        services.AddDbContext<TaskManagementContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Default")));

        // Configure Identity SignInManager
        services.AddIdentity<AppUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<TaskManagementContext>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddUserManager<UserManager<AppUser>>()
            .AddDefaultTokenProviders();

        // Configure cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        // Configure authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        services.AddAuthorization(options =>
        {
            // Add authorization policies if needed
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            SeedDatabase(userManager, roleManager).Wait();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // Enable authentication
        app.UseAuthorization(); // Enable authorization

        app.UseCookiePolicy(); // Apply cookie policy

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");
        });
    }

    private async Task SeedDatabase(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the roles if they don't exist
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            // Create admin user if it doesn't exist
            var user = new AppUser
            {
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                FirstName = "Scott",
                LastName = "DeVerinne",
                TwoFactorEnabled = false,
                BirthDate= DateOnly.Parse("1984-09-12"),
                Designation="Manager"
            };
            var result = await userManager.CreateAsync(user, "Admin*123");

            if (result.Succeeded)
            {
                // Assign "Admin" role to the admin user
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
