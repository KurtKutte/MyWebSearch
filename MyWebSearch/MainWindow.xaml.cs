using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWebSearch
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Doc> ListDoc = new List<Doc>();
        List<File> ListFile = new List<File>();
        string MyPath = "";


        public MainWindow()
        {
            InitializeComponent();

            ReadFiles();
            ListFileToView();

            ReadDocs();
            ListDocToView();
        }

        private void ReadFiles()
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(docPath + "\\MyWebSearch"))
            {
                MessageBoxResult result = MessageBox.Show("Ordner 'MyWebSearch' im Dokumenten-Verzeichnis existiert nicht\nEinrichten ?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(docPath + "\\MyWebSearch");
                    MyPath = docPath + "\\MyWebSearch";
                }


            }
            else
            {
                MyPath = docPath + "\\MyWebSearch";
            }




            string[] files = System.IO.Directory.GetFiles(MyPath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                foreach (var item in files)
                {
                    string name = item.Substring(item.LastIndexOf('\\') + 1);
                    File f = new File();
                    f.FileName = name;
                    ListFile.Add(f);
                }
            }
            else
            {

                System.IO.File.WriteAllText(MyPath + "\\" + "MyWebSearch.txt", "wikipedia.de" + "\t" + "true");

                File f = new File();
                f.FileName = "MyWebSearch.txt";
                ListFile.Add(f);
                files = new string[1];
                files[0] = "MyWebSearch.txt";
       
     

            }

            if (Properties.Settings.Default.FileIndexNow == -1 | Properties.Settings.Default.FileIndexNow + 1 > files.Length)
            {
                Properties.Settings.Default.FileIndexNow = 0;
                Properties.Settings.Default.Save();
            }

        

        }

        private void ListFileToView()
        {
            ListViewFiles.Items.Clear();
            foreach (var item in ListFile)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = item;
                ListViewFiles.Items.Add(lvi);
            }
        }

        private void ListDocToView()
        {
            ListViewDoc.Items.Clear();

            foreach (var item in ListDoc)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = item;
                ListViewDoc.Items.Add(lvi);
            }

        }

        private void ReadDocs()
        {
            string line;
            ListDoc.Clear();

            if (Properties.Settings.Default.FileIndexNow < 0)
            {
                return;
            }

            if (!System.IO.File.Exists(MyPath + "\\" + ListFile[Properties.Settings.Default.FileIndexNow].FileName))
                return;

            //if (!File.Exists(@"D:\Documents\Temp\MyFile.txt"))
            //    return;

            System.IO.StreamReader sr = new System.IO.StreamReader(MyPath + "\\" + ListFile[Properties.Settings.Default.FileIndexNow].FileName);

            while ((line = sr.ReadLine()) != null)
            {
                string[] sa = line.Split('\t');

                Doc doc = new Doc();
                doc.Name = sa[0];

                if (sa[1] == "false")
                    doc.Activ = false;
                else
                    doc.Activ = true;

                ListDoc.Add(doc);

            }
            sr.Close();
        }

        private void WriteDocs()
        {
            if (System.IO.File.Exists(MyPath + "\\" + ListFile[Properties.Settings.Default.FileIndexNow].FileName))
            {
                System.IO.File.Delete(MyPath + "\\" + ListFile[Properties.Settings.Default.FileIndexNow].FileName);
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(MyPath + "\\" + ListFile[Properties.Settings.Default.FileIndexNow].FileName))
            {

                foreach (var item in ListDoc)
                {
                    string sDoc = item.ToString();
                    file.WriteLine(sDoc);
                }
            }
        }


        private void ButtonFileAdd_Click(object sender, RoutedEventArgs e)
        {
            File f = new File();
            string s = "";

            if (TextBoxFileAdd.Text.Contains('.'))
            {
                s = TextBoxFileAdd.Text;
            }
            else
            {
                s = TextBoxFileAdd.Text+".txt";
            }

            if (s.Length<5)
            {
                MessageBox.Show("Name zu kurz\nanderen Namen wählen");
                return;
            }


            foreach (var item in ListFile)
            {
                if (item.FileName==s)
                {
                    MessageBox.Show("Datei existiert schon\nanderen Namen wählen");
                    return;
                }
            }




            f.FileName = s;
            ListFile.Add(f);
            System.IO.File.WriteAllText(MyPath + "\\" + s, "");
            ListFileToView();

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {

            if (CheckBoxGoogle.IsChecked == false && CheckBoxBing.IsChecked == false)
            {
                CheckBoxGoogle.IsChecked = true;
            }


            string strSearch = TextBoxSearch1.Text + " ) site:";

            foreach (var item in ListDoc)
            {
                if (item.Activ)
                {
                    if (CheckBoxGoogle.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + strSearch + item.Name);
                    if (CheckBoxBing.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + strSearch + item.Name);
                }

            }


            //try
            //{

            //    foreach (var item in ListViewDoc.SelectedItems)
            //    {
            //        ListViewItem lvi = (ListViewItem)item;
            //        Doc doc = (Doc)lvi.Content;

            //        if (doc.Activ == true)
            //            Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + doc.Name);
            //        else
            //            Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + doc.Name);
            //    }




            //}
            //catch (Exception)
            //{


            //}



            //// Process.Start("microsoft-edge:http://www.google.com/search?q=thread site:openbook.rheinwerk-verlag.de/csharp") ;
            //Process.Start("microsoft-edge:http://www.bing.de/search?q=(thread AND Array) (site:openbook.rheinwerk-verlag.de/csharp OR site:stackoverflow.com)");
            //Process.Start("microsoft-edge:http://www.bing.de/search?q=(threadpool) (site:openbook.rheinwerk-verlag.de/csharp OR site:stackoverflow.com)");
        }

        private void ButtonDocAdd_Click(object sender, RoutedEventArgs e)
        {
            Doc doc = new Doc();
            doc.Name = TextBoxDocAdd.Text;
            doc.Activ = true;


            ListDoc.Add(doc);

            WriteDocs();

            ListDocToView();
            TextBoxDocAdd.Text = "";



        }

        private void ButtonLinkBing_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://help.bing.microsoft.com/#apex/18/de-de/10001/-1");
        }

        private void ButtonLinkGoogle_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://support.google.com/websearch/answer/2466433?hl=de");
        }

        private void ListViewDoc_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int selIndex = ListViewDoc.SelectedIndex;

                if (selIndex < 0)
                {
                    return;
                }

                TextBoxDocAdd.Text = ListDoc[selIndex].Name;



                var x = ListViewDoc.SelectedItems;

                var z = x.Count;

                ListViewItem y = (ListViewItem)x[0];


                MessageBoxResult result = MessageBox.Show("Soll  '" + ListDoc[selIndex].Name + "' gelöscht werden?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    ListDoc.RemoveAt(selIndex);
                    WriteDocs();
                    ListDocToView();

                }
            }
            catch (Exception)
            {


            }

        }

        private void ListViewDoc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int selIndex = ListViewDoc.SelectedIndex;

            if (selIndex <0)
            {
                return;
            }
            TextBoxDocAdd.Text = ListDoc[selIndex].Name;

            //ListDoc[selIndex].Activ = !ListDoc[selIndex].Activ;
            //WriteDocs();
            //ListDocToView();


        }

        private void ListViewFiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int selIndex = ListViewFiles.SelectedIndex;

            if (selIndex < 0)
            {
                return;
            }


            Properties.Settings.Default.FileIndexNow = selIndex;
            Properties.Settings.Default.Save();


            ReadDocs();
            ListDocToView();
        }

        private void ListViewFiles_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int selIndex = ListViewFiles.SelectedIndex;

                if (selIndex < 0)
                {
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Soll  '" + ListFile[selIndex].FileName + "' gelöscht werden?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {

                    if (System.IO.File.Exists(MyPath + "\\" + ListFile[selIndex].FileName))
                    {
                        System.IO.File.Delete(MyPath + "\\" + ListFile[selIndex].FileName);
                    }

                    ListFile.RemoveAt(selIndex);
                    ListFileToView();

                    if (ListFile.Count > 0)
                    {
                        Properties.Settings.Default.FileIndexNow = 0;
                        Properties.Settings.Default.Save();
                        ListViewFiles.SelectedIndex = 0;
                    }
                    else
                    {
                        Properties.Settings.Default.FileIndexNow = -1;
                        Properties.Settings.Default.Save();

                    }


                    ReadDocs();
                    ListDocToView();

                }
            }
            catch (Exception)
            {


            }
        }

        private void TextBoxDocAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonDocAdd_Click(null, null);
                TextBoxDocAdd.Text = "";
            }
        }

        private void TextBoxFileAdd_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonFileAdd_Click(null, null);
                TextBoxFileAdd.Text = "";
            }
        }

        private void TextBoxSearch1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSearch_Click(null, null);
             
            }
        }
    }
}
