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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace DES_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool[] Key1;
        public bool[] Key2;
        public bool[] Key3;
        public string Input;

        public MainWindow()
        {
            InitializeComponent();
            Key1 = new bool[64];
            Key2 = new bool[64];
            Key3 = new bool[64];
        }

        private void BtKey1_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                int counter = 0;
                string[] lines = File.ReadAllLines(openFileDialog.FileName);
                for(int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (lines[i][j] == '0') Key1[counter] = false;
                        else Key1[counter] = true;
                        counter++;
                    }
                }
                LbK1.Content = openFileDialog.FileName;
            }

        }

       
        

        private void BtInput_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Input = openFileDialog.FileName;
                LbIn.Content = openFileDialog.FileName;
                using (Stream str = File.Open(openFileDialog.FileName, FileMode.Open))
                {
                    byte[] fileBytes = ReadFully(str);
                    TbOutput.Text = Encoding.ASCII.GetString(fileBytes);
                }
            }
        }

        private void BtCipher_OnClick(object sender, RoutedEventArgs e)
        {
            FIPS46 system = new FIPS46();
            system.Key1 = this.Key1;
            system.Key2 = this.Key2;
            system.Key3 = this.Key3;
            system.TripleEncrypt(Input, "enc.bin");
            using (Stream str = File.Open("enc.bin", FileMode.Open))
            {
                byte[] fileBytes = ReadFully(str);
                TbOutput.Text = Encoding.ASCII.GetString(fileBytes);
            }
        }

        private void BtDecipher_OnClick(object sender, RoutedEventArgs e)
        {
            FIPS46 system = new FIPS46();
            system.Key1 = this.Key1;
            system.Key2 = this.Key2;
            system.Key3 = this.Key3;
            system.TripleDecrypt("enc.bin", "dec.bin");
            using (Stream str = File.Open("dec.bin", FileMode.Open))
            {
                byte[] fileBytes = ReadFully(str);
                TbOutput.Text = Encoding.ASCII.GetString(fileBytes);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
