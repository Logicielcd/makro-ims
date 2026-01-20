using Makro.IMS.Infra.Data.Models;
using Makro.IMS.POService.Api.Configurations;
using Makro.IMS.POServices.Api.Configurations;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("application.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MakroIMS",
        builder => builder
        //.WithOrigins(
        //    "http://localhost:60054", 
        //    "http://localhost:4200"
        //)
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    //.AllowCredentials()
    );
});

builder.Services.AddJwtConfiguration(builder.Configuration.GetSection(nameof(JwtIssuerOptions)));


// Setting DBContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

builder.Services.AddMemoryCache();

builder.Services.AddSwaggerConfiguration();

builder.Services.RegisterServices();

builder.Services.AddSieveConfiguration(builder.Configuration.GetSection("Sieve"));

// Configure Compression level
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);


// Add Response compression services
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
});

app.UseResponseCompression();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("MakroIMS");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.MapControllers();

app.UseSwaggerSetup();

app.Run();
