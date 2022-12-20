using System.Net.Sockets;
using System.Text;

namespace UdpListener
{
    internal class UdpListener
    {
        private const string ExitCommand = "exit";

        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Console.WriteLine("Invalid number of parameters. Expected one parameter (local port)");
                Console.ReadLine();
            }
            else
            {
                if (int.TryParse(args[0], out var port))
                {
                    var cts = new CancellationTokenSource();
                    Task task = null;
                    try
                    {
                        task = RunUdpListener(port, cts.Token);
                        var cmd = Console.ReadLine();
                        while (cmd != ExitCommand)
                        {
                            cmd = Console.ReadLine();
                        }
                        cts.Cancel();
                        task.Wait();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally 
                    {
                        task?.Dispose();
                        cts.Dispose();
                    }
                }
                else
                {
                    Console.WriteLine($"Cannot parse port {args[0]}");
                }
            }
        }

        private static async Task RunUdpListener(int localPort, CancellationToken token)
        {
            UdpClient udpClient = null;
            try
            {
                Console.WriteLine($"Start listening on port {localPort}");
                udpClient = new UdpClient(localPort);

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    var result = await udpClient.ReceiveAsync(token);
                    var data = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine($"Received from {result.RemoteEndPoint}: {data}");
                }
            }
            finally
            {
                udpClient?.Dispose();
            }
        }
    }
}