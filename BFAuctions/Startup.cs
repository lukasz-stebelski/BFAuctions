using BFAuctions.Processors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//using UbudKusCoin.Grpc;

namespace BFAuctions
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");



            }));

        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AccountProcessorImpl>().RequireCors("AllowAll");
                endpoints.MapGrpcService<AuctionProcessorImpl>().RequireCors("AllowAll");
                endpoints.MapGrpcService<BidProcessorImpl>().RequireCors("AllowAll");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(
                        "Use gRPC client please");
                });

            });
        }
    }
}
