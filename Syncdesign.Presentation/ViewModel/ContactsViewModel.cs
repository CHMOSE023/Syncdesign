using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace Syncdesign.Presentation.ViewModel;

public enum ContactTab
{
    好友,
    项目,
}
public class UserItem
{
    public string? UserName { get; set; }
    public string ?GroupName { get; set; } // "内部组", "外部组", "其他组"
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

    [ObservableProperty]
    public ICollectionView? contactView;

    /// <summary>
    /// 选项发生变化时
    /// </summary> 
    partial void OnCurrentTabChanged(ContactTab? value)
    {
        Debug.WriteLine($"ContactsViewModel: ContactTab {value} , newValue {value}");
        LoadData(value);
    }

    private void LoadData(ContactTab? tab)
    {
        // 1. 根据当前 Tab 获取原始数据
        var rawData = tab switch
        {
            ContactTab.好友 => GetFriendData(),
            ContactTab.项目 => GetProjectData(),
            _ => new List<UserItem>()
        };

        // 2. 创建 CollectionView 包装器
        var view = CollectionViewSource.GetDefaultView(rawData);

        // 3. 【核心】配置分组逻辑
        // 只要 GroupDescriptions 里添加了属性名，UI 里的 GroupItem 就会自动生效
        view.GroupDescriptions.Clear();
        view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(UserItem.GroupName)));

        ContactView = view;
    }

    // 模拟数据源
    private List<UserItem> GetFriendData() => new()
    {
        new UserItem { UserName = "用户A", GroupName = "内部" },
        new UserItem { UserName = "用户B", GroupName = "内部" },
        new UserItem { UserName = "用户D", GroupName = "外部" },
        new UserItem { UserName = "用户H", GroupName = "其他" }
    };

    private List<UserItem> GetProjectData() => new()
    {
        new UserItem { UserName = "同步设计插件", GroupName = "自研项目" },
        new UserItem { UserName = "BIM 数据中台", GroupName = "公司项目" }
    };
}

