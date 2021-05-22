using Build_Installer.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;

namespace Build_Installer
{
    /// <summary>
    /// Interaction logic for AndroidDeviceView.xaml
    /// </summary>
    public partial class AndroidDeviceView : UserControl
    {
        public AndroidDeviceView()
        {
            InitializeComponent();
        }

        private void UserControl_DragOver(object sender, DragEventArgs e)
        {
            //(DataContext as AndroidDeviceViewModel).InstallBuild(e.Data.GetData())
            bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop, false);
            if (!isFile)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop);
            if(isFile)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length != 0)
                {
                    // Get that data as string
                    string buildPath = files[0];
                    // Then we call the install build Command on android view model
                    var androidDeviceViewModel = (DataContext as AndroidDeviceViewModel);
                    androidDeviceViewModel.InstallBuild.Execute(buildPath);
                }
                
            }
        }

        
    }
}
