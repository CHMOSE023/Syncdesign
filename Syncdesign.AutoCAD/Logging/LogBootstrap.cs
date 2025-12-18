using Serilog;
using Serilog.Events;
using System.IO;
using System.Net.Sockets;

namespace Syncdesign.AutoCAD.Logging
{
    public static class LogBootstrap
    {
        private static bool _initialized = false;

        public static void Init()
        {
            
            if (_initialized) return;
            _initialized = true; 

            var logPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "Syncdesign", "AutoCAD", "plugin.log");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.File(
                //    logPath,
                //    rollingInterval: RollingInterval.Day,
                //    retainedFileCountLimit: 7,
                //    shared: true)
                .WriteTo.Udp(
                    "127.0.0.1",
                    7071,
                    AddressFamily.InterNetwork,
                    formatter: new Serilog.Formatting.Json.JsonFormatter(),
                    localPort: 0,
                    enableBroadcast: false,
                    restrictedToMinimumLevel: LogEventLevel.Debug)
                .CreateLogger();
        }
    }
}
