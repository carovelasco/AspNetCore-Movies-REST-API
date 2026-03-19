using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRespositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
                opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//Soporte para autenticación con .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Agregar repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Agregar automapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Configurar autenticacion JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Soporte para cache
builder.Services.AddResponseCaching();

builder.Services.AddControllers(opcion =>
{
    //cache global para no ponerlo en todas partes
    opcion.CacheProfiles.Add("PorDefecto20Segundos", new CacheProfile() { Duration = 30 });
});

//Versionado de API
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = true;
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;
});

apiVersioningBuilder.AddApiExplorer(opciones =>
{
    opciones.GroupNameFormat = "'v'VVV";
    opciones.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "Autenticación JWT usando el esquema Bearer. \r\n\r\n" +
            "Ingresa la palabra 'Bearer' seguido de un [espacio] y después su token.\r\n\r\n" +
            "Ejemplo: \"Bearer tkljk125jhhk\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "PeliculasApi",
        Description = "Api de Peliculas",
        TermsOfService = new Uri("https://caro_apis/promociones"),
        Contact = new OpenApiContact
        {
            Name = "caro_apis",
            Url = new Uri("https://caro_apis/promociones")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://caro_apis/promociones")
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "PeliculasApi V2",
        Description = "Api de Peliculas",
        TermsOfService = new Uri("https://caro_apis/promociones"),
        Contact = new OpenApiContact
        {
            Name = "caro_apis",
            Url = new Uri("https://caro_apis/promociones")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://caro_apis/promociones")
        }
    });
});

builder.Services.AddCors(p => p.AddPolicy("PoliticaCORS", build =>
{
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
    });
}


//Soporte para archivos estaticos como imagen
app.UseStaticFiles();
app.UseHttpsRedirection();

//Soporte para cors
app.UseCors("PoliticaCORS");

//Soporte para cache
app.UseResponseCaching();

//Soporte para autenticacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();