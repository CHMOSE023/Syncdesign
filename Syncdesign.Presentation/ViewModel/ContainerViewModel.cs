using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Syncdesign.Presentation.Model;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel;

public partial class ContainerViewModel : ObservableObject
{ 
    public ContainerViewModel(UserListViewModel userListViewModel, SidebarViewModel sidebarViewModel,MessageListViewModel messageListViewModel)
    {
        var messenger = WeakReferenceMessenger.Default;
        // 像注册路由一样注册逻辑，不需要实现 IRecipient 接口
        messenger.Register<SidebarItemMessage>(this, (r, m) => HandleSidebar(m.Value));
        messenger.Register<SearchItemMessage>(this, (r, m) => HandleSearch(m.Value));
         
        UserListVM = userListViewModel;
        SidebarVM= sidebarViewModel;
        MessageListVM = messageListViewModel;
    }

    [ObservableProperty]
    private UserListViewModel? _userListVM;

    [ObservableProperty]
    private SidebarViewModel? _sidebarVM;

    [ObservableProperty]
    private MessageListViewModel? _messageListVM;

    [ObservableProperty]
    private string? siderbar;

    [ObservableProperty]
    private string? search;

    public void HandleSidebar(SidebarItem selectedItem)
    {
        // 导航到不同组件
        // ContentControl + DataTemplateSelector 来根据不同消息类型动态显示不同的 View。

    }

    public void HandleSearch(SearchItem searchItem)
    {
        Search = searchItem.Name;
    }
}
