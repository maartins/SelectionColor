using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SelectionColor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        
        public MainWindow() {
            InitializeComponent();

            ReadColorsFromRegistry();
        }

        private void ReadColorsFromRegistry() {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors\", false);

            string textSelection = (string)key.GetValue("Hilight");
            string rectangleSelectionFill = (string)key.GetValue("HotTrackingColor");
            string menuHilight = (string)key.GetValue("MenuHilight");

            key.Close();

            int[] textSelectColor = StringToColorArray(textSelection);
            int[] rectSelectFillColor = StringToColorArray(rectangleSelectionFill);
            int[] thirdColors = StringToColorArray(menuHilight);

            textSelectCP.SetRgbColor(textSelectColor[0], textSelectColor[1], textSelectColor[2]);
            rectSelectFillCP.SetRgbColor(rectSelectFillColor[0], rectSelectFillColor[1], rectSelectFillColor[2]);
        }

        private int[] StringToColorArray(string value) {
            int[] output = new int[] { 0, 0, 0 };

            if(!value.Equals("")) {
                value += " ";
                string tmp = "";
                int counter = 0;

                for(int i = 0; i < value.Length; i++) {
                    if(Char.IsDigit(value[i])) {
                        tmp += value[i];
                    } else {
                        output[counter] = int.Parse(tmp);
                        tmp = "";
                        counter++;
                    }
                }
            }
            Console.WriteLine(output[0] + " " + output[1] + " " + output[2]);
            return output;
        }

        private void SetColorsButton_Click(object sender, RoutedEventArgs e) {
            int[] textSelectColor = textSelectCP.GetSelectedRgbColor();
            int[] rectSelectFillColor = rectSelectFillCP.GetSelectedRgbColor();

            String textSelection = textSelectColor[0] + " " + textSelectColor[1] + " " + textSelectColor[2];
            String rectangleSelectionFill = rectSelectFillColor[0] + " " + rectSelectFillColor[1] + " " + rectSelectFillColor[2];

            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors\", true);
            key.SetValue("Hilight", textSelection);
            key.SetValue("HotTrackingColor", rectangleSelectionFill);
            key.Close();
        }
    }
}
