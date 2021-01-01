using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft.Models
{
    public class AmountSetting : NotifyPropertyChangedBase
    {     
        public int[] Values { get; set; }
        public Action OnSelectedChanged;
        private int _selected;
        public int Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnSelectedChanged?.Invoke();
                    NotifyPropertyChanged();
                }
            }
        }

        public AmountSetting(int[] values, int preselectedAmount)
        {
            _selected = preselectedAmount;
            Values = values;
        } 
    }
}
