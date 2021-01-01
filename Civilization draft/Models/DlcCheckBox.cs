using Civilization_draft.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft.Models
{
    public class DlcCheckbox : NotifyPropertyChangedBase
    {
        // https://stackoverflow.com/questions/48955781/wpf-select-all-checkbox-in-a-datagrid/48989696

        public Dlc Dlc { get; set; }
        private List<CivButton> civButtons;
        private bool? _allSelected;
        public bool? AllSelected
        {
            get { return _allSelected; }
            set
            {
                if (value != _allSelected)
                {
                    _allSelected = value;
                    AllSelectedChanged();
                    NotifyPropertyChanged();
                }
            }
        }

        public DlcCheckbox(Dlc dlc, List<CivButton> connectedCivButtons)
        {
            civButtons = connectedCivButtons;
            Dlc = dlc;
            foreach (var civButton in civButtons)
            {
                civButton.OnIsCheckedChanged += RecheckAllSelected;
            }
            RecheckAllSelected();
        }

        #region Methods
        private void AllSelectedChanged()
        {
            if (AllSelected == true)
            {
                foreach (var item in civButtons)
                {
                    item.IsChecked = true;
                }
            }
            else if (AllSelected == false)
            {
                foreach (var item in civButtons)
                {
                    item.IsChecked = false;
                }
            }
        }
        private void RecheckAllSelected()
        {
            if (civButtons.All(a => a.IsChecked))
                AllSelected = true;
            else if (civButtons.All(a => !a.IsChecked))
                AllSelected = false;
            else
                AllSelected = null;
        } 
        #endregion
    }
}
