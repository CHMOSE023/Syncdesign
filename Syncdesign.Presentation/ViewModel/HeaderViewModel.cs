using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
 

namespace Syncdesign.Presentation.ViewModel;
 
public class HeaderViewModel : INotifyPropertyChanged
{
    public class SearchItem
    {
        public SearchItem(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; set; }
        public string Type { get; set; }
    }


    public HeaderViewModel()
    {
        Suggestions = new ObservableCollection<SearchItem>();
        SearchCommand = new RelayCommand<string>(OnSearch);
        Signature = "个性签名->HeaderViewModel";
        UserName = "用户";
        SelectedCommand = new RelayCommand<SearchItem>(searchItem =>
        {
            Search=string.Empty;
            // 这里处理输入后的逻辑
            Debug.WriteLine($"搜索关键词: {searchItem.Name+" -- "+searchItem.Type}");
        });
    }

    private string _userName=string.Empty;
    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
        }
    }


    private string _signature = string.Empty;
    public string Signature
    {
        get => _signature;
        set
        {
            _signature = value;
            OnPropertyChanged();
        }
    }


    private string _search = string.Empty;
    public string Search
    {
        get => _search;
        set
        {
            _search = value;
            OnPropertyChanged();           
        }
    }
     
    private ObservableCollection<SearchItem> _suggestions = new();
    public  ObservableCollection<SearchItem> Suggestions
    {
        get => _suggestions;
        set
        {
            _suggestions = value;
            OnPropertyChanged(); 
        }
    }

    // 选中备选内容
    public ICommand SelectedCommand { get; }
    // 动态调整搜索内容
    public ICommand SearchCommand { get; }

  
    private void OnSearch(string keyword)
    {
        Debug.WriteLine($"Search keyword: {keyword}");

        Suggestions.Clear();

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        // 模拟搜索结果
        Suggestions.Add(new SearchItem(keyword+ "A", "A"));
        Suggestions.Add(new SearchItem(keyword+ "B", "B"));
        Suggestions.Add(new SearchItem(keyword+ "C", "C")); 
        Suggestions.Add(new SearchItem(keyword+ "D", "D"));  
    } 

    public event PropertyChangedEventHandler ?PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
      => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
