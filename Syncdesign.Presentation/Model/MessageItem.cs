using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncdesign.Presentation.Model
{
    public class MessageItem
    {
        public string Avatar { get; set; } = string.Empty;   // 头像路径
        public string Name { get; set; } = string.Empty;     // 会话名称
        public DateTime LastTime { get; set; }                // 最后消息时间
        public string LastMessage { get; set; } = string.Empty;// 最后一条消息
        public int UnreadCount { get; set; }                  // 未读数量
    }
}
