using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using SchoolDiary.Domain.Data;
using SchoolDiary.Domain.Services;
using SchoolDiary.Domain.Services.Interfaces;
using SchoolDiary.Helpers;
using SchoolDiary.Helpers.Interfaces;
using Microsoft.AspNetCore.CookiePolicy;
using System.Threading.Tasks;

namespace SchoolDiary
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding project services.
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IClassService, ClassService>();
            services.AddTransient<ITeachersEditService, TeachersEditService>();
            services.AddTransient<IStudentsEditService, StudentsEditService>();
            services.AddTransient<IScheduleEditService, ScheduleEditService>();
            services.AddHttpContextAccessor();
            // Adding database context.
            services.AddDbContext<DataContext>(options => 
                options.UseSqlServer(Configuration
                    .GetConnectionString("DefaultConnection")));
            // Authentication settins.
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config =>
                {
                    config.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.ContainsKey("refregeratorprice"))
                            {
                                context.Token = context.Request.Cookies["refregeratorprice"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidAudience = AuthOptions.AUDIENCE,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                    };
                });
            services.AddCors();
            services.AddControllers();
            services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            });
            app.UseAuthentication();
            app.UseAuthorization();
            // For correctly working with vue front.
            // Allowing cross-domain queries!
            app.UseCors(c => c
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment())
                {
                    // Launch development server for Vue.js.
                    spa.UseVueDevelopmentServer();
                }
            });
        }
    }
}
