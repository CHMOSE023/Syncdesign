using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Syncdesign.Presentation.Model;
using Syncdesign.Ui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Syncdesign.Presentation.ViewModel;

public partial class SidebarViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<SidebarItem> topItems = [];

    [ObservableProperty]
    private ObservableCollection<SidebarItem> bottomItems = [];

    public SidebarViewModel()
    {
        TopItems = new ObservableCollection<SidebarItem>
        {
            CreateItem(NavType.Messages,"消息",SymbolFilled.Chat32),
            CreateItem(NavType.Contacts,"联系人",SymbolFilled.Person48,true),
            CreateItem(NavType.Projects,"项目",SymbolFilled.Briefcase48),
            CreateItem(NavType.Tasks, "任务",SymbolFilled.Checkmark48),
            CreateItem(NavType.Reviews, "审查",SymbolFilled.Eye48),
            CreateItem(NavType.Workflow, "流程",SymbolFilled.Document48),
        };

        BottomItems = new ObservableCollection<SidebarItem>
        {
            CreateItem(NavType.Settings,"设置",SymbolFilled.Settings48)
        };
    }

    private SidebarItem CreateItem(NavType navType, string title, SymbolFilled symbolFilled, bool isSelected=false)
    {
        var item = new SidebarItem
        {
            IsSelected = isSelected,
            NavType = navType,
            Title = title,
            Symbol = symbolFilled.ToString()
        }; 
        // 关键：必须手动把父级的 SelectItemCommand 赋值给子项 
        item.ClickCommand = SelectItemCommand;

        return item;
    }

    [RelayCommand]
    private void SelectItem(SidebarItem? targetItem)  
    {
        if (targetItem == null) return;

        if (targetItem.IsSelected) return;

        // --- 发送消息 ---
        WeakReferenceMessenger.Default.Send(new SidebarItemMessage(targetItem));
        // ----------------

        Debug.WriteLine(targetItem.Title);
         
        var allItems = TopItems.Concat(BottomItems);
        foreach (var item in allItems)
        {
            item.IsSelected = false;
        }
        
        targetItem.IsSelected = true;
    }

}
