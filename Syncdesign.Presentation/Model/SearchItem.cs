namespace Syncdesign.Presentation.Model;

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
