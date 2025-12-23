using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Syncdesign.Ui.Controls
{
    public class SearchBox : Control
    {
        private TextBox _textBox;
        private ListBox _listBox;
        private DispatcherTimer _debounceTimer;

        static SearchBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _listBox = GetTemplateChild("PART_ListBox") as ListBox;

            if (_textBox != null)
            {
                _textBox.TextChanged += OnTextChanged;
                _textBox.PreviewKeyDown += OnPreviewKeyDown;
            }

            if (_listBox != null)
            {
                _listBox.MouseLeftButtonUp += (_, _) =>
                {
                    if (_listBox.SelectedItem != null)
                    {  
                        // 执行外部命令
                        if (SelectedCommand?.CanExecute(_listBox.SelectedItem) == true)
                        {
                            SelectedCommand.Execute(_listBox.SelectedItem);
                            IsPopupOpen = false;
                        }
                    }
                };
            }

            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(Delay)
            };
            _debounceTimer.Tick += (_, _) =>
            {
                _debounceTimer.Stop();
                ExecuteSearchCommand();
            };
        }

        #region Text

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextPropertyChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (SearchBox)d;
            box.IsPopupOpen = !string.IsNullOrWhiteSpace(box.Text);
        }

        #endregion

        #region Placeholder

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata("搜索…"));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        #endregion

        #region Popup

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(
                nameof(IsPopupOpen),
                typeof(bool),
                typeof(SearchBox),
                new PropertyMetadata(false));

        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        #endregion

        #region Items

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(SearchBox));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(SearchBox));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        #endregion

        #region SearchCommand

        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(
                nameof(SearchCommand),
                typeof(ICommand),
                typeof(SearchBox));

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        #endregion

        #region SelectedCommand
        public static readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register(
                nameof(SelectedCommand),
                typeof(ICommand),
                typeof(SearchBox));

        public ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }
        #endregion

        #region Delay

        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register(
                nameof(Delay),
                typeof(int),
                typeof(SearchBox),
                new PropertyMetadata(300));

        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        #endregion

        #region Logic

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(Delay);
            _debounceTimer.Start();

            if (_listBox != null && _listBox.Items.Count > 0)
            {
                _listBox.SelectedIndex = 0; // 默认选中第一个
            }
        }

        private void ExecuteSearchCommand()
        {
            if (SearchCommand?.CanExecute(Text) == true)
            {
                SearchCommand.Execute(Text);
            }

            // 默认选中第一个
            if (_listBox != null && _listBox.Items.Count > 0)
            {
                _listBox.SelectedIndex = 0;
                IsPopupOpen = true; // 打开 Popup
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                IsPopupOpen = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                if (_listBox != null && _listBox.SelectedItem != null)
                {
                    // 执行选中命令
                    if (SelectedCommand?.CanExecute(_listBox.SelectedItem) == true)
                    {
                        SelectedCommand.Execute(_listBox.SelectedItem);
                    }

                    IsPopupOpen = false;
                    e.Handled = true;
                }
                else
                {
                    // 如果没有选中项，则执行搜索命令
                    ExecuteSearchCommand();
                    IsPopupOpen = false;
                    e.Handled = true;
                }
            }
            else if (_listBox != null)
            {
                if (e.Key == Key.Down)
                {
                    _listBox.SelectedIndex =
                        Math.Min(_listBox.SelectedIndex + 1, _listBox.Items.Count - 1);
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    _listBox.SelectedIndex =
                        Math.Max(_listBox.SelectedIndex - 1, 0);
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
