using LearnIOAPI;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.OpenApi.Models;
using Google.Api;

public class Program
{

    private readonly string _connectionString;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder);
        
        Console.OutputEncoding = Encoding.UTF8;

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();  
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        
        var app = builder.Build();

        Configure(app, builder.Environment);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddLogging(config => { config.AddConsole(); });
       
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
        
        builder.Services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

        builder.Services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });
    }

    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Application has started.");

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });

        app.UseHttpsRedirection();
    
        app.UseRouting(); // add UseRouting before UseAuthorization and UseEndpoints 

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}