using Civilization_draft.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Civilization_draft.Views
{
    public partial class DraftView : UserControl
    {

        public DraftView()
        {
            InitializeComponent();
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            ViewModel vm = (ViewModel)this.DataContext;
            try
            {
                vm.SaveConfigCommand.Execute(null);
                PopupSavedConfig.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Configuration could not be saved!\n\nError message: " + ex.Message, "Civilization Drafter", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
