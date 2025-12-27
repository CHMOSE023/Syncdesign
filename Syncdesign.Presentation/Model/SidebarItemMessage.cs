using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Syncdesign.Presentation.Model;

// 定义一个消息，传递选中的侧边栏项
public class SidebarItemMessage(SidebarItem value) : ValueChangedMessage<SidebarItem>(value)
{
}