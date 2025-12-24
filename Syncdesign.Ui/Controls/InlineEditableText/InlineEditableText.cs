using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Syncdesign.Ui.Controls;

public class InlineEditableText : Control
{
    private TextBox? _editor;
    private TextBlock? _display;
    private string _backupText = string.Empty;

    static InlineEditableText()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(InlineEditableText),
            new FrameworkPropertyMetadata(typeof(InlineEditableText)));
    }

    #region Text

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(InlineEditableText),
            new FrameworkPropertyMetadata(string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    #endregion

    #region Placeholder

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(InlineEditableText),
            new PropertyMetadata(string.Empty));

    #endregion

    #region IsEditing

    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register(
            nameof(IsEditing),
            typeof(bool),
            typeof(InlineEditableText),
            new PropertyMetadata(false, OnIsEditingChanged));

    private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (InlineEditableText)d;

        if ((bool)e.NewValue)
        {
            control._backupText = control.Text;

            control.Dispatcher.BeginInvoke(() =>
            {
                control._editor?.Focus();
                control._editor?.SelectAll();
            }, DispatcherPriority.Input);
        }
    }

    #endregion

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Unhook old
        if (_display != null)
            _display.MouseLeftButtonDown -= OnDisplayClick;

        if (_editor != null)
        {
            _editor.LostKeyboardFocus -= OnEditorLostFocus;
            _editor.KeyDown -= OnEditorKeyDown;
        }

        // Get new
        _display = GetTemplateChild("PART_Display") as TextBlock;
        _editor = GetTemplateChild("PART_Editor") as TextBox;

        if (_display != null)
            _display.MouseLeftButtonDown += OnDisplayClick;

        if (_editor != null)
        {
            _editor.LostKeyboardFocus += OnEditorLostFocus;
            _editor.KeyDown += OnEditorKeyDown;

        }
    }



    private void OnDisplayClick(object sender, MouseButtonEventArgs e)
    {
        IsEditing = true;
    }

    private void OnEditorLostFocus(object sender, RoutedEventArgs e)
    {
        if (!IsEditing)
            return;

        CommitEdit();
        IsEditing = false;
    }

    private void OnEditorKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommitEdit();
            IsEditing = false;
            Keyboard.ClearFocus();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            CancelEdit();
            IsEditing = false;
            Keyboard.ClearFocus();
            e.Handled = true;
        }
    }

    private void CommitEdit()
    {
        IsEditing = false;
    }

    private void CancelEdit()
    {
        Text = _backupText;
        IsEditing = false;
    }
}
