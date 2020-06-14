using System.ComponentModel;

namespace Kuretru.QuickCropper.Entity
{
    class WorkProgress : INotifyPropertyChanged
    {

        private int step;
        private int total;

        public int Step
        {
            get => step;
            set
            {
                step = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Step"));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        public int Total
        {
            get => total;
            set
            {
                total = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Total"));
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        public string Text
        {
            get
            {
                if (Total == 0)
                {
                    return "";
                }
                else
                {
                    return Step + " / " + Total;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
