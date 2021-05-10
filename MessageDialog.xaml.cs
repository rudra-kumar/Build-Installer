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
using Build_Installer.ViewModels;

namespace Build_Installer
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageDialog));


        public MessageDialog()
        {
            InitializeComponent();
            var messageDialogViewModel = new MessageDialogViewModel();
            DataContext = messageDialogViewModel;
            //Binding viewModelMessage = new Binding();
            //viewModelMessage.Mode = BindingMode.TwoWay;
            //viewModelMessage.Source = messageDialogViewModel;
            //viewModelMessage.Path = new PropertyPath("Message");
            //BindingOperations.SetBinding(this, MessageProperty, viewModelMessage);
        }
    }
}
