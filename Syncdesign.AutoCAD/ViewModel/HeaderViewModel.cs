using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Syncdesign.AutoCAD.ViewModel
{
    public class HeaderViewModel : INotifyPropertyChanged
    {
        public HeaderViewModel()
        {
            Signature = "个性签名->HeaderViewModel";
            UserName = "用户";
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }


        private string _signature;
        public string Signature
        {
            get => _signature;
            set
            {
                _signature = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
