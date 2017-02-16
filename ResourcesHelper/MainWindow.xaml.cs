using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;

namespace ResourcesHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }        

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                filePath.Text = filename;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;

                var result = dialog.ShowDialog();
                
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var path = dialog.SelectedPath;
                    folderPath.Text += ";"+path;

                    folderPath.Text = folderPath.Text.TrimStart(';');
                }
            }

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void Start()
        {
            var doc = new XmlDocument();

            doc.Load(filePath.Text);

            var dataEls = doc.GetElementsByTagName("data");

            var resourceElements = new Dictionary<string, string>();

            foreach (XmlElement item in dataEls)
            {
                var name = item.Attributes["name"]?.Value;
                var value = (item.GetElementsByTagName("value")[0] as XmlElement).InnerText;

                resourceElements.Add(name, value);
            }

            var paths = folderPath.Text.Split(';');

            var files = new List<string>();

            foreach (var item in paths)
            {
                var list = Directory.GetFiles(item, "*.cs");
                files.AddRange(list);
            }
            

            foreach (var item in resourceElements)
            {
                var isUsed = false;

                foreach (var file in files)
                {
                    var lines = File.ReadLines(file);

                    foreach (var line in lines)
                    {
                        if (!Regex.IsMatch(line, "^\\s*//") && line.Contains("GlobalMessages." + item.Key))
                        {
                            isUsed = true;
                            textBoxKeys.AppendText(item.Key + "\r\n");
                            textBoxValues.AppendText(item.Value + "\r\n");
                            goto BreakTwoLoops;
                        }
                    }
                }

                BreakTwoLoops:

                if (!isUsed)
                {
                    textBox.AppendText(item.Key + "\r\n");
                }

            }

        }
    }

}
