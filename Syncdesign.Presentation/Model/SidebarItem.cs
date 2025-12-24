using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Syncdesign.Presentation.Model;

public partial class SidebarItem : ObservableObject
{ 
    public string ?Title { set; get; }

    public string? Symbol { set; get; }

    [ObservableProperty]
    private bool isSelected;

    public IRelayCommand? ClickCommand { get; set; }
}
