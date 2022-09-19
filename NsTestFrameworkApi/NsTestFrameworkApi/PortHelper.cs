using System.Net;
using System.Net.Sockets;

namespace NsTestFrameworkApi;

public static class PortHelper
{
    public static (int Port1, int Port2) Ports { get; } = GetAvailablePorts();

    private static (int Port1, int Port2) GetAvailablePorts()
    {
        var ip = Dns.GetHostEntry("localhost").AddressList[0];

        TcpListener listener1 = new(ip, 0);
        listener1.Start();
        var port1 = ((IPEndPoint)listener1.LocalEndpoint).Port;

        TcpListener listener2 = new(ip, 0);
        listener2.Start();
        var port2 = ((IPEndPoint)listener2.LocalEndpoint).Port;

        listener1.Stop();
        listener2.Stop();

        return (port1, port2);
    }

    public static string BuildUrlUsingAvailablePort(string host, int port)
        => $"{host}:{port}/";
}