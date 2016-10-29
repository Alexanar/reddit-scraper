using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RedditScrapper
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Program program;
        public PropertyViewModel viewModel;


        public MainWindow()
        {
            InitializeComponent();
            program = new Program(this);
            viewModel = new PropertyViewModel();
            this.DataContext = viewModel;
            ShowWindow(Visibility.Visible, Visibility.Hidden);

        }

        private void subredditTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (program != null)
            {
                program.SubredditGet();
            }

        }

        private void directoryTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (program != null)
            {
                program.FolderTextDirectoryGet();
            }
        }

        private void button_StartScrapper(object sender, RoutedEventArgs e)
        {
            if (program != null)
            {

                ShowWindow(Visibility.Hidden, Visibility.Visible);
                Thread ScrapperThread = new Thread(program.RunScrapper);
                ScrapperThread.Start();

            }
        }

        private void pageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (program != null)
            {
                program.MaxPagesGet();
            }
        }

        private void folderDialog_Click(object sender, RoutedEventArgs e)
        {
            if (program != null)
            {
                program.FolderDirectoryGet();
            }
        }

        private void directoryTextbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (directoryTextbox.Text == "<save directory path>")
            {
                directoryTextbox.Text = "";
            }
        }

        private void subredditTextbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (subredditTextbox.Text == "<subreddit name>")
            {
                subredditTextbox.Text = "";
            }
        }

        public void ShowWindow(Visibility main, Visibility output)
        {
            this.DataContext = viewModel;
            viewModel.mainUIVisibility = main;
            viewModel.outputUIVisibility = output;
        }

    }
}
