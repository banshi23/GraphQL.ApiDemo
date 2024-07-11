using GraphQL.ApiDemo.Data;
using GraphQL.ApiDemo.GraphQL.Mutations;
using GraphQL.ApiDemo.GraphQL.Types;
using GraphQL.ApiDemo.Repositories;
using GraphQL.ApiDemo.Utilities;
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

// configure strongly typed settings object
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add services to the container.


//Register Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
//InMemory Database
builder.Services.AddDbContext<DbContextClass>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("GraphQLDBConnection")));
//GraphQL Config
builder.Services.AddGraphQLServer()
	.AddAuthorization()
	.AddQueryType<Query>()
	.AddMutationType<Mutations>();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

//builder.Services
//			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//			.AddJwtBearer(options =>
//			{
//				var tokenSettings = builder.Configuration
//				.GetSection("JwtSettings").Get<JwtSettings>();
//				options.TokenValidationParameters = new TokenValidationParameters
//				{
//					ValidIssuer = tokenSettings.Issuer,
//					ValidateIssuer = true,
//					ValidAudience = tokenSettings.Audience,
//					ValidateAudience = true,
//					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
//					ValidateIssuerSigningKey = true,
//				};
//			});

builder.Services
	.AddAuthorization(options =>
	{
		options.AddPolicy("roles-policy", policy =>
		{
			policy.RequireRole(new string[] { "admin", "super-admin" });
		});
		options.AddPolicy("claim-policy-1", policy =>
		{
			policy.RequireClaim("LastName");
		});
		options.AddPolicy("claim-policy-2", policy =>
		{
			policy.RequireClaim("LastName", new string[] { "Bommidi", "Test" });
		});
	});

var app = builder.Build();
//Seed Data
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<DbContextClass>();
	SeedData.Initialize(services);
}
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
