using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Syncdesign.Presentation.Model;

public enum NavType
{
    /// <summary>
    /// 消息
    /// </summary>
    Messages,
    /// <summary>
    /// 联系人
    /// </summary>
    Contacts,
    /// <summary>
    /// 项目
    /// </summary>
    Projects,
    /// <summary>
    /// 任务
    /// </summary>
    Tasks,
    /// <summary>
    /// 审查
    /// </summary>
    Reviews,
    /// <summary>
    /// 流程
    /// </summary>
    Workflow,
    /// <summary>
    /// 设置
    /// </summary>
    Settings
}

public partial class SidebarItem : ObservableObject
{
    public NavType NavType { get; set; }
    public string ?Title { set; get; }

    public string? Symbol { set; get; }

    [ObservableProperty]
    private bool isSelected;

    public IRelayCommand? ClickCommand { get; set; }
}
