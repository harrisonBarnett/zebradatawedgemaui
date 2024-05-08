using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZebraDataWedge.Controllers
{
    public class ScannerController : INotifyPropertyChanged
    {
        private static ScannerController _instance;
        public static ScannerController Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScannerController();
                }
                return _instance;
            }
        }

        private string _scannedData = "";

        public string ScannedData
        {
            get => _scannedData;
            set
            {
                _scannedData = value;
                OnPropertyChanged(nameof(ScannedData));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ScannerController() { }

    }
}
