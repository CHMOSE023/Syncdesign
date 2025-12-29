using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Syncdesign.Presentation.Model;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel;

public partial class ContainerViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    public ContainerViewModel(IServiceProvider serviceProvider)
    {
        var messenger = WeakReferenceMessenger.Default;
        // 像注册路由一样注册逻辑，不需要实现 IRecipient 接口
        messenger.Register<SidebarItemMessage>(this, (r, m) => HandleSidebar(m.Value));
        messenger.Register<SearchItemMessage>(this, (r, m) => HandleSearch(m.Value));

        _serviceProvider = serviceProvider;
        CurrentDynamicVM = _serviceProvider.GetService<ContactsViewModel>();
    }
     
     
    [ObservableProperty]
    private string? siderbar;

    [ObservableProperty]
    private string? search;

    [ObservableProperty]
    private object? _currentDynamicVM;

    public void HandleSidebar(SidebarItem selectedItem)
    {
        // 导航到不同组件 
        Debug.WriteLine($"SidebarItem NavType: {selectedItem.NavType}");
        
         CurrentDynamicVM = selectedItem.NavType switch
        {
            // 从容器中获取
            NavType.Messages => _serviceProvider.GetService<MessageListViewModel>(),
            NavType.Contacts => _serviceProvider.GetService<ContactsViewModel>(),
            NavType.Projects => _serviceProvider.GetService<ProjectsViewModel>(),
            NavType.Tasks => _serviceProvider.GetService<TasksViewModel>(),
            NavType.Reviews => _serviceProvider.GetService<ReviewsViewModel>(),
            NavType.Workflow => _serviceProvider.GetService<WorkflowViewModel>(),
            NavType.Settings => _serviceProvider.GetService<SettingsViewModel>(),
            _ => CurrentDynamicVM
        };
    }

    public void HandleSearch(SearchItem searchItem)
    {
        Search = searchItem.Name;
    }
}
