﻿using Civilization_draft.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
            
            CivsPerPlayer = new AmountSetting(8, 3, this);
            NumberOfPlayers = new AmountSetting(12, 1, this);

            var dlcList = DataAccess.LoadDlc();
            CivButtonList = CreateCivButtons(DataAccess.LoadCivilizations(), dlcList);
            Dlc noDlc = new Dlc { Fullname = "Vanilla", Abbreviation = "" };
            DlcCheckboxes = new List<DlcCheckbox> { new DlcCheckbox(CivButtonList, noDlc) };
            foreach (var dlc in dlcList.Values)
            {
                if (dlc.Checkbox == true)
                    DlcCheckboxes.Add(new DlcCheckbox(CivButtonList, dlc));
            }

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
        public List<DlcCheckbox> DlcCheckboxes { get; set; }
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
        public DlcCheckbox RiseAndFallCheckBox { get; set; }
        public DlcCheckbox GatheringStormCheckBox { get; set; }
        public DlcCheckbox VanillaCheckBox { get; set; }
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
            if(EnoughCivs == true)
                return true;
            else 
                return false;
        }

        public ICommand BackCommand { get; private set; }
        private void ClickBack()
        {
            CurrentView = 1;
        }
        
        // --- Internal methods ---
        private List<CivButton> CreateCivButtons(List<Civilization> civList, SortedList<string, Dlc> dlcList)
        {
            List<CivButton> outputList = new List<CivButton>();
            foreach (var civ in civList)
            {
                Dlc dlc;
                dlcList.TryGetValue(civ.Dlc, out dlc);
                var civButton = new CivButton(civ, dlc, this); // this == viewmodel
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
            public Dlc Dlc { get; set; }
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
            public CivButton(Civilization civ, Dlc dlc, ViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.Civ = civ;
                this.Dlc = dlc;
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

            #region Properties
            private ViewModel viewModel;
            public int[] Amount { get; set; }
            private int _selected;
            public int Selected
            {
                get { return _selected; }
                set
                {
                    if (_selected != value)
                    {
                        _selected = value;
                        NotifyPropertyChanged();
                        viewModel.NotifyPropertyChanged("MinimumCivs");
                        viewModel.CalculateCivRatio();
                    }
                }
            } 
            #endregion

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
        
        public class DlcCheckbox : NotifyPropertyChangedBase
        {
            // https://stackoverflow.com/questions/48955781/wpf-select-all-checkbox-in-a-datagrid/48989696

            // Constructor
            public DlcCheckbox(List<CivButton> civButtons, Dlc dlc)
            {                
                AllCivButtons = civButtons;
                Dlc = dlc;
                foreach(var civButton in AllCivButtons)
                {                    
                    if (civButton.Civ.Dlc == Dlc.Abbreviation)
                    {
                        // If a civButton's IsSelected is changed it needs to run RecheckAllSelected to update DlcCheckbox
                        //  RecheckAllSelected is run through DlcCheckboxPropertyChanged
                        civButton.PropertyChanged += DlcCheckboxPropertyChanged;
                    }
                }
                RecheckAllSelected();
            }

            public Dlc Dlc { get; set; }
            private List<CivButton> AllCivButtons;
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
                    foreach(var item in AllCivButtons.Where(a=> a.Civ.Dlc == Dlc.Abbreviation))
                    {
                        item.IsChecked = true;
                    }
                }
                else if(AllSelected == false)
                {
                    foreach (var item in AllCivButtons.Where(a => a.Civ.Dlc == Dlc.Abbreviation))
                    {
                        item.IsChecked = false;
                    }
                }
            }
            private void RecheckAllSelected()
            {
                if (AllCivButtons.Where(a => a.Civ.Dlc == Dlc.Abbreviation).All(a => a.IsChecked))
                    AllSelected = true;
                else if (AllCivButtons.Where(a => a.Civ.Dlc == Dlc.Abbreviation).All(a => !a.IsChecked))
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
