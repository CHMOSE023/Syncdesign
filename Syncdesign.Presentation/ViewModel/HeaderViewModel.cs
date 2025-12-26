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
        Signature = "打造最好用的协同设计软件！";
    }

    [ObservableProperty]
    private  string? userName;

    [ObservableProperty]
    private string? signature;

    [ObservableProperty]
    private ObservableCollection<SearchItem> searchItems = new();

    [ObservableProperty]
    private string? keyword;

    [RelayCommand]
    private void Search(string keyword)
    {
        Debug.WriteLine($"Search keyword: {keyword}");

        SearchItems.Clear();

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        // 模拟搜索结果
        SearchItems.Add(new SearchItem(keyword + "A", "A"));
        SearchItems.Add(new SearchItem(keyword + "B", "B"));
        SearchItems.Add(new SearchItem(keyword + "C", "C"));
        SearchItems.Add(new SearchItem(keyword + "D", "D"));
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

    [RelayCommand]
    private void Add()
    {
        Debug.WriteLine($"Add:{DateTime.Now}");
    }

    [RelayCommand]
    private void Archive()
    {
        Debug.WriteLine($"Archive:{DateTime.Now}");
    }
    [RelayCommand]
    private void CloudDown()
    {
        Debug.WriteLine($"CloudDown:{DateTime.Now}");
    }
    [RelayCommand]
    private void CloudUp()
    {
        Debug.WriteLine($"CloudUp:{DateTime.Now}");
    }
    [RelayCommand]
    private void CloudSave()
    {
        Debug.WriteLine($"CloudSave:{DateTime.Now}");
    }

    [RelayCommand]
    private void AppsAdd()
    {
        Debug.WriteLine($"AppsAdd:{DateTime.Now}");
    }

    /// <summary>
    /// 个性签名属性发生变化
    /// </summary> 
    partial void OnSignatureChanged(string? oldValue, string? newValue)
    {
        Debug.WriteLine($"OnSignatureChanged: oldValue {oldValue} , newValue {newValue}");
    }
}
