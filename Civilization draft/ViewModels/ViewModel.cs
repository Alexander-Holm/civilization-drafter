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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Civilization_draft.ViewModels
{
    public class ViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        // civList and dlcSortedList as parameters for testing without reading Json
        public ViewModel(List<Civilization> civList, SortedList<string, Dlc> dlcSortedList)
        {
            // Create a dlc so that a checkbox can be made for basegame civs
            // Ignored if such Dlc already exists
            if (!dlcSortedList.ContainsKey(""))
            {
                Dlc noDlc = new Dlc { Abbreviation = "", Fullname = "Base game", HasCheckbox = true };
                dlcSortedList.Add(noDlc.Abbreviation, noDlc);
            }

            CivButtonList = CreateCivButtons(civList, dlcSortedList);

            AllowDuplicateCivs = false;
            AllowDuplicateLeaders = false;

            SubmitCommand = new RelayCommand(o => ClickSubmit(), o => EnoughCivs);
            BackCommand = new RelayCommand(o => CurrentView = 1);
            CopyResultAsTextCommand = new RelayCommand(o => DataAccess.ClipBoard.CopyResultAsText(Result));
            CopyUiElementAsImageCommand = new RelayCommand(param => DataAccess.ClipBoard.CopyUiElementAsImage(param as UIElement));

            CivsPerPlayer = new AmountSetting(Enumerable.Range(1, 10).ToArray(), 3);
            NumberOfPlayers = new AmountSetting(Enumerable.Range(1, 12).ToArray(), 1);
            CivsPerPlayer.OnSelectedChanged += NotifyCivRatioChanged;
            NumberOfPlayers.OnSelectedChanged += NotifyCivRatioChanged;

            // Create checkboxes for dlc where HasCheckbox == true
            DlcCheckboxes = new List<DlcCheckbox>();
            foreach (var dlc in dlcSortedList.Values)
            {
                if (dlc.HasCheckbox == true)
                {
                    // DlcCheckbox needs references to all the CivButtons with the matching Dlc
                    var civButtonsOfDlc = CivButtonList.Where(cb => cb.Dlc.Abbreviation == dlc.Abbreviation).ToList();
                    DlcCheckboxes.Add(new DlcCheckbox(dlc, civButtonsOfDlc));
                }
            }

            _currentView = 1;
        }
        #endregion

        #region Fields and properties
        public List<CivButton> CivButtonList { get; set; }
        public List<CivButton> SelectedCivs { get; set; }
        public List<PlayerResult> Result { get; set; }
        public List<DlcCheckbox> DlcCheckboxes { get; set; }
        public AmountSetting CivsPerPlayer { get; set; }
        public AmountSetting NumberOfPlayers { get; set; }
        private int _currentView;
        public int CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; NotifyPropertyChanged(); }
        }        
        public int MinimumCivs { get { return (CivsPerPlayer.Selected * NumberOfPlayers.Selected); } }
        public int SelectedCivsCount
        {
            get
            {
                var selectedCivs = CivButtonList.Where(a => a.IsChecked).ToList();
                if (AllowDuplicateCivs == false)
                {
                    selectedCivs = selectedCivs
                        .GroupBy(c => c.Civ.Name)
                        .Select(g => g.First()).ToList();
                }
                if (AllowDuplicateLeaders == false)
                {
                    selectedCivs = selectedCivs
                        .GroupBy(c => c.Civ.Leader)
                        .Select(g => g.First()).ToList();
                }

                return selectedCivs.Count;
            }
        }
        private bool _allowDuplicateCivs;
        public bool AllowDuplicateCivs
        {
            get { return _allowDuplicateCivs; }
            set
            {
                if (_allowDuplicateCivs != value)
                {
                    _allowDuplicateCivs = value;
                    NotifyCivRatioChanged();
                }
            }
        }
        private bool _allowDuplicateLeaders;
        public bool AllowDuplicateLeaders
        {
            get { return _allowDuplicateLeaders; }
            set
            {
                if (_allowDuplicateLeaders != value)
                {
                    _allowDuplicateLeaders = value;
                    NotifyCivRatioChanged();
                }
            }
        }
        public bool EnoughCivs
        {
            get
            {
                if (SelectedCivsCount < MinimumCivs)
                    return false;
                return true;
            }
        }        
        public ICommand SubmitCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand CopyResultAsTextCommand { get; private set; }
        public ICommand CopyUiElementAsImageCommand { get; private set; }
        #endregion

        #region Command methods
        private void ClickSubmit()
        {
            SelectedCivs = new List<CivButton>();
            foreach (var civ in CivButtonList)
            {
                if (civ.IsChecked == true)
                {
                    SelectedCivs.Add(civ);
                }
            }
            AssignCivsToPlayers();
            CurrentView = 2;
        }
        #endregion

        #region Methods
        private List<CivButton> CreateCivButtons(List<Civilization> civList, SortedList<string, Dlc> dlcList)
        {
            List<CivButton> outputList = new List<CivButton>();
            foreach (var civ in civList)
            {                       
                Dlc dlc;
                // string is a reference type and can be null
                // If null set civ.Dlc to ""
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
                if (!dlcList.TryGetValue(civ.Dlc ?? "", out dlc))
                    dlc = new Dlc { Abbreviation = civ.Dlc };

                // /Images/ instead of Images/
                // AppDomain.CurrentDomain.BaseDirectory does not end with / in testproject
                Uri imageUri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/" + civ.Image);
                BitmapImage image;
                try { image = new BitmapImage(imageUri); }
                catch (FileNotFoundException){ image = null; }
                catch (DirectoryNotFoundException) { image = null; }

                var civButton = new CivButton(civ, dlc);
                civButton.BitmapImage = image;
                civButton.OnIsCheckedChanged += NotifyCivRatioChanged;
                outputList.Add(civButton);
            }
            return outputList;
        }
        private void AssignCivsToPlayers()
        {
            Result = new List<PlayerResult>();
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
                Result.Add(player);
            }
        }        

        private void NotifyCivRatioChanged()
        {
            NotifyPropertyChanged(nameof(SelectedCivsCount));
            NotifyPropertyChanged(nameof(EnoughCivs));
            NotifyPropertyChanged(nameof(MinimumCivs));
        }
        #endregion
    }
}
