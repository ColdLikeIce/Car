using CommonCore;
using CommonCore.Dependency;
using CommonCore.Enum;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Share.Dtos;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Worker;
using HeyTripCarWeb.Supplier.ACE.Config;
using HeyTripCarWeb.Supplier.BarginCar.Config;
using HeyTripCarWeb.Supplier.NZ.Config;
using HeyTripCarWeb.Supplier.Sixt.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Reflection;
using System.Text;

var envir = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("Serilog.json")
.AddJsonFile($"Serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
.Build();
var logger = new LoggerConfiguration()
   .ReadFrom.Configuration(configuration)
   .CreateLogger();
Log.Logger = logger;
var builder = WebApplication.CreateBuilder(args);
Log.Information($"Starting Car {envir} WebApi");
//ע�����
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, //�Ƿ���֤Issuer
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"), //������Issuer
        ValidateAudience = true, //�Ƿ���֤Audience
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"), //������Audience
        ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:SecretKey"))), //SecurityKey
        ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
        ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
        RequireExpirationTime = true,
    };
});
builder.Services.AddControllers()
              .AddNewtonsoftJson(op =>
              {
                  op.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                  op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                  op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
              });
// ��������ļ�����
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

// �����־
builder.Logging.AddConsole();

builder.Services.AddHostedService<SFTPToDbWorker>();
builder.Services.AddHostedService<LogToDbWorker>();

// ���Swagger��������UI
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
// ��� Swagger ����
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // ���� Swagger ʹ�� JWT ��֤
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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

builder.Services.AddSingleton(new JwtHelper());
builder.Services.Configure<ABGAppSetting>(builder.Configuration.GetSection("ABGAppSetting"));
builder.Services.Configure<AceAppSetting>(builder.Configuration.GetSection("AceAppSetting"));
builder.Services.Configure<BarginCarAppSetting>(builder.Configuration.GetSection("BarginCarAppSetting"));
builder.Services.Configure<SixtAppSetting>(builder.Configuration.GetSection("SixtAppSetting"));
builder.Services.Configure<NZCarAppSetting>(builder.Configuration.GetSection("NZCarAppSetting"));

builder.Services.AddAutoIoc(typeof(IScopedDependency), LifeCycle.Scoped)
       .AddAutoIoc(typeof(ISingletonDependency), LifeCycle.Singleton)
       .AddAutoIoc(typeof(ITransientDependency), LifeCycle.Transient)
       .AddMapper();
// ������ݷ��ʲ�
builder.Services.AddHyTripEntityFramework<CarRentalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarRentalDb"));
});
builder.Services.AddHyTripEntityFramework<CarSupplierDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarSupplierDb"));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseKnife4UI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseKnife4UI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();