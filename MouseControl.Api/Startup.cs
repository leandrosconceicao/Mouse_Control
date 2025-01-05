using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MouseControl.Api.Configurations;
using MouseControl.Domain.Entities;
using MouseControl.Domain.Interfaces;
using MouseControl.Service;

namespace MouseControl.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddSignalR().AddMessagePackProtocol();
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
            }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseWebSockets();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapPost("/send", (IHubContext<ChatHub, IChatHub> hub, [FromBody] MousePosition data) =>
                {
                    hub.Clients.All.SetPosition(data);
                });
            });
        }
    }
}
