using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobList;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.INFRASTRUCTURE.Configurations;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using JOBPORTALWEB.INFRASTRUCTURE.Data.Seed;
using JOBPORTALWEB.INFRASTRUCTURE.Persistence;
using JOBPORTALWEB.INFRASTRUCTURE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ITPM_JOBPORTALWEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. CẤU HÌNH DỊCH VỤ
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // A. DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                    b => b.MigrationsAssembly("JOBPORTALWEB.INFRASTRUCTURE")
                ));

            // B. Identity
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Cấu hình password đơn giản cho dễ test
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // C. Services & Repositories
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            // D. JWT & External Auth
            var jwtKey = builder.Configuration["Jwt:Key"]!;
            var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
            var jwtAudience = builder.Configuration["Jwt:Audience"]!;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var googleClientId = builder.Configuration["Google:ClientId"]!;
            var googleClientSecret = builder.Configuration["Google:ClientSecret"]!;
            var facebookAppId = builder.Configuration["Facebook:AppId"]!;
            var facebookAppSecret = builder.Configuration["Facebook:AppSecret"]!;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero
                };

                // Cấu hình đọc token từ query string cho SignalR (vẫn giữ để ai có token thì dùng được)
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs/notifications")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
                options.CallbackPath = "/signin-google";
            })
            .AddFacebook(options =>
            {
                options.AppId = facebookAppId;
                options.AppSecret = facebookAppSecret;
                options.CallbackPath = "/signin-facebook";
                options.Scope.Add("email");
                options.Scope.Add("public_profile");
            });

            // F. CORS (MỞ RỘNG TỐI ĐA - AllowAnyOrigin)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                    policy =>
                    {
                        policy.SetIsOriginAllowed(origin => true)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials(); 
                    });
            });

            // G. MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetJobListQuery).Assembly));

            // H. Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // I. SignalR
            builder.Services.AddSignalR();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            // J. Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Job Portal API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập 'Bearer ' theo sau là token JWT."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

            // 2. APP PIPELINE
            var app = builder.Build();

            // Seed Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    await ApplicationDbContextSeed.SeedRolesAsync(roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database roles.");
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Portal API V1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            // Mapping SignalR Hub (BỎ RequireAuthorization để ai cũng connect được)
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.MapControllers();

            await app.RunAsync();
        }
    }
}