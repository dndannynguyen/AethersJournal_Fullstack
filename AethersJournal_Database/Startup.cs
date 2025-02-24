public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure MongoDB Service with settings from appsettings.json
        services.Configure<MongoDBSettings>(Configuration.GetSection("MongoDB"));
        services.AddSingleton<MongoDBService>();

         var allowedOrigin = Configuration["ALLOWED_ORIGIN"];

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins(allowedOrigin) // Add allowed origins here
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        // Add controllers
        services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Enable CORS
        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=login}/{id?}");
        });
    }
}
