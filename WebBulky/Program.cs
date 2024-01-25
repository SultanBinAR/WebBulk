using Microsoft.EntityFrameworkCore;
using WebBulky.DataAccess.Data;
using WebBulky.DataAccess.Repository;
using WebBulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebBulky.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Adding Services to EFC Dependency Injection Container
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Stripe thing
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//3rd Party OAuth Configurations
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1486690311893862";
    options.AppSecret = "68e34f7adc31209977cb188117fa0a5d";
});
//Adding Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddRazorPages(); //To make Razor Pages functional in project. And also Map service to pipeline down below. 


//PIPELINES?
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//Middlewares | Request? Pipelines
app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//Adding Session to Request Pipeline [Keep after Auth.]
app.UseSession();
//Adding routing to map the R.P. in pipeline!
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
