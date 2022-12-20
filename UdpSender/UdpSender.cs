using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UdpSender
{
    internal class UdpSender
    {
        private const string ExitCommand = "exit";

        public static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("Invalid number of parameters. Expected two parameters (remote endpoint ip address and port)");
                Console.ReadLine();
            }
            else
            {
                var cts = new CancellationTokenSource();
                if (IPAddress.TryParse(args[0], out var ip))
                {
                    if (int.TryParse(args[1], out var port))
                    {
                        var task = Task.Run(() => RunUdpSender(ip, port, cts.Token));
                        var cmd = Console.ReadLine();
                        while (cmd != ExitCommand)
                        {
                            cmd = Console.ReadLine();
                        }
                        cts.Cancel();
                        task.GetAwaiter().GetResult();
                        task.Dispose();
                    }
                    else
                    {
                        Console.WriteLine($"Cannot parse port {args[1]}");
                    }
                }
                else
                {
                    Console.WriteLine($"Cannot parse IP address {args[0]}");
                }

                cts.Dispose();
            }
        }

        private static void RunUdpSender(IPAddress ip, int port, CancellationToken token)
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                var sender = new IPEndPoint(ip, port);

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var text = $"{DateTime.Now.ToString("F")}: UDP sender";
                    var data = Encoding.ASCII.GetBytes(text);

                    socket.SendTo(data, sender);

                    Thread.Sleep(3000);
                }
                socket.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}