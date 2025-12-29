namespace Syncdesign.Presentation.Model;

public class ContactItem
{
    public string? GroupName { get; set; } // "用户" 或 "项目"
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Icon { get; set; } // 对应 SymbolIcon 的枚举
}
