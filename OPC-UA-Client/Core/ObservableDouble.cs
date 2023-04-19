using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_UA_Client.Core
{
    internal class ObservableDouble : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private double? _value;

        public double? Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;

                _value = value;
                NotifyPropertyChanged("Value");
            }
        }
    }
}
