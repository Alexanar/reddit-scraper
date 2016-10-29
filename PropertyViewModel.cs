using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedditScrapper
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        //visiblity bindings
        private Visibility m_mainUIVisibility;
        public Visibility mainUIVisibility
        {
            get { return m_mainUIVisibility; }
            set
            {
                m_mainUIVisibility = value;
                OnPropertyChanged("mainUIVisibility");
            }
        }
        private Visibility m_outputUIVisibility;
        public Visibility outputUIVisibility
        {
            get { return m_outputUIVisibility; }
            set
            {
                m_outputUIVisibility = value;
                OnPropertyChanged("outputUIVisibility");
            }
        }

        //property changed event
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
