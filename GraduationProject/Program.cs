
using GraduationProject.data;
using Microsoft.EntityFrameworkCore;
using GraduationProject.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using GraduationProject.Services;

namespace GraduationProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
                ).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient<RecommendationService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("InstructorPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser(); // Ensure the user is authenticated
                    policy.RequireClaim("Role", "teacher"); // Ensure the user has the "Role" claim with value "teacher"
                });
                options.AddPolicy("InstuctandandadminandstudentPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser(); 
                    policy.RequireClaim("Role", "admin", "teacher","student"); 
                });
                options.AddPolicy("InstructorAndAdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser(); // Ensure the user is authenticated
                    policy.RequireClaim("Role", "teacher","admin"); // Ensure the user has the "Role" claim with value "teacher"
                });
                options.AddPolicy("AdminPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser(); // Ensure the user is authenticated
                    policy.RequireClaim("Role","admin"); // Ensure the user has the "Role" claim with value "teacher"
                });
                options.AddPolicy("TeacherPolicy", policy
                    => {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("Role", "teacher");
                    });
                options.AddPolicy("StudentPolicy", policy 
                    => {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("Role", "student");
                   } );
            });
             Xabe.FFmpeg.Downloader.FFmpegDownloader.GetLatestVersion(Xabe.FFmpeg.Downloader.FFmpegVersion.Official);


            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<EmailService>();
            // Add services to the container.
            builder.Services.AddDbContext<AppDBContext>(options =>
             options.UseSqlServer(builder.Configuration.
             GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
       });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GraduationProject API", Version = "v1" });
                c.OperationFilter<FileUploadOperationFilter>();
                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            //builder.Services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // Set limit to 200 MB
            //});
            builder.WebHost.
                ConfigureKestrel(
                options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);

            var app = builder.Build();

          
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
          

            app.MapControllers();

            app.Run();
        }
    }
}
