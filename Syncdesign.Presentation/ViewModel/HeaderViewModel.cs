using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Syncdesign.Presentation.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace Syncdesign.Presentation.ViewModel;
public partial class HeaderViewModel : ObservableObject
{
    public HeaderViewModel()
    {
        UserName ="用户1";
    }

    [ObservableProperty]
    private  string? userName;

    [ObservableProperty]
    private ObservableCollection<SearchItem> signatures = new();

    [ObservableProperty]
    private string? keyword;

    [RelayCommand]
    private void Search(string keyword)
    {
        Debug.WriteLine($"Search keyword: {keyword}");

        Signatures.Clear();

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        // 模拟搜索结果
        Signatures.Add(new SearchItem(keyword + "A", "A"));
        Signatures.Add(new SearchItem(keyword + "B", "B"));
        Signatures.Add(new SearchItem(keyword + "C", "C"));
        Signatures.Add(new SearchItem(keyword + "D", "D"));
    }

    [RelayCommand]
    private void Selected(SearchItem searchItem)
    {
        // 这里处理输入后的逻辑
        Debug.WriteLine($"搜索关键词: {searchItem.Name + " -- " + searchItem.Type}");

        // 发送消息
        WeakReferenceMessenger.Default.Send(new SearchItemMessage(searchItem)); 

        Keyword = string.Empty;
    } 
   
}
