using OfficialMemo.Context;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Serialization;
using System.Text.Json;
using OfficialMemo.Converters;
using DateOnlyTimeOnly.AspNet.Converters;
using OfficialMemo.Services;
using UserWS;
using OfficialMemo.Services.RestClients;
using System.Net.Http.Headers;
using System.Net;
using FileServer.Client;
using PdfProcessor.Client;
using Refit;
using Dapper;
using OfficialMemo.Models;
using OfficialMemo.Interfaces;
using TechSupportWS;

using LoggingHttpMessageHandler = Signer.Services.LoggingHttpMessageHandler;
using OfficialMemo.Logging;
using Signer.Services;
using Signer;
using HRServices;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseIISIntegration();

//if(builder.Environment.IsDevelopment()){
//    builder.WebHost.UseKestrel(options =>
//    {
//        options.Limits.MaxRequestLineSize = 100000;
//    });
//};


builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy",
    builder => builder.SetIsOriginAllowed(origin => origin.StartsWith("https://bpm2") || origin.StartsWith("http://localhost")
                                                   || origin.StartsWith("https://localhost") || origin.StartsWith("http://bpm2d"))
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials().WithExposedHeaders("Content-Disposition"));
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services
    .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
    .AddJsonOptions(options =>
    {
        options.UseDateOnlyTimeOnlyStringConverters();
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddScoped(_ =>
{
    var options = new JsonSerializerOptions();
    options.Converters.Add(new DateOnlyJsonConverter());
    options.Converters.Add(new TimeOnlyJsonConverter());
    return options;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IDbConnection, SqlConnection>(serviceProvider =>
    new SqlConnection(builder.Configuration.GetConnectionString("SQL")));

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddSingleton(_ => new SqlLoggerOptions { FlushPeriod = TimeSpan.FromMilliseconds(3000) });
builder.Services.AddScoped<SqlLoggerProvider>();
builder.Services.AddLogging(b => b.AddBusinessProcessLogger(options => options.FlushPeriod = TimeSpan.FromSeconds(3)));

builder.Services.AddScoped(_ => {
    UserWSSoapClient.ServiceUrl = builder.Configuration["Services:UserWs"];
    return new UserWSSoapClient(UserWSSoapClient.EndpointConfiguration.UserWSSoap12);
});

builder.Services.AddHttpClient<FileServerApiClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Services:FileServerApi"]);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        UseDefaultCredentials = true
    });

builder.Services.AddScoped<ISignRepository, SignRepository>();

builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SQL")));

builder.Services.AddScoped<EmployeesDbService>();
builder.Services.AddScoped<ProcessService>();
builder.Services.AddScoped<ProcessesDbService>();
builder.Services.AddScoped<TechSupportWSService>();
builder.Services.AddScoped<TasksService>();
builder.Services.AddScoped<HandbooksService>();

builder.Services.AddSignStamperService();

builder.Services.AddFileServerClient(client =>
        client.BaseAddress = new Uri(builder.Configuration["Services:FileServerApi"]))
    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler()
    { Credentials = CredentialCache.DefaultCredentials });

builder.Services.AddTransient<LoggingHttpMessageHandler>(_ => new LoggingHttpMessageHandler(new HttpClientHandler()));
builder.Services.AddRefitClient<IPdfApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.Configuration["Services:PdfProcessor"]))
    .ConfigurePrimaryHttpMessageHandler<LoggingHttpMessageHandler>();

builder.Services.AddScoped(_ =>
{
    TechSupportSoapClient.ServiceUrl = builder.Configuration["Services:TechSupportWs"];
    var client = new TechSupportSoapClient(TechSupportSoapClient.EndpointConfiguration.TechSupportSoap);
    client.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

    return client;
});


builder.Services.AddSingleton<IHRClient>(services =>
{
    var clientHandler = new HttpClientHandler()
    {
        UseDefaultCredentials = true,
    };
    var httpClient = new HttpClient(clientHandler)
    {
        Timeout = TimeSpan.FromMinutes(30)
    };
    return new HRClient(httpClient)
    {
        BaseUrl = builder.Configuration["Services:HRServerAPI"]
    };
});
builder.Services.AddTransient<PdfApiClient>();

SqlMapper.AddTypeHandler(new DocumentsArrayJsonConverter());
SqlMapper.AddTypeHandler(new RecipientsArrayJsonConverter());


MapsterConfig.Configure();
OfficialMemoMapsterConfig.Configure();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
