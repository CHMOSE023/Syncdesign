using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel;

public enum ContactTab
{
    好友,
    项目,
}

public class UserItem
{
    public string? UserName { get; set; }
    public string? GroupName { get; set; } // "内部组", "外部组", "其他组"
}

public class Contact
{
    public string? Name { get; set; }
    public string? Icon { get; set; } = "pack://application:,,,/Syncdesign.Ui;component/Resources/Images/avatar.jpg";
    public bool IsExpanded { get; set; }
}

public  class ContactGroup  
{
    public string ?GroupName { get; set; }

    public List<Contact> ?Items { get; set; }

    public bool? IsExpanded { get; set; }
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
    private IEnumerable<ContactGroup>? contactGroups;

    /// <summary>
    /// 选项发生变化时
    /// </summary> 
    partial void OnCurrentTabChanged(ContactTab? value)
    {
        Debug.WriteLine($"ContactsViewModel: ContactTab {value} , newValue {value}");
        LoadData(value);
    }

    [ObservableProperty]
    private object? currentSelectedObject;

    partial void OnCurrentSelectedObjectChanged(object? value)
    {
        if (value is Contact contact)
        {
            Debug.WriteLine($"选中了联系人: {contact.Name}");
        }
    }

    private void LoadData(ContactTab? tab)
    {
        // 1. 模拟获取原始数据（你之前的 UserItem 列表）
        var rawData = tab switch
        {
            ContactTab.好友 => GetFriendData(),
            ContactTab.项目 => GetProjectData(),
            _ => new List<UserItem>()
        };

        // 2. 使用 LINQ 进行分组转换
        // 将 List<UserItem> 转换为 List<ContactGroup>
        ContactGroups = rawData
            .GroupBy(x => x.GroupName)
            .Select(g => new ContactGroup
            {
                GroupName = g.Key ?? "未分组",
                IsExpanded = true, // 默认展开
                Items = g.Select(u => new Contact
                {
                    Name = u.UserName,
                    // 如果 UserItem 有 Icon 属性可以这里赋值
                }).ToList()
            })
            .ToList();
    } 

    private List<UserItem> GetFriendData() => new() {
            new UserItem { UserName = "张三", GroupName = "内部好友" },
            new UserItem { UserName = "李四", GroupName = "内部好友" },
            new UserItem { UserName = "王五", GroupName = "外部联系人" }
    };

    private List<UserItem> GetProjectData() => new(){
            new UserItem { UserName = "BIM项目A", GroupName = "自研" },
            new UserItem { UserName = "BIM项目B", GroupName = "自研" }
    };
}

