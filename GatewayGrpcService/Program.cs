using GatewayGrpcService.Queries;
using GatewayGrpcService.Services;
using Grpc.Net.Client;
using Grpc.Core;

namespace GatewayGrpcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            if (connectionString != null)
            {
                connectionString = connectionString.Replace("{#host}", dbHost).Replace("{#dbName}", dbName).Replace("{#dbPassword}", dbPassword);
            }

            builder.Services.AddScoped<IGatewayRequestQueries>(sp => new GatewayRequestQueries(connectionString));
            builder.Services.AddGrpc().AddJsonTranscoding();
            builder.Services.AddGrpcReflection();
            builder.Services.AddGrpcSwagger();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Gateway Grpc Service", Version = "v1" });
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "gRPC using .NET7 Demo");
            });

            // Configure the HTTP request pipeline.
            app.MapGrpcService<RsiMessageService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.MapGrpcReflectionService();
            app.Run();
        }
    }
}