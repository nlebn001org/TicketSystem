using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MinimalWebAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

RegisterServices(builder.Services); // services

var app = builder.Build();

Configure(app); //middleware

List<Person> users = new() { new Person("first@gmail.com", "p123", "admin"), new Person("second@gmail.com", "p234", "user") };

app.Map("/", () => "Welcome page!");
app.Map("/hello", [Authorize(Roles = "admin")] (HttpContext context) =>
{
    var token = context.Request.Headers["Authorization"];
    return $"Hello {context.User?.Identity?.Name}, your auth type is {context.User?.Identity?.AuthenticationType}";
});

app.MapPost("/login", (Person person) =>
{
    Person? user = users.FirstOrDefault(u => u.Email == person.Email && u.Password == person.Password);

    if (user == null) return Results.Unauthorized();

    List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email), new Claim(ClaimTypes.Role, user.Role) };

    JwtSecurityToken jwt = new JwtSecurityToken(
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            SecurityAlgorithms.HmacSha256)
        ); ;

    string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    var response = new
    {

        username = person.Email,
        access_token = encodedJwt,
    };

    return Results.Json(response);
});

app.MapGet("/logout", (HttpContext context) =>
{
    string token = context.Request.Headers["Authorization"];

});

app.Run();


void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb"));
    });

    //Start repository services


    //End repository services

    services.AddAuthorization();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateIssuerSigningKey = true,
            };
        });
}

void Configure(WebApplication app)
{
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}

record Person(string Email, string Password, string Role);