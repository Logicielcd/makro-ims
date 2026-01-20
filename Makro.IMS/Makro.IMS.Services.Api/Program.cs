using Makro.IMS.Infra.Data.Models;
using Makro.IMS.Services.Api.Configurations;
using Makro.IMS.Services.Api.Hubs;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("application.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

//builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MakroIMS",
        builder => builder
        .WithOrigins(
            "http://localhost:60054",
            "http://localhost:4200",
            "http://localhost:4201",
            "https://bookingqa.siammakro.co.th",
            "https://bookingprd.siammakro.co.th"
        )
        //.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

builder.Services.AddSignalR();

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


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("MakroIMS");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<UserRegisterHub>("/userregister");
app.MapHub<SupplierHub>("/supplier");

//app.UseEndpoints(endpoints =>
//{
//    //_ = endpoints.MapControllers();
//    _ = endpoints.MapHub<UserRegisterHub>("/userregister");
//    _ = endpoints.MapHub<SupplierHub>("/supplier");
//});

//app.MapControllers();

app.UseSwaggerSetup();

app.Run();
