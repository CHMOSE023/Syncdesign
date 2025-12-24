using CommunityToolkit.Mvvm.ComponentModel;
using Syncdesign.Presentation.View;

namespace Syncdesign.Presentation.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _containerView;
    public MainViewModel()
    { 
        _containerView = new Container();
        
    }
}
