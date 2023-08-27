using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PharmacyDB.Interfaces;
using PharmacyDB.Models;
using PharmacyInfrastructure.Repositories;
using PharmacyWeb.Services.Class;
using PharmacyWeb.Services.Interface;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
 {
     option.Password.RequireDigit = true;
     option.Password.RequireLowercase = true;
     option.Password.RequiredLength = 5;
 }).AddEntityFrameworkStores<PharmacyDbContext>().AddDefaultTokenProviders();
builder.Services.AddDbContext<PharmacyDbContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
//builder.Services.AddSingleton<IWebHostEnvironment>(provider => provider.GetRequiredService<IWebHostEnvironment>());

builder.Services.AddControllers(
                options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5191")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "http://JoullanarAli.net",
        ValidIssuer = "http://JoullanarAli.net",
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is the key that we will use in encryption")),
        ValidateIssuerSigningKey=true
    };
}) ;
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
            
builder.Services.AddScoped<IUserService,UserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
//app.UseMvc();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
