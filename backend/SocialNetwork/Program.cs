using Microsoft.AspNetCore.Identity;
using SocialNetwork.Data;
using SocialNetwork.Model;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Extract the string from IConfiguration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register the DbContext using that string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

// 3. Add Identity (as we discussed before)
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.Run();