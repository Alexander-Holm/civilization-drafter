using Civilization_draft.Models;
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
            var civList = DataAccess.LoadCivilizations();
            var dlcSortedList = DataAccess.LoadDlc();
            var vm = new ViewModel(civList, dlcSortedList);
            this.DataContext = vm;
        }
    }        
}
  
