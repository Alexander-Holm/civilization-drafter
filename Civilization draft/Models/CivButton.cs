using Civilization_draft.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Civilization_draft.Models
{
    public class CivButton : NotifyPropertyChangedBase
    {
        public Civilization Civ { get; set; }
        public Dlc Dlc { get; set; }
        public Action OnIsCheckedChanged;
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnIsCheckedChanged?.Invoke();
                    NotifyPropertyChanged();
                }
            }
        }
        public BitmapImage BitmapImage { get; set; }

        public CivButton(Civilization civ, Dlc dlc)
        {
            this.Civ = civ;
            this.Dlc = dlc;
            _isChecked = true;
        }
    }
}
