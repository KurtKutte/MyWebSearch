using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace MyWebSearch
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MyDoc> ListDoc = new List<MyDoc>();
        List<MyFile> ListFile = new List<MyFile>();
        string MyPath = "";


        public MainWindow()
        {
            InitializeComponent();

            IniLayout();

            ReadFiles();
            ListFileToView();

            ReadDocs();
            ListDocToView();

            if (Properties.Settings.Default.StepEnable == true)
                CheckBoxStepMode.IsEnabled = true;
            else
                CheckBoxStepMode.IsEnabled = false;
        }

        private void IniLayout()
        {
            MyWindow.Width = System.Windows.SystemParameters.PrimaryScreenWidth / 3.0;
            MyWindow.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 3.0;
        }

        private void ReadFiles()
        {
            string docPath = Properties.Settings.Default.MyPath; 

            if (!Directory.Exists(docPath))
            {
                MessageBoxResult result = MessageBox.Show("Ein Ordner für 'MyWebSearch' existiert nicht\nSoll er in 'Eigene Dateien' eingerichtet werden?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    MyPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "MyWebSearch";
                    Directory.CreateDirectory(MyPath) ;
                    Properties.Settings.Default.MyPath = MyPath;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    return;
                }
            }
            else
            {
                MyPath = docPath ;
            }




            string[] files = System.IO.Directory.GetFiles(MyPath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            ListFile.Clear();

            if (files.Length > 0)
            {
                foreach (var item in files)
                {
                    string name = item.Substring(item.LastIndexOf('\\') + 1);
                    MyFile f = new MyFile();
                    f.FileName = name;
                    ListFile.Add(f);
                }
            }
            else
            {

                System.IO.File.WriteAllText(MyPath + "\\" + "MyWebSearch.txt", "\t" + "wikipedia.de" + "\t" + "true");

                MyFile f = new MyFile();
                f.FileName = "MyWebSearch.txt";
                ListFile.Add(f);
                files = new string[1];
                files[0] = "MyWebSearch.txt";



            }



            if (Properties.Settings.Default.FileNameNow == "")
            {
                Properties.Settings.Default.FileNameNow = files[0];
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
            int docCount = 0;
            int docActive = 0;

            foreach (var item in ListDoc)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = item;
                if (item.Activ)
                {
                    lvi.Background = Brushes.LightGreen;
                    docActive += 1;
                }
                else
                {
                    lvi.Background = Brushes.White;
                }

                if (item.Name.ToUpper().Contains("MICROSOFT"))
                {
                    lvi.FontWeight = FontWeights.Bold;
                }
                ListViewDoc.Items.Add(lvi);
                docCount += 1;
            }
            LabelCount.Content = docActive.ToString() + " / " + docCount.ToString();
        }

        private void ReadDocs()
        {
            string line;
            ListDoc.Clear();


            if (Properties.Settings.Default.FileNameNow == "")
            {
                return;
            }

            if (!System.IO.File.Exists(MyPath + "\\" + Properties.Settings.Default.FileNameNow))
                return;


            System.IO.StreamReader sr = new System.IO.StreamReader(MyPath + "\\" + Properties.Settings.Default.FileNameNow);

            try
            {
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sa = line.Split('\t');

                    MyDoc doc = new MyDoc();
                    doc.Group = sa[0];
                    doc.Name = sa[1];

                    if (sa[2] == "false")
                        doc.Activ = false;
                    else
                        doc.Activ = true;

                    ListDoc.Add(doc);

                }
            }
            catch (Exception e)
            {
                string name = MyPath + "\\" + Properties.Settings.Default.FileNameNow;
                MessageBox.Show("Datei\n" + name + "\nkann nicht gelesen werden\n" + e.Message);
            }
            finally
            {
                sr.Close();
            }

            for (int i = 0; i < ListFile.Count; i++)
            {
                if (ListFile[i].FileName == Properties.Settings.Default.FileNameNow)
                {
                    ListViewFiles.SelectedIndex = i;
                    break;
                }

            }


        }

        private void WriteDocs()
        {
            if (System.IO.File.Exists(MyPath + "\\" + Properties.Settings.Default.FileNameNow))
            {
                System.IO.File.Delete(MyPath + "\\" + Properties.Settings.Default.FileNameNow);
            }


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(MyPath + "\\" + Properties.Settings.Default.FileNameNow))
            {

                foreach (var item in ListDoc)
                {
                    string sDoc = item.ToString();
                    file.WriteLine(sDoc);
                }
            }
        }

        private void SearchNext(string strSearch)
        {
            LabelBusy.Content = "Abfrage läuft";

            for (int i = 0; i < ListDoc.Count; i++)
            {
                if (ListDoc[i].Activ)
                {
                    if (CheckBoxGoogle.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + strSearch + ListDoc[i].Name);
                    if (CheckBoxBing.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + strSearch + ListDoc[i].Name);

                    if (CheckBoxAutoOff.IsChecked == true)
                    {
                        ListDoc[i].Activ = false;
                    }
                    WriteDocs();
                    ListDocToView();
                    break;
                }


            }


            LabelBusy.Content = "";
        }

        private void SearchAll(string strSearch)
        {
            LabelBusy.Content = "Abfrage läuft";

            for (int i = 0; i < ListDoc.Count; i++)
            {
                if (ListDoc[i].Activ)
                {
                    if (CheckBoxGoogle.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + strSearch + ListDoc[i].Name);
                    if (CheckBoxBing.IsChecked == true)
                        Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + strSearch + ListDoc[i].Name);


                    ListDoc[i].Activ = false;
                    WriteDocs();
                    ListDocToView();

                    Thread.Sleep(2000);
                }
             
            }

            LabelBusy.Content = "";
        }

        private void MnuConfig_Click(object sender, RoutedEventArgs e)
        {
            WindowConfig winConf = new WindowConfig();
            winConf.ShowDialog();

            ReadFiles();
            ListFileToView();

            ReadDocs();
            ListDocToView();

            if (Properties.Settings.Default.StepEnable == true)
                CheckBoxStepMode.IsEnabled = true;
            else
                CheckBoxStepMode.IsEnabled = false;

        }

        private void ButtonDocActiveAll_Click(object sender, RoutedEventArgs e)
        {

            foreach (ListViewItem item in ListViewDoc.Items)
            {
                item.IsSelected = true;
            }

            ListViewDocItemBrushes();

        }

        private void ButtonDocsDelete_Click(object sender, RoutedEventArgs e)
        {

            int selIndex = ListViewDoc.SelectedIndex;

            if (selIndex < 0)
            {
                MessageBox.Show("Kein Element markiert !");
                return;
            }

            try
            {

                TextBoxDocGroupAdd.Text = ListDoc[selIndex].Group;
                TextBoxDocAdd.Text = ListDoc[selIndex].Name;


                var selItems = ListViewDoc.SelectedItems;


                MessageBoxResult result = MessageBox.Show("Sollen die markierten Such-Seiten (" + selItems.Count.ToString() + " Stück) gelöscht werden?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < selItems.Count; i++)
                    {
                        ListViewItem selItemSingle = (ListViewItem)selItems[i];
                        MyDoc selDocSingle = (MyDoc)selItemSingle.Content;


                        foreach (var item in ListDoc)
                        {
                            if (item.Name == selDocSingle.Name)
                            {
                                ListDoc.Remove(item);
                                break;
                            }
                        }

                    }

                    ListDoc.Sort();

                    WriteDocs();
                    ListDocToView();

                    TextBoxDocGroupAdd.Text = "";
                    TextBoxDocAdd.Text = "";

                }
            }
            catch (Exception)
            {


            }
        }

        private void ButtonDocActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selIndex = ListViewDoc.SelectedIndex;

                if (selIndex < 0)
                {
                    MessageBox.Show("Kein Element markiert !");
                    return;
                }

                TextBoxDocGroupAdd.Text = ListDoc[selIndex].Group;
                TextBoxDocAdd.Text = ListDoc[selIndex].Name;

                var selItems = ListViewDoc.SelectedItems;


                for (int i = 0; i < selItems.Count; i++)
                {
                    ListViewItem selItemSingle = (ListViewItem)selItems[i];
                    MyDoc selDocSingle = (MyDoc)selItemSingle.Content;


                    for (int k = 0; k < ListDoc.Count; k++)
                    {
                        if (ListDoc[k].Name == selDocSingle.Name)
                        {
                            ListDoc[k].Activ = true;
                            break;
                        }
                    }

                }

                WriteDocs();
                ListDocToView();

            }
            catch (Exception)
            {


            }
        }

        private void ButtonDocsFalse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selIndex = ListViewDoc.SelectedIndex;

                if (selIndex < 0)
                {
                    MessageBox.Show("Kein Element markiert !");
                    return;
                }

                TextBoxDocGroupAdd.Text = ListDoc[selIndex].Group;
                TextBoxDocAdd.Text = ListDoc[selIndex].Name;

                var selItems = ListViewDoc.SelectedItems;

                for (int i = 0; i < selItems.Count; i++)
                {
                    ListViewItem selItemSingle = (ListViewItem)selItems[i];
                    MyDoc selDocSingle = (MyDoc)selItemSingle.Content;


                    for (int k = 0; k < ListDoc.Count; k++)
                    {
                        if (ListDoc[k].Name == selDocSingle.Name)
                        {
                            ListDoc[k].Activ = false;
                            break;
                        }
                    }

                }

                WriteDocs();
                ListDocToView();
            }
            catch (Exception)
            {


            }
        }

        private void ButtonFileAdd_Click(object sender, RoutedEventArgs e)
        {
            MyFile f = new MyFile();
            string s = "";

            if (TextBoxFileAdd.Text.Contains('.'))
            {
                s = TextBoxFileAdd.Text;
            }
            else
            {
                s = TextBoxFileAdd.Text + ".txt";
            }

            if (s.Length < 5)
            {
                MessageBox.Show("Name zu kurz\nanderen Namen wählen");
                return;
            }


            foreach (var item in ListFile)
            {
                if (item.FileName == s)
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
            bool OneItemActive = false;
            int ItemActiceCount = 0;

            #region Prüfung, on ein Element aktiv (true) angewählt ist

            foreach (var item in ListDoc)
            {
                if (item.Activ)
                {
                    OneItemActive = true;
                    ItemActiceCount += 1;
                }

            }

            if (!OneItemActive)
            {
                MessageBox.Show("kein Eintrag AKTIV - Suche nicht möglich");
                return;
            }

            if (ItemActiceCount>1 && CheckBoxAutoOff.IsChecked==false)
            {
                MessageBox.Show("Achtung - 'with deselect' ist nicht aktiviert,\n" +
                                 "dadurch wird immer nur die erste grün markierte Seite angewählt\n"+
                                 "");
              
            }




            #endregion


            if (CheckBoxGoogle.IsChecked == false && CheckBoxBing.IsChecked == false)
            {
                CheckBoxGoogle.IsChecked = true;
            }

            string strSearch = TextBoxSearch1.Text + " ) site:";

            if (CheckBoxStepMode.IsChecked == false)
            {
                SearchAll(strSearch);
            }
            else
            {
                SearchNext(strSearch);
            }


        }

        private void ButtonSyntax_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gib die wichtigsten Wörter ein: glatthaar foxterrier dreifarbig \n\n " +
                "Setze die gesuchten Wörter zwischen Anführungszeichen:\n \"glatthaar terrier\"  \n\n" +
                "Gib ODER zwischen allen gesuchten Wörtern ein:\n miniatur OR standard\n\n" +
                "Setze ein Minuszeichen direkt vor Wörter, die nicht angezeigt werden sollen:\n -rauhhaar, -\"jack russell\" ");
        }

        private void ButtonPageStart_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("microsoft-edge:" + TextBoxDocAdd.Text);
            // Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + strSearch + ListDoc[i].Name);
        }

        private void ButtonDocAdd_Click(object sender, RoutedEventArgs e)
        {

            if (TextBoxDocAdd.Text.Length < 5)
            {
                MessageBox.Show("Name zu kurz\nanderen Namen wählen");
                return;
            }
            if (TextBoxDocGroupAdd.Text == "")
            {
                MessageBoxResult result =
                             MessageBox.Show("Keine Gruppe angegeben\nOhne Gruppe speichern?", "Achtung", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (TextBoxDocGroupAdd.Text.Length > 5)
            {
                MessageBox.Show("Gruppenname darf max. 5 Buchstaben haben\nanderen Namen wählen");
                return;
            }

            MyDoc doc = new MyDoc();
            doc.Name = TextBoxDocAdd.Text;
            doc.Group = TextBoxDocGroupAdd.Text;
            doc.Activ = true;


            for (int i = 0; i < ListDoc.Count; i++)
            {
                if (ListDoc[i].Name == doc.Name)
                {
                    MessageBoxResult result =
                                MessageBox.Show("Seite existiert schon\nüberschreiben ?", "Achtung", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    ListDoc.RemoveAt(i);
                    break;
                }
            }
            ListDoc.Add(doc);
            ListDoc.Sort();

            WriteDocs();

            ListDocToView();
            TextBoxDocAdd.Text = "";
            TextBoxDocGroupAdd.Text = "";



        }

        private void ButtonLinkBing_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://help.bing.microsoft.com/#apex/18/de-de/10001/-1");
        }

        private void ButtonLinkGoogle_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://support.google.com/websearch/answer/2466433?hl=de");
        }

        private void ButtonSearchDelete_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearch1.Text = "";
        }

        private void ButtonPageCheck_Click(object sender, RoutedEventArgs e)
        {
            List<String> listNewTerms = new List<String>();
            List<String> listItemTerms = new List<String>();

            List<String> listFound = new List<String>();


            string[] newTerms = TextBoxDocAdd.Text.Split('/');

            for (int i = 0; i < newTerms.Length; i++)
            {
                if (!newTerms[i].Contains(':') & newTerms[i] != "")
                {
                    listNewTerms.Add(newTerms[i]);
                }
            }


            foreach (var item in ListDoc)
            {
                string[] itemTerms = item.Name.Split('/');
                listItemTerms.Clear();


                for (int i = 0; i < itemTerms.Length; i++)
                {
                    if (!itemTerms[i].Contains(':') & itemTerms[i] != "")
                    {
                        listItemTerms.Add(itemTerms[i]);
                    }
                }


                foreach (var termNew in listNewTerms)
                {
                    foreach (var termOld in listItemTerms)
                    {
                        if (termNew == termOld)
                        {
                            if (!listFound.Contains(item.Name))
                            {
                                listFound.Add(item.Name);
                            }
                        }
                    }

                }
            }


            string s = "";
            foreach (var item in listFound)
            {

                s += item + "\n";

            }


            if (s == "")
            {
                MessageBox.Show("keine Übereinstimmung gefunden");
            }
            else
            {

                MessageBox.Show(TextBoxDocAdd.Text + "\n" +
                               "-           Übereinstimmungen bei:\n" + s);
            }


        }

        private void ButtonAddLineClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxDocAdd.Text = "";
        }

        private void ListViewDoc_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            int selIndex = ListViewDoc.SelectedIndex;

            if (selIndex < 0)
            {
                return;
            }

            TextBoxDocAdd.Text = ListDoc[selIndex].Name;


            ListDoc[selIndex].Activ = !ListDoc[selIndex].Activ;

            WriteDocs();
            ListDocToView();



        }

        private void ListViewDoc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int selIndex = ListViewDoc.SelectedIndex;

            if (selIndex < 0)
            {
                return;
            }
            TextBoxDocGroupAdd.Text = ListDoc[selIndex].Group;
            TextBoxDocAdd.Text = ListDoc[selIndex].Name;

            ListViewDocItemBrushes();

        }

        private void ListViewDocItemBrushes()
        {
            for (int i = 0; i < ListViewDoc.Items.Count; i++)
            {
                ListViewItem lvi = (ListViewItem)ListViewDoc.Items[i];

                if (lvi.IsSelected)
                {
                    lvi.Background = Brushes.LightBlue;
                    continue;
                }

                MyDoc doc = (MyDoc)lvi.Content;


                if (doc.Activ)
                {
                    lvi.Background = Brushes.LightGreen;
                }
                else
                {
                    lvi.Background = Brushes.White;
                }

            }
        }

        private void ListViewFiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int selIndex = ListViewFiles.SelectedIndex;

            if (selIndex < 0)
            {
                return;
            }


            Properties.Settings.Default.FileNameNow = ListFile[selIndex].FileName;
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
                        Properties.Settings.Default.FileNameNow = ListFile[0].FileName;
                        Properties.Settings.Default.Save();
                        ListViewFiles.SelectedIndex = 0;
                    }
                    else
                    {
                        Properties.Settings.Default.FileNameNow = "";
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
                TextBoxDocGroupAdd.Text = "";
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
