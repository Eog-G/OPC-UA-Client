using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_UA_Client.Core
{
    public class ObservableString : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private string _value;

        public string Value
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
