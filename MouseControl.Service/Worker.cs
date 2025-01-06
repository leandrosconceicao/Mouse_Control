using Microsoft.AspNetCore.SignalR.Client;
using MouseControl.Domain.Entities;
using MouseControl.Domain.Interfaces;

namespace MouseControl.Service
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IMouseHandler _mouseHandler;

        public Worker(IConfiguration configuration, IMouseHandler mouseHandler)
        {
            _config = configuration;
            _mouseHandler = mouseHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string host = _config.GetSection("SocketUrl").Value ?? throw new ArgumentNullException("Url do socket é inválida");

            await using var connection = new HubConnectionBuilder()
                .WithUrl(host)
                .Build();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    switch (connection.State)
                    {
                        case HubConnectionState.Disconnected:

                            await connection.StartAsync(stoppingToken);
                            connection.On<MousePosition>("SetPosition", position =>
                            {
                                Console.WriteLine($"Posição do mouse {position}");
                                _mouseHandler.SetCursor(Convert.ToInt32(position.X), Convert.ToInt32(position.Y));
                            });

                            connection.On("LeftClick", () =>
                            {
                                Console.WriteLine("Botão esquerdo do mouse clicado");
                                _mouseHandler.MouseLeftClick();
                            });

                            connection.On("RightClick", () =>
                            {
                                Console.WriteLine("Botão direito do mouse clicado");
                                _mouseHandler.MouseRightClick();
                            });
                            break;

                        default:
                            break;

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
