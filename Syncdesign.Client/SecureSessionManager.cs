using Org.BouncyCastle.Crypto.Parameters;

namespace Syncdesign.Client;

/// <summary>
/// 会话管理器
/// </summary>
public class SecureSessionManager
{
    private readonly string _localDeviceId;
    private readonly Ed25519PrivateKeyParameters _localPrivateKey;

    // Key: remoteDeviceId, Value: SecureSession
    private readonly Dictionary<string, SecureSession> _sessions = new();

    private readonly object _lock = new();

    public SecureSessionManager(string localDeviceId, Ed25519PrivateKeyParameters localPrivateKey)
    {
        _localDeviceId = localDeviceId;
        _localPrivateKey = localPrivateKey;
    }

    /// <summary>
    /// 获取现有 session 或创建新 session
    /// </summary> 
    public SecureSession GetOrCreateSession(
        string remoteDeviceId,
        Ed25519PublicKeyParameters remotePublicKey)
    {
        lock (_lock)
        {
            if (_sessions.TryGetValue(remoteDeviceId, out var session))
            {
                // 如果失效，删除重建
                if (session.State != SecureSessionState.Established)
                {
                    session.Close();
                    _sessions.Remove(remoteDeviceId);
                }
                else
                {
                    return session;
                }
            }

            // 创建新的 session
            session = new SecureSession(_localDeviceId, _localPrivateKey);
            _sessions[remoteDeviceId] = session;

            // 主动发起握手
            var handshakeMsg = session.CreateHandshake();
            // 这里需要通过网络发送 handshakeMsg 给 remoteDevice
            // 例如 SignalR / TCP
            return session;
        }
    }

    /// <summary>
    /// 收到远端 handshake 消息
    /// </summary> 
    public void ProcessRemoteHandshake(
        string remoteDeviceId,
        HandshakeMessage remoteMsg,
        Ed25519PublicKeyParameters remotePublicKey)
    {
        lock (_lock)
        {
            var session = GetOrCreateSession(remoteDeviceId, remotePublicKey);

            // 如果已经是 Established，可以忽略或重新握手
            if (session.State != SecureSessionState.Established)
            {
                var replyMsg = session.ProcessHandshake(remoteMsg, remotePublicKey);

                // replyMsg 发送回远端
            }
        }
    }

    public void RemoveSession(string remoteDeviceId)
    {
        lock (_lock)
        {
            if (_sessions.TryGetValue(remoteDeviceId, out var session))
            {
                session.Close();
                _sessions.Remove(remoteDeviceId);
            }
        }
    }

    public IEnumerable<SecureSession> GetAllSessions()
    {
        lock (_lock)
        {
            return _sessions.Values.ToList();
        }
    }
}

/**
 *  使用方法
    
    // 本地身份
    var localDeviceId = "A";
    var localPrivateKey = deviceAPrivateKey;

    // 远端身份
    var remoteDeviceId = "B";
    var remotePublicKey = deviceBPublicKey;

    // 创建 manager
    var manager = new SecureSessionManager(localDeviceId, localPrivateKey);

    // 发送消息
    var session = manager.GetOrCreateSession(remoteDeviceId, remotePublicKey);
    byte[] cipher = session.Encrypt(Encoding.UTF8.GetBytes("Hello B"));

    // 收到远端 handshake
    manager.ProcessRemoteHandshake(remoteDeviceId, handshakeMsgFromB, remotePublicKey);

    // 移除会话
    manager.RemoveSession(remoteDeviceId);

 */
