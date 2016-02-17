using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class Player : BaseViewModel
    {
        private byte _Index;
        /// <summary>
        /// Index of player chunk starting from 0. 
        /// </summary>
        public byte Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        
        private string _Name;
        /// <summary>
        /// Name of the player. 
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private int _Score;

        public int Score
        {
            get { return _Score; }
            set
            {
                _Score = value;
                RaisePropertyChanged(nameof(Score));
            }
        }

        private TimeSpan _Duration;

        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                _Duration = value;
                RaisePropertyChanged(nameof(Duration));
            }
        }

        public override string ToString()
        {
            //return string.Format("Name:{0}; Frags:{1}; Time:{2}", _Name, _Score, _Duration.ToString(@"hh\:mm\:ss"));
            return string.Format("Name:{0}; Frags:{1}; Time:{2}", _Name, _Score, _Duration);
        }
    }
}
