using Civilization_draft.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Civilization_draft.ViewModels
{
    class ViewModel : NotifyPropertyChangedBase
    {
        // ViewModel Constructor
        public ViewModel()
        {            
            SubmitCommand = new RelayCommand(o => ClickSubmit(), o=> ClickSubmit_CanExecute());
            BackCommand = new RelayCommand(o => ClickBack());
            CivButtonList = CreateCivButtons(Civilization.GetCivilizationsList());
            CivsPerPlayer = new AmountSetting(8, 3, this);
            NumberOfPlayers = new AmountSetting(12, 1, this);
            RiseAndFallCheckBox = new DlcCheckBox(CivButtonList, "R&F");
            GatheringStormCheckBox = new DlcCheckBox(CivButtonList, "GS");
            VanillaCheckBox = new DlcCheckBox(CivButtonList, "");
            CalculateCivRatio();
            _currentView = 1;
        }

        // --- Fields and properties ---
        private int _currentView;
        public int CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; NotifyPropertyChanged(); }
        }
        public AmountSetting CivsPerPlayer { get; set; } 
        public AmountSetting NumberOfPlayers { get; set; } 
        public int MinimumCivs { get { return (CivsPerPlayer.Selected * NumberOfPlayers.Selected); } }
        public int NumberOfSelectedCivs { get { return CivButtonList.Count(a=>a.IsChecked); } }
        private bool _enoughCivs;
        public bool EnoughCivs { 
            get { return _enoughCivs; }
            set
            {
                if(value != _enoughCivs)
                {
                    _enoughCivs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DlcCheckBox RiseAndFallCheckBox { get; set; }
        public DlcCheckBox GatheringStormCheckBox { get; set; }
        public DlcCheckBox VanillaCheckBox { get; set; }
        public List<CivButton> CivButtonList { get; set; }
        public List<CivButton> SelectedCivs { get; set; }
        public List<PlayerResult> Players { get; set; }

        // --- Commands ---
        public ICommand SubmitCommand { get; private set; }        
        private void ClickSubmit()
        {
            SelectedCivs = new List<CivButton>();
            foreach (var civ in CivButtonList)
            {
                if(civ.IsChecked == true)
                {
                    SelectedCivs.Add(civ);
                }
            }
            if(SelectedCivs.Count() >= MinimumCivs)
            {
                AssignCivsToPlayers();
                CurrentView = 2;
            }
        }
        private bool ClickSubmit_CanExecute()
        {
            if(EnoughCivs == true) { return true; }
            else { return false; }
        }

        public ICommand BackCommand { get; private set; }
        private void ClickBack()
        {
            CurrentView = 1;
        }
        
        // --- Internal methods ---
        private List<CivButton> CreateCivButtons(List<Civilization> inputList)
        {
            List<CivButton> outputList = new List<CivButton>();
            foreach (var civ in inputList)
            {
                var civButton = new CivButton(civ, this);
                outputList.Add(civButton);
            }
            return outputList;
        }
        private void AssignCivsToPlayers()
        {
            Players = new List<PlayerResult>();
            var rnd = new Random();
            for(int i=0; i < NumberOfPlayers.Selected; i++)
            {
                var player = new PlayerResult();
                player.PlayerNumber = i + 1;
                for(int j=0; j<CivsPerPlayer.Selected; j++)
                {
                    int randomIndex = rnd.Next(SelectedCivs.Count());
                    var civ = SelectedCivs[randomIndex];
                    player.Civs.Add(civ);
                    SelectedCivs.RemoveAt(randomIndex);
                }
                Players.Add(player);
            }
        }
        private void CalculateCivRatio()
        {
            if(NumberOfSelectedCivs >= MinimumCivs)
            {
                EnoughCivs = true;
            }
            else if(NumberOfSelectedCivs < MinimumCivs)
            {
                EnoughCivs = false;
            }
        }
        
        // --- ViewModel classes ---

        // Have to pass the ViewModel in constructor to use NotifyPropertyChanged on ViewModel properties
        public class CivButton : NotifyPropertyChangedBase
        {
            // Civilization Civ is a property because CivButton cannot inherit from two classes
            public Civilization Civ { get; set; }
            private ViewModel viewModel;
            private bool _isChecked;
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        NotifyPropertyChanged();
                        viewModel.NotifyPropertyChanged("NumberOfSelectedCivs");
                        viewModel.CalculateCivRatio();
                    }
                }
            }
            // Constructor
            public CivButton(Civilization civ, ViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.Civ = civ;
                _isChecked = true;
            }
        }
        public class AmountSetting : NotifyPropertyChangedBase
        {
            // Constructor
            public AmountSetting(int amount, int selectedAmount, ViewModel viewModel)
            {
                this.viewModel = viewModel;
                _selected = selectedAmount;
                Amount = FillAmountArray(amount);
            }

            private ViewModel viewModel;
            public int[] Amount { get; set; }
            private int _selected;
            public int Selected {
                get { return _selected; }
                set
                {
                    if(_selected != value)
                    {
                        _selected = value;
                        NotifyPropertyChanged();
                        viewModel.NotifyPropertyChanged("MinimumCivs");
                        viewModel.CalculateCivRatio();
                    }
                }
            }
            private int[] FillAmountArray(int amount)
            {
                int[] arr = new int[amount];
                for(int i=0; i<amount; i++)
                {
                    arr[i] = i+1;
                }
                return arr;
            }
        }
        public class PlayerResult
        {
            public int PlayerNumber { get; set; }
            public List<CivButton> Civs { get; set; } = new List<CivButton>();
        }
        
        public class DlcCheckBox : NotifyPropertyChangedBase
        {
            // https://stackoverflow.com/questions/48955781/wpf-select-all-checkbox-in-a-datagrid/48989696

            // Constructor
            public DlcCheckBox(List<CivButton> civButtons, string filterString)
            {                
                AllCivButtons = civButtons;
                this.filterString = filterString;
                foreach(var civButton in AllCivButtons)
                {                    
                    if (civButton.Civ.Dlc == filterString)
                    {
                        // If a civButton's IsSelected is changed it needs to run RecheckAllSelected to update DlcCheckbox
                        //  RecheckAllSelected is run through DlcCheckboxPropertyChanged
                        civButton.PropertyChanged += DlcCheckboxPropertyChanged;
                    }
                }
                RecheckAllSelected();
            }

            private List<CivButton> AllCivButtons;
            private string filterString;
            private bool? _allSelected;
            public bool? AllSelected
            {
                get { return _allSelected; }
                set
                {
                    if(value != _allSelected)
                    {
                        _allSelected = value;
                        NotifyPropertyChanged();
                        AllSelectedChanged();
                    }
                }
            }

            private void AllSelectedChanged()
            {
                if(AllSelected == true)
                {
                    foreach(var item in AllCivButtons.Where(a=> a.Civ.Dlc == filterString))
                    {
                        item.IsChecked = true;
                    }
                }
                else if(AllSelected == false)
                {
                    foreach (var item in AllCivButtons.Where(a => a.Civ.Dlc == filterString))
                    {
                        item.IsChecked = false;
                    }
                }
            }
            private void RecheckAllSelected()
            {
                if (AllCivButtons.Where(a => a.Civ.Dlc == filterString).All(a => a.IsChecked))
                    AllSelected = true;
                else if (AllCivButtons.Where(a => a.Civ.Dlc == filterString).All(a => !a.IsChecked))
                    AllSelected = false;
                else
                    AllSelected = null;
            }
            private void DlcCheckboxPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                // Only re-check if the IsChecked property changed
                if (args.PropertyName == nameof(CivButton.IsChecked))
                    RecheckAllSelected();
            }
        }
    }
}
