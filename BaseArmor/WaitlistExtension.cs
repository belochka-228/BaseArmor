using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseArmor
{
    public partial class Waitlist : INotifyPropertyChanged
    {
        private bool _canBook;

        public bool CanBook
        {
            get => _canBook;
            set
            {
                if (_canBook != value)
                {
                    _canBook = value;
                    OnPropertyChanged(nameof(CanBook));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
