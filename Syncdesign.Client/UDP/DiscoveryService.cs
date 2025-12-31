using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Syncdesign.Client.UDP
{
    /// <summary>
    /// 局域网设备信息
    /// </summary>
    public class PeerInfo
    {
        public string DeviceId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public IPEndPoint EndPoint { get; set; } = default!;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public Ed25519PublicKeyParameters? IdentityPublicKey { get; set; } // 可选，后续握手用
    }

    /// <summary>
    /// 广播/监听局域网发现服务
    /// </summary>
    public class DiscoveryService : IDisposable
    {
        private const int BroadcastPort = 22027;
        private const int BroadcastIntervalMs = 3000;

        private readonly string _deviceId;
        private readonly string _name;
        private readonly int _listenPort;

        private readonly UdpClient _udpSender;
        private readonly UdpClient _udpReceiver;

        private readonly CancellationTokenSource _cts = new();
        private readonly Task _broadcastTask;
        private readonly Task _receiveTask;

        private readonly ConcurrentDictionary<string, PeerInfo> _peers = new();

        public DiscoveryService(string deviceId, string name, int listenPort)
        {
            _deviceId = deviceId;
            _name = name;
            _listenPort = listenPort;

            // 广播客户端（发送）
            _udpSender = new UdpClient
            {
                EnableBroadcast = true
            };

            // 监听客户端（接收）
            _udpReceiver = new UdpClient(listenPort)
            {
                EnableBroadcast = true
            };

            _broadcastTask = Task.Run(BroadcastLoop);
            _receiveTask = Task.Run(ReceiveLoop);
        }

        /// <summary>
        /// 获取当前已发现设备列表
        /// </summary>
        public PeerInfo[] GetPeers()
        {
            var now = DateTime.UtcNow;
            // 移除超时设备
            foreach (var kvp in _peers)
            {
                if ((now - kvp.Value.LastSeen).TotalSeconds > 10)
                    _peers.TryRemove(kvp.Key, out _);
            }

            return _peers.Values.ToArray();
        }

        private async Task BroadcastLoop()
        {
            var endpoint = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var msg = new
                    {
                        type = "DISCOVERY",
                        deviceId = _deviceId,
                        name = _name,
                        udpPort = _listenPort,
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };

                    byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                    await _udpSender.SendAsync(data, data.Length, endpoint);
                }
                catch
                {
                    // 忽略异常，保证循环继续
                }

                await Task.Delay(BroadcastIntervalMs, _cts.Token);
            }
        }

        private async Task ReceiveLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await _udpReceiver.ReceiveAsync();
                    ProcessPacket(result.Buffer, result.RemoteEndPoint);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // 忽略异常，继续接收
                }
            }
        }

        private class DiscoveryPacket
        {
            public string type { get; set; } = "";
            public string deviceId { get; set; } = "";
            public string name { get; set; } = "";
            public int udpPort { get; set; }
        }

        private void ProcessPacket(byte[] data, IPEndPoint sender)
        {
            try
            {
                string json = Encoding.UTF8.GetString(data);
                var obj = JsonConvert.DeserializeObject<DiscoveryPacket>(json);

                if (obj == null || obj.type != "DISCOVERY")
                    return;

                if (obj.deviceId == _deviceId)
                    return; // 忽略自己

                var peer = new PeerInfo
                {
                    DeviceId = obj.deviceId,
                    Name = obj.name,
                    EndPoint = new IPEndPoint(sender.Address, obj.udpPort),
                    LastSeen = DateTime.UtcNow
                };

                _peers[obj.deviceId] = peer;
            }
            catch
            {
                // 忽略非法数据
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _udpSender.Dispose();
            _udpReceiver.Dispose();
        }
    }
}
