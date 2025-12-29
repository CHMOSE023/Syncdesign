using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel;

public enum ContactTab
{
    好友, 
    项目,
}

/// <summary>
/// 联系人
/// </summary>
public partial class ContactsViewModel : ObservableObject
{
    public ContactsViewModel()
    {
        Tabs = Enum.GetValues(typeof(ContactTab)).Cast<ContactTab>();
        CurrentTab = ContactTab.好友;
    }

    [ObservableProperty]
    private IEnumerable<ContactTab>? tabs;

    [ObservableProperty]
    private ContactTab? currentTab;

    /// <summary>
    /// 选项发生变化时
    /// </summary> 
    partial void OnCurrentTabChanged(ContactTab? value) 
    {
        Debug.WriteLine($"ContactsViewModel: ContactTab {value} , newValue {value}");
    }
}

