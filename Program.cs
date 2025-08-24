using complaints_back.Data;
using complaints_back.models.Users;
using complaints_back.Services.AuthenticationService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;
using FluentValidation;

using System.Text;
using complaints_back.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using complaints_back.Helpers;
using complaints_back.Services.ComplaintService;
using System.Security.Claims;
using Microsoft.Extensions.FileProviders;
using complaints_back.Services.CategoriesService;
using complaints_back.Services.AdminAuthenticationService;
using complaints_back.Services.AdminUserService;
using complaints_back.Services.AdminServices.AdminComplaintService;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000") // your Next.js frontend origin
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // important for cookies
        });
});
// 🔥 Register Identity properly
builder.Services.AddIdentity<User, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedEmail = true; // Require email confirmation




    }
)
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
// AddUserValidator<UserIdentityValidator<User>>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();


// Register Identity API endpoints
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IComplainService, ComplaintService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService>();
builder.Services.AddScoped<IAdminComplaintService, AdminComplaintService>();
builder.Services.AddScoped<IAdminUsersService, AdminUsersService>();
builder.Services.AddScoped<EmailSenderService>();
// 

var jwtSettings = builder.Configuration.GetSection("AppSettings");
var secretKey = jwtSettings["Token"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;




})


.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]))
    };
    //custom resposne for the authrize 
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse(); // suppress the default response

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Unauthorized",

            });

            return context.Response.WriteAsync(result);
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        }
    };
});



// Build the app
var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/images" // This is the URL path where files will be accessible
});
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
// 🔥 Register Identity API endpoints
// app.MapIdentityApi<IdentityUser>();

app.MapControllers();


async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "SuperAdmin", "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

async Task SeedSuperAdmin(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string superAdminEmail = "superadmin@example.com";
    var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

    if (superAdmin == null)
    {
        superAdmin = new User
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            DisplayName = "Super Admin",
            Role = "SuperAdmin",
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // no need for email confirmation for admin
        };

        var result = await userManager.CreateAsync(superAdmin, "SuperAdmin123!"); // default password
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRoles(services);
    await SeedSuperAdmin(services);
}


app.Run();
