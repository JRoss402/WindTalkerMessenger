using WindTalkerMessenger.Services;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Hubs;
using WindTalkerMessenger.Models.DataLayer;
using Microsoft.AspNetCore.Identity;
using WindTalkerMessenger.Models.DomainModels;
using Coravel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddScheduler();
builder.Services.AddTransient<ClientHearbeat>();
builder.Services.AddScoped<IContextService, ContextService>();
builder.Services.AddSingleton<OnlineUsersLists>();
builder.Services.AddScoped<IUserNameService, UserNameService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<HeartBeat>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Services.UseScheduler(schedular =>
{
    schedular.Schedule<ClientHearbeat>()
        .EverySeconds(4);
});


var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await ConfigureRoles.CreateAdminRole(scope.ServiceProvider);
}
app.Run();
