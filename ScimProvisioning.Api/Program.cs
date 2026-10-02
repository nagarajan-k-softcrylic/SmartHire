using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Api.Data;
using ScimProvisioning.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ScimDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ScimDb")));

builder.Services
    .AddAuthentication(ScimApiKeyAuthenticationOptions.SchemeName)
    .AddScheme<ScimApiKeyAuthenticationOptions, ScimApiKeyAuthenticationHandler>(
        ScimApiKeyAuthenticationOptions.SchemeName,
        options => options.ApiKey = builder.Configuration["Scim:ApiKey"] ?? string.Empty);

builder.Services.AddAuthorization();

builder.Services.Configure<SmartHireSyncOptions>(builder.Configuration.GetSection(SmartHireSyncOptions.SectionName));
builder.Services.AddHttpClient<SmartHireSyncClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
