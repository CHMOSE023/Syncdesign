using CommunityToolkit.Mvvm.ComponentModel;
using Syncdesign.Presentation.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel
{
    public partial class MessageListViewModel : ObservableObject
    {
        public MessageListViewModel()
        {
            Messages?.Add(new()
            {
                Avatar="",
                LastTime=DateTime.Now,
                Name="项目群1",
                LastMessage="最后一条消息1" 
            });
            Messages?.Add(new()
            {
                Avatar = "",
                LastTime = DateTime.Now,
                Name = "项目群2",
                LastMessage = "最后一条消息2",
                UnreadCount = 2,
            });
            Messages?.Add(new()
            {
                Avatar = "",
                LastTime = DateTime.Now,
                Name = "马工",
                LastMessage = "最后一条消息3",
                UnreadCount = 3,
            });
            Messages?.Add(new()
            {
                Avatar = "",
                LastTime = DateTime.Now,
                Name = "张工",
                LastMessage = "最后一条消息4",
                UnreadCount = 5,
            });
            Messages?.Add(new()
            {
                Avatar = "",
                LastTime = DateTime.Now,
                Name = "刘工",
                LastMessage = "最后一条消息5",
                UnreadCount = 0,
            });
            Messages?.Add(new()
            {
                Avatar = "",
                LastTime = DateTime.Now,
                Name = "项目群4",
                LastMessage = "最后一条消息7",
                UnreadCount =10,
            });
        }
         
        [ObservableProperty]
        private ObservableCollection<MessageItem>? _messages=[];

        [ObservableProperty]
        private MessageItem? _selected;

        /// <summary>
        /// 选中
        /// </summary> 
        partial void OnSelectedChanged( MessageItem? value)
        {
            Debug.WriteLine($"OnSignatureChanged: Name {value?.Name} , LastTime {value?.LastTime}");
        }

    }
}
