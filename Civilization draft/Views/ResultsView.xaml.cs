using Civilization_draft.Models;
using Civilization_draft.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
        }

        private void Click_CopyAsText(object sender, RoutedEventArgs e)
        {
            ViewModel vm = (ViewModel)this.DataContext;
            try
            {
                vm.CopyResultAsTextCommand.Execute(null);
                PopupCopyText.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy text to clipboard!\n\nError message: " + ex.Message, "Civilization Drafter", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Click_CopyAsImage(object sender, RoutedEventArgs e)
        {
            ViewModel vm = (ViewModel)this.DataContext;
            try
            {
                vm.CopyUiElementAsImageCommand.Execute(ResultContainer);
                PopupCopyImage.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy image to clipboard!\n\nError message: " + ex.Message, "Civilization Drafter", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
