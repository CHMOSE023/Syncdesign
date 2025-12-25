using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Syncdesign.Presentation.Model;

namespace Syncdesign.Presentation.ViewModel;

public partial class ContainerViewModel : ObservableObject
{ 
    public ContainerViewModel(UserListViewModel userListVM,SidebarViewModel sidebarViewModel)
    {
        var messenger = WeakReferenceMessenger.Default;
        // 像注册路由一样注册逻辑，不需要实现 IRecipient 接口
        messenger.Register<SidebarItemMessage>(this, (r, m) => HandleSidebar(m.Value));
        messenger.Register<SearchItemMessage>(this, (r, m) => HandleSearch(m.Value));
         
        UserListVM = userListVM;
        SidebarVM= sidebarViewModel;
    }

    [ObservableProperty]
    private UserListViewModel? _userListVM;

    [ObservableProperty]
    private SidebarViewModel? _sidebarVM;

    [ObservableProperty]
    private string? siderbar;

    [ObservableProperty]
    private string? search;

    public void HandleSidebar(SidebarItem selectedItem)
    {
        Siderbar = selectedItem.Title; 
    }

    public void HandleSearch(SearchItem searchItem)
    {
        Search = searchItem.Name;
    }
}
