using Syncdesign.Ui.Controls;
using System.Collections.ObjectModel; 

namespace Syncdesign.Presentation.ViewModel;

public class SidebarViewModel
{
    public ObservableCollection<SidebarItem> TopItems { get; }
    public ObservableCollection<SidebarItem> BottomItems { get; }
     

    public SidebarViewModel()
    {
        TopItems = new ObservableCollection<SidebarItem>
        {
            CreateItem("消息",SymbolFilled.Chat32),
            CreateItem("联系人",SymbolFilled.Person48),
            CreateItem("项目",SymbolFilled.Briefcase48),
            CreateItem("任务",SymbolFilled.Checkmark48),
            CreateItem("审查",SymbolFilled.Eye48),
            CreateItem("流程",SymbolFilled.Document48),
        };

        BottomItems = new ObservableCollection<SidebarItem>
        {
            CreateItem("设置",SymbolFilled.Settings48)
        };
    }

    private SidebarItem CreateItem(string title, SymbolFilled symbolFilled)
    {
        return new SidebarItem
        {
            Title = title,
            Symbol = symbolFilled.ToString(),
            Command = new RelayCommand<string>(Select)
        };
    }

    private void Select(string title)
    {
        var selected = TopItems.Concat(BottomItems).FirstOrDefault(x => x.Title == title);
        if (selected.IsSelected)
            return;
          
        foreach (var item in TopItems.Concat(BottomItems))
        {
            item.IsSelected = false;
        }
         
        if (selected != null)
        {
             selected.IsSelected = true;
        }
          
    }
}
