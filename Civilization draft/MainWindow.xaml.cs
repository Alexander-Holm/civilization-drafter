using Civilization_draft.Models;
using Civilization_draft.Models.JsonModels;
using Civilization_draft.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Civilization_draft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var civList = DataAccess.Json.LoadCivilizations();
                var dlcSortedList = DataAccess.Json.LoadDlc();
                var config = DataAccess.Json.LoadConfig(); // Will be null if config is not found
                var vm = new ViewModel(civList, dlcSortedList, config);
                this.DataContext = vm;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Could not find Civilizations.json or Dlc.json in folder CivData", "Civilization drafter", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Civilization drafter", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }        
}
  
