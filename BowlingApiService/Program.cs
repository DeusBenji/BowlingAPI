using BowlingApiService.BusinessLogicLayer;
using BowlingApiService.Security;
using BowlingData.DatabaseLayer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
// Configure the JWT Authentication Service
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
    .AddJwtBearer("JwtBearer", jwtOptions => {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters()
        {
            // The SigningKey is defined in the TokenController class
            ValidateIssuerSigningKey = true,
            // IssuerSigningKey = new SecurityHelper(configuration).GetSecurityKey(),
            IssuerSigningKey = new SecurityHelper(builder.Configuration).GetSecurityKey(),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "https://localhost:7197",
            ValidAudience = "https://localhost:7197",
            ValidateLifetime = true
        };
    });


// Add services to the container.
builder.Services.AddSingleton<ICustomerData, CustomerdataControl>();
builder.Services.AddSingleton<ICustomerAccess, CustomerDatabaseAccess>();
builder.Services.AddSingleton<ILaneData, LanedataControl>();
builder.Services.AddSingleton<ILaneAccess, LaneDatabaseAccess>();
builder.Services.AddSingleton<IPriceData, PricedataControl>();
builder.Services.AddSingleton<IPriceAccess, PriceDatabaseAccess>();
builder.Services.AddSingleton<IBookingData, BookingdataControl>();
builder.Services.AddSingleton<IBookingAccess, BookingDatabaseAccess>();


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
