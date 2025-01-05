using Microsoft.AspNetCore.SignalR.Client;
using MouseControl.Domain.Entities;
using System.Runtime.InteropServices;

namespace MouseControl.Service
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _config;

        public Worker(IConfiguration configuration)
        {
            _config = configuration;
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
                                SetCursorPos(Convert.ToInt32(position.X), Convert.ToInt32(position.Y));
                            });

                            connection.On("LeftClick", () =>
                            {
                                Console.WriteLine("Botão esquerdo do mouse clicado");
                                MouseLeftClick();
                            });

                            connection.On("RightClick", () =>
                            {
                                Console.WriteLine("Botão direito do mouse clicado");
                                MouseRightClick();
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

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private void MouseLeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private void MouseRightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
    }
}
