using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Civilization_draft.Views.Controls
{
    /// <summary>
    /// Interaction logic for SuccessPopup.xaml
    /// </summary>
    public partial class SuccessPopup : UserControl
    {
        public SuccessPopup()
        {
            InitializeComponent();
            // https://blog.jerrynixon.com/2013/07/solved-two-way-binding-inside-user.html
            // ^^ Why this is needed
            (this.Content as FrameworkElement).DataContext = this;
        }

        public int Duration { get; set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SuccessPopup), new PropertyMetadata("SuccessPopup"));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set 
            { 
                SetValue(IsOpenProperty, value);
                if (value == false)
                    return;

                var timer = new Timer();
                timer.Interval = Duration;
                timer.Elapsed += (Object source, ElapsedEventArgs elapsedEventArgs) =>
                {
                    Dispatcher.Invoke(() => IsOpen = false);
                    timer.Stop();
                };
                timer.Start();
            }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SuccessPopup), new PropertyMetadata(false));        

    }
}
