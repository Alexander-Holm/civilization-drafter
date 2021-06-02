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
        public ViewModel(List<Civilization> civList, SortedList<string, Dlc> dlcSortedList, Config config)
        {
            // Create a dlc so that a checkbox can be made for basegame civs
            // Ignored if such Dlc already exists
            if (!dlcSortedList.ContainsKey(""))
            {
                Dlc noDlc = new Dlc { Abbreviation = "", Fullname = "Base game", HasCheckbox = true };
                dlcSortedList.Add(noDlc.Abbreviation, noDlc);
            }

            CivButtonList = CreateCivButtons(civList, dlcSortedList);

            // Always creating defaultConfig because some default values may be used even if a config is passed in. 
            Config defaultConfig = Config.GetDefaultConfig();
            if (config is null)
                config = defaultConfig;

            if (config.UncheckedCivs is object)
            {
                var uncheckedCivs = CivButtonList
                .Where(civButton => config.UncheckedCivs
                .Any(civ =>
                    civ.Civ == civButton.Civ.Name
                    &&
                    civ.Leader == civButton.Civ.Leader))
                .ToList();
                uncheckedCivs.ForEach(civButton => civButton.IsChecked = false);
            }

            AllowDuplicateCivs = config.AllowDuplicateCivs;
            AllowDuplicateLeaders = config.AllowDuplicateLeaders;

            int[] amountSettingValues;
            int preselectedValue;            

            amountSettingValues = Enumerable.Range(1, 10).ToArray();
            if (amountSettingValues.Contains(config.SelectedCivsPerPlayer))
                preselectedValue = config.SelectedCivsPerPlayer;
            else preselectedValue = defaultConfig.SelectedCivsPerPlayer;
            CivsPerPlayer = new AmountSetting(amountSettingValues, preselectedValue);
            CivsPerPlayer.OnSelectedChanged += NotifyCivRatioChanged;

            amountSettingValues = Enumerable.Range(1, 12).ToArray();
            if (amountSettingValues.Contains(config.SelectedNumberOfPlayers))
                preselectedValue = config.SelectedNumberOfPlayers;
            else preselectedValue = defaultConfig.SelectedNumberOfPlayers;
            NumberOfPlayers = new AmountSetting(amountSettingValues, preselectedValue);
            NumberOfPlayers.OnSelectedChanged += NotifyCivRatioChanged;

            // Not using canExecute, doing it manually for better control over xaml styling
            DraftCommand = new RelayCommand(o => Draft());
            BackCommand = new RelayCommand(o => CurrentView = 1);
            CopyResultAsTextCommand = new RelayCommand(o => DataAccess.ClipBoard.CopyResultAsText(Result));
            CopyUiElementAsImageCommand = new RelayCommand(param => DataAccess.ClipBoard.CopyUiElementAsImage(param as UIElement));
            SaveConfigCommand = new RelayCommand(o => SaveConfig());

            // Create checkboxes only for dlc where HasCheckbox == true
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
        public ICommand DraftCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand CopyResultAsTextCommand { get; private set; }
        public ICommand CopyUiElementAsImageCommand { get; private set; }
        public ICommand SaveConfigCommand { get; private set; }
        #endregion

        #region Command methods
        private void Draft()
        {
            if (EnoughCivs == false)
                return;

            var selectedCivs = CivButtonList.Where(civButton => civButton.IsChecked == true).ToList();

            Result = new List<PlayerResult>();
            var rnd = new Random();
            for (int i = 0; i < NumberOfPlayers.Selected; i++)
            {
                var playerResult = new PlayerResult();
                playerResult.PlayerNumber = i + 1;
                for (int j = 0; j < CivsPerPlayer.Selected; j++)
                {
                    int randomIndex = rnd.Next(selectedCivs.Count());
                    var receivedCiv = selectedCivs[randomIndex];
                    playerResult.Civs.Add(receivedCiv);
                    selectedCivs.RemoveAt(randomIndex);
                    if(AllowDuplicateCivs == false)
                        selectedCivs.RemoveAll(civ => civ.Civ.Name == receivedCiv.Civ.Name);
                    if(AllowDuplicateLeaders == false)
                        selectedCivs.RemoveAll(civ => civ.Civ.Leader == receivedCiv.Civ.Leader);
                }
                Result.Add(playerResult);
            }

            CurrentView = 2;
        }

        private void SaveConfig()
        {
            Config config = new Config
            {
                UncheckedCivs = CivButtonList.Where(civButton => civButton.IsChecked == false).Select(civbutton => new CivSimple
                {
                    Civ = civbutton.Civ.Name,
                    Leader = civbutton.Civ.Leader,
                }).ToList(),
                AllowDuplicateCivs = AllowDuplicateCivs,
                AllowDuplicateLeaders = AllowDuplicateLeaders,
                SelectedCivsPerPlayer = CivsPerPlayer.Selected,
                SelectedNumberOfPlayers = NumberOfPlayers.Selected
            };
            DataAccess.Json.SaveConfig(config);
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
                // AppDomain.CurrentDomain.BaseDirectory does not end with / when run from testproject
                Uri imageUri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/" + civ.Image);
                BitmapImage image;
                try { image = new BitmapImage(imageUri); }
                catch (FileNotFoundException) { image = null; }
                catch (DirectoryNotFoundException) { image = null; }

                var civButton = new CivButton(civ, dlc);
                civButton.BitmapImage = image;
                civButton.OnIsCheckedChanged += NotifyCivRatioChanged;
                outputList.Add(civButton);
            }
            return outputList;
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
