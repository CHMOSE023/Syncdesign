using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Syncdesign.AutoCAD.ViewModel
{
    public class SidebarItem : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Symbol { get; set; }
        public ICommand Command { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
} 