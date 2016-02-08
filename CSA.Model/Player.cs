using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    public class Player : BaseModel
    {
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(Name);
            }
        }
        private string _Name;

        private int _Frags;

        public int Frags
        {
            get { return _Frags; }
            set
            {
                _Frags = value;
                RaisePropertyChanged(Frags);
            }
        }

        private TimeSpan _Time;

        public TimeSpan Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                RaisePropertyChanged(Time);
            }
        }

        public override string ToString()
        {
            return string.Format("Name:{0}; Frags:{1}; Time:{2}", _Name, _Frags, _Time.ToString(@"hh\:mm\:ss"));
        }
    }
}
