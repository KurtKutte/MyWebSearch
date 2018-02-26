using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;


namespace MyWebSearch
{
    /// <summary>
    /// Interaktionslogik für WindowConfig.xaml
    /// </summary>
    public partial class WindowConfig : Window
    {
        public WindowConfig()
        {
            InitializeComponent();
            IniLayout();
            TextBoxPath.Text = Properties.Settings.Default.MyPath;
            CheckBoxStepEnable.IsChecked = Properties.Settings.Default.StepEnable;

        }

        private void IniLayout()
        {
            WindowConf.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 3.0;
            WindowConf.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 3.0;
        }

        private void ButtonFolderBrowserDiaglog_Click(object sender, RoutedEventArgs e)
        {
            string path0 = "";

            if (Directory.Exists(Properties.Settings.Default.MyPath))
                path0 = Properties.Settings.Default.MyPath;
            else
                path0 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            TextBoxPath.Text = path0;

            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = path0;

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                Properties.Settings.Default.MyPath = folderDialog.SelectedPath;
                Properties.Settings.Default.Save();
                TextBoxPath.Text = Properties.Settings.Default.MyPath;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            WindowConf.Close();
        }

        private void CheckBoxStepEnable_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxStepEnable.IsChecked==true)
                Properties.Settings.Default.StepEnable = true;
            else
                Properties.Settings.Default.StepEnable = false;

            Properties.Settings.Default.Save();
        }
    }
}
