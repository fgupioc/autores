using ApiWebAutores.Middleware;
using ApiWebAutores.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiWebAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers().AddNewtonsoftJson(x =>
 x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddDbContext<ApplicationDbContex>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddTransient<IAutorService, AutorService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opc => opc.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llaveJwt"])
                        ),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiWebAutores", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
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
                    new string[]{}
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContex>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(opc => {
                opc.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
                opc.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
            });
            /*
            services.AddCors( opc => {
                opc.AddDefaultPolicy( builder => {
                    builder.WithOrigins("http//localhost:4200/").AllowAnyMethod().AllowAnyHeader();
                });
            });
            */

            services.AddCors( opc => {
                opc.AddPolicy("AllowOrigin", opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            } );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
            app.Use(async (contexto, siguiente) => {
                using (var ms = new MemoryStream()) {
                    var cuerpoOriginar = contexto.Response.Body;
                    contexto.Response.Body = ms;
                    await siguiente.Invoke();


                    ms.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(ms).ReadToEnd();

                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(cuerpoOriginar);
                    contexto.Response.Body = cuerpoOriginar;


                } 
            });
            */

            // app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();

            app.UseLoguearRespuestaHTTP();

            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("hola");
                });
            });

           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiWebAutores v1"));
            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseCors();
            app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
