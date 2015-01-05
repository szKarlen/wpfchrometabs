using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace ChromiumTabsRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.newTabNumber = 1;
        }

        private void HandleAddTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), false);
        }

        private void HandleAddTabAndSelect(object sender, RoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), true);
        }

        private object GenerateNewItem()
        {
            object itemToAdd = new WebBrowser() { Source = new Uri("http://google.com") };
            Interlocked.Increment(ref this.newTabNumber);
            //if(this.title.Text.Length > 0)
            {
                itemToAdd = new ChromeTabs.ChromeTabItem
                {
                    Header = "Web Browser",
                    Content = itemToAdd
                };
            }
            return itemToAdd;
        }

        private void HandleRemoveTab(object sender, RoutedEventArgs e)
        {
            this.chrometabs.RemoveTab(this.chrometabs.SelectedItem);
        }

        private int newTabNumber;

        private void NewTabCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTabCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.chrometabs.AddTab(this.GenerateNewItem(), false);
            this.chrometabs.SelectedIndex = this.chrometabs.Items.Count - 1;
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            layout.Margin = this.WindowState == WindowState.Maximized 
                ? new Thickness(7.0, 6.0, 7.0, 7.0) 
                : new Thickness(7.0, 16.0, 7.0, 7.0);
        }
    }
}
