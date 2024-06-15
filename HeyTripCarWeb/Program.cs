using CommonCore;
using CommonCore.Dependency;
using CommonCore.Enum;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ABG.Config;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Reflection;

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
Log.Information("Starting NKFlight WebApi");
builder.Services.AddControllers()
              .AddNewtonsoftJson(op =>
              {
                  op.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                  op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                  op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
              });
// �����־
builder.Logging.AddConsole();

// ���Swagger��������UI
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});
builder.Services.Configure<ABGAppSetting>(builder.Configuration.GetSection("ABGAppSetting"));
// ������ݿ�����
string connectionString = builder.Configuration.GetConnectionString("DbConnection");
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

// ������ݷ��ʲ�
//builder.Services.AddTransient(typeof(IRepository<>), typeof(DapperRepository<>));
builder.Services.AddScopedDependencies(Assembly.GetExecutingAssembly()).AddMapper();

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

app.UseAuthorization();

app.MapControllers();

app.Run();