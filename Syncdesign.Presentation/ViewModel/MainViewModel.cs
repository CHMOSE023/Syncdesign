using CommunityToolkit.Mvvm.ComponentModel; 

namespace Syncdesign.Presentation.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(ContainerViewModel containerVM, SidebarViewModel sidebarVM, HeaderViewModel headerVM)
    {
        ContainerVM = containerVM;
        SidebarVM = sidebarVM;
        HeaderVM = headerVM;
    }

    [ObservableProperty]
    private SidebarViewModel? _sidebarVM;

    [ObservableProperty]
    private ContainerViewModel? _containerVM;

    [ObservableProperty]
    private HeaderViewModel? _headerVM;
   
}
