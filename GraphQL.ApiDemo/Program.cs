using GraphQL.ApiDemo.Data;
using GraphQL.ApiDemo.GraphQL.Mutations;
using GraphQL.ApiDemo.GraphQL.Types;
using GraphQL.ApiDemo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUrl"];
var clientId = builder.Configuration["AzureKeyVault:ClientId"];
var clientSecret = builder.Configuration["AzureKeyVault:ClientSecret"];

builder.Configuration.AddAzureKeyVault(keyVaultUri, clientId, clientSecret, new DefaultKeyVaultSecretManager());


//Register Service
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<DbContextClass>(options =>
	options.UseSqlServer(builder.Configuration["EnergyOptimiserCorporatePaaSUAT"])
);
//GraphQL Config
builder.Services.AddGraphQLServer()
	.AddAuthorization()
	.AddQueryType<Query>()
	.AddMutationType<Mutations>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
	o.TokenValidationParameters = new TokenValidationParameters
	{
		ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
		ValidAudience = builder.Configuration["JwtSettings:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorization();


var app = builder.Build();

app.MapGraphQL();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
