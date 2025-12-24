using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Syncdesign.Presentation.Model;

public class SearchItemMessage(SearchItem value) : ValueChangedMessage<SearchItem>(value)
{
}