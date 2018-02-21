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

        public MainWindow()
        {
            InitializeComponent();
            ReadDocs();
            ShowListDoc();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

         

            try
            {

                foreach (var item in ListViewDoc.SelectedItems)
                {
                    ListViewItem lvi = (ListViewItem)item;
                    Doc doc = (Doc)lvi.Content;

                    if (doc.SearchWithGoogle == true)
                        Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + doc.Name);
                    else
                        Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + doc.Name);
                }


                //while ((selIndex = ListViewDoc.SelectedIndex)>=0)
                //{
                //    if (ListDoc[selIndex].SearchWithGoogle==true)
                //        Process.Start("microsoft-edge:" + "http://www.google.com" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + ListDoc[selIndex].Name);
                //    else
                //        Process.Start("microsoft-edge:" + "http://www.bing.de" + "/search?q=(" + TextBoxSearch1.Text + " ) site:" + ListDoc[selIndex].Name);

                    
                //}
           


            }
            catch (Exception)
            {


            }



            //// Process.Start("microsoft-edge:http://www.google.com/search?q=thread site:openbook.rheinwerk-verlag.de/csharp") ;
            //Process.Start("microsoft-edge:http://www.bing.de/search?q=(thread AND Array) (site:openbook.rheinwerk-verlag.de/csharp OR site:stackoverflow.com)");
            //Process.Start("microsoft-edge:http://www.bing.de/search?q=(threadpool) (site:openbook.rheinwerk-verlag.de/csharp OR site:stackoverflow.com)");
        }

        private void ButtonAddDoc_Click(object sender, RoutedEventArgs e)
        {
            Doc doc = new Doc();
            doc.Name = TextBoxAddDoc.Text;
            if (CheckBoxGoogle.IsChecked == true)
                doc.SearchWithGoogle = true;
            else
                doc.SearchWithGoogle = false;

            ListDoc.Add(doc);

            WriteDocs();

            ShowListDoc();
            TextBoxAddDoc.Text = "";



        }

        private void ShowListDoc()
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

            if (!File.Exists(@"D:\Documents\Temp\MyFile.txt"))
                return;


            System.IO.StreamReader file = new System.IO.StreamReader(@"D:\Documents\Temp\MyFile.txt");

            while ((line = file.ReadLine()) != null)
            {
                string[] sa = line.Split('\t');

                Doc doc = new Doc();
                doc.Name = sa[0];

                if (sa[1] == "false")
                    doc.SearchWithGoogle = false;
                else
                    doc.SearchWithGoogle = true;

                ListDoc.Add(doc);

            }
        }

        private void WriteDocs()
        {
            if (File.Exists(@"D:\Documents\Temp\MyFile.txt"))
            {
                File.Delete(@"D:\Documents\Temp\MyFile.txt");
            }
           

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\Documents\Temp\MyFile.txt"))
            {

                foreach (var item in ListDoc)
                {
                    string sDoc = item.ToString();
                    file.WriteLine(sDoc);
                }
            }
        }

        private void ListViewDoc_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int selIndex = ListViewDoc.SelectedIndex;

                MessageBoxResult result = MessageBox.Show("Soll  '" + ListDoc[selIndex].Name + "' gelöscht werden?", "Achtung", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    ListDoc.RemoveAt(selIndex);
                    WriteDocs();
                    ShowListDoc();

                }
            }
            catch (Exception)
            {

               
            }
            
        }
    }
}
