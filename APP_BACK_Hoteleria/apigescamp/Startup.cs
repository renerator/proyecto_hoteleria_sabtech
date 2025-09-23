using AutoMapper;
using DemoBackend.Configurations;
using DemoBackend.Models;
using DemoBackend.Repositories;
using DemoBackend.Repository;
using DemoBackend.RepositoryGes;
using DemoBackend.Services;
using DemoBackend.Services.Autenticacion;
using DemoBackend.Services.Mantenedores;
using DemoBackend.Services.Habitacion;
using DemoBackend.Services.Reserva;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
//using Microsoft.OpenApi.Models;
using System;
using System.Text;


namespace DemoBackend
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

            try
            {
                services.AddCors();
                services.AddControllers();
                services.AddMvc();
                // Escanea los perfiles de AutoMapper en tu ensamblado

                services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
                AddSwagger(services);
                services.Configure<IISServerOptions>(options =>
                {
                    options.AutomaticAuthentication = false;
                });

                services.AddDbContext<CTRContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CONNECTIONSTRINGSQL.CTR")));
                services.AddScoped(typeof(Repositories.IGenericRepositoryCTR<>), typeof(GenericRepositoryCTR<>));
                services.AddScoped(typeof(Repository.IGenericRepositoryCTREntity<>), typeof(GenericRepositoryCTREntity<>));

                services.AddDbContext<GESContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CNN_Sistema")));
                services.AddScoped(typeof(Repositories.IGenericRepositories<>), typeof(GenericRepositories<>));
                services.AddScoped(typeof(RepositoryGes.IGenericRepositoryEntity<>), typeof(GenericRepositoriesGes<>));

                services.AddTransient<IAutenticacionService, AutenticacionService>();
                services.AddTransient<IMantenedoresService, MantenedoresService>();
                services.AddTransient<IHabitacionService, HabitacionService>();
                services.AddTransient<IReservaService, ReservaService>();

                services.AddTransient<IValidaUsuarioService, ValidaUsuarioService>();
                services.Configure<CredencialesConfig>(Configuration.GetSection("CredencialesConfig"));


                services.AddCors();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void AddSwagger(IServiceCollection services)
        {
            string urlEmpresa = Configuration.GetSection("URL").GetSection("EMPRESA").Value;
            string email = Configuration.GetSection("URL").GetSection("EMAIL").Value;

            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";
                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"API Gestión de Campamentos BACKEND {groupName}",
                    Version = groupName,
                    Description = "API Gestión de Campamentos",
                    Contact = new OpenApiContact
                    {
                        Name = "Integraciones Sistema Gestión de Campamentos",
                        Email = email,
                        Url = new Uri(urlEmpresa),
                    }
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}
                    }
                });
            });
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) //Configuration["JwtToken:SecretKey"]
                };
            });

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(option =>
            {
                option.AllowAnyMethod();
                option.AllowAnyHeader();
                option.SetIsOriginAllowed(origin => true);
                option.AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API DEMO BACKEND v1");
            });

        }
    }
}
