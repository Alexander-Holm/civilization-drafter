using Civilization_draft.Models;
using Civilization_draft.Models.JsonModels;
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
    public class ViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        public ViewModel(List<Civilization> civList, SortedList<string, Dlc> dlcSortedList)
        {
            Dlc noDlc = new Dlc { Fullname = "Base game", Abbreviation = "", HasCheckbox = true };
            dlcSortedList.Add(noDlc.Abbreviation, noDlc);

            SubmitCommand = new RelayCommand(o => ClickSubmit(), o => ClickSubmit_CanExecute());
            BackCommand = new RelayCommand(o => ClickBack());

            Action onAmountSelectedChanged = CalculateCivRatio;
            onAmountSelectedChanged += () => NotifyPropertyChanged(nameof(MinimumCivs));
            CivsPerPlayer = new AmountSetting(8, 3);
            CivsPerPlayer.OnSelectedChanged += onAmountSelectedChanged;
            NumberOfPlayers = new AmountSetting(12, 1);
            NumberOfPlayers.OnSelectedChanged += onAmountSelectedChanged;

            dl(civList); // TEMP

            CivButtonList = CreateCivButtons(civList, dlcSortedList);

            DlcCheckboxes = new List<DlcCheckbox>();
            foreach (var dlc in dlcSortedList.Values)
            {
                if (dlc.HasCheckbox == true)
                {
                    var civButtonsOfDlc = CivButtonList.Where(cb => cb.Dlc.Abbreviation == dlc.Abbreviation).ToList();
                    DlcCheckboxes.Add(new DlcCheckbox(dlc, civButtonsOfDlc));
                }
            }

            CalculateCivRatio();

            _currentView = 1;
        } 
        #endregion
        // TEMP
        Dictionary<string, int> duplicateLeaders = new Dictionary<string, int>();
        void dl(List<Civilization> list)
        {
            duplicateLeaders = list
                .GroupBy(c => c.Name)
                .Where(g => g.Count() > 1)
                .ToDictionary(x => x.Key, x => x.Count());
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
                
        #region Methods
        private List<CivButton> CreateCivButtons(List<Civilization> civList, SortedList<string, Dlc> dlcList)
        {
            List<CivButton> outputList = new List<CivButton>();
            Action onIsCheckedChanged = CalculateCivRatio;
            onIsCheckedChanged = () => NotifyPropertyChanged(nameof(NumberOfSelectedCivs));
            foreach (var civ in civList)
            {
                Dlc dlc;
                if (dlcList.TryGetValue(civ.Dlc, out dlc))
                {
                    var civButton = new CivButton(civ, dlc);
                    civButton.OnIsCheckedChanged += onIsCheckedChanged;
                    outputList.Add(civButton);
                }
            }
            return outputList;
        }
        private void AssignCivsToPlayers()
        {
            Players = new List<PlayerResult>();
            var rnd = new Random();
            for (int i = 0; i < NumberOfPlayers.Selected; i++)
            {
                var player = new PlayerResult();
                player.PlayerNumber = i + 1;
                for (int j = 0; j < CivsPerPlayer.Selected; j++)
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
            if (NumberOfSelectedCivs >= MinimumCivs)
            {
                EnoughCivs = true;
            }
            else if (NumberOfSelectedCivs < MinimumCivs)
            {
                EnoughCivs = false;
            }
        } 
        #endregion

        public class PlayerResult
        {
            public int PlayerNumber { get; set; }
            public List<CivButton> Civs { get; set; } = new List<CivButton>();
        }
    }
}
