using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Syncdesign.AutoCAD.DebugLog
{
    public static class DebugLogListener
    {
        public static void Main()
        {
            int port = 7071;

            // 初始化本地 Serilog 日志（控制台 + 文件可选）
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                //.WriteTo.File("debuglistener.log", rollingInterval: RollingInterval.Day) 
                .CreateLogger();

            var udp = new UdpClient(port);
            var endpoint = new IPEndPoint(IPAddress.Any, 0); 

            Log.Information("Syncdesign.AutoCAD UDP 调试日志监听端口 {port}", port);

            while (true)
            {
                try
                {
                    var bytes = udp.Receive(ref endpoint);
                    var message = Encoding.UTF8.GetString(bytes).Trim(); 
                    Log.Information(message); 
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to process UDP log message");
                }
            }
        }
 
    }
}
