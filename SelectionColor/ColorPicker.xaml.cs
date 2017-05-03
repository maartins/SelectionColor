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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl, INotifyPropertyChanged {
        private List<int[]> colors = new List<int[]>();
        private int selectedValue = 5;
        private int sliderR = 0, sliderG = 0, sliderB = 0;
        private int selectedR = 0, selectedG = 0, selectedB = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public int SliderR {
            get { return sliderR; }
            set {
                if(value != sliderR) {
                    sliderR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int SliderG {
            get { return sliderG; }
            set {
                if(value != sliderG) {
                    sliderG = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int SliderB {
            get { return sliderB; }
            set {
                if(value != sliderB) {
                    sliderB = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ColorPicker() {
            InitializeComponent();

            SetupRgbSlider();

            rgbSlider.Maximum = colors.Count;
        }

        private void SetupRgbSlider() {
            colors.Add(new int[] { 255, 0, 0 });
            // R <- G
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 255, 0 + i, 0 });
            }

            // R -> G
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 255 - i, 255, 0 });
            }

            // G -> B
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 0, 255, 0 + i });
            }

            // G <- B
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 0, 255 - i, 255 });
            }

            // B -> R
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 0 + i, 0, 255 });
            }

            // B <- R
            for(int i = 1; i <= 255; i++) {
                colors.Add(new int[] { 255, 0, 255 - i });
            }
        }

        private Color GetPixelColor(Visual visual, Point pt) {
            Point ptDpi = GetScreenDPI(visual);

            Size srcSize = VisualTreeHelper.GetDescendantBounds(visual).Size;

            //Viewbox uses values between 0 & 1 so normalize the Rect with respect to the visual's Height & Width
            Rect percentSrcRec = new Rect(pt.X / srcSize.Width, pt.Y / srcSize.Height,
                                          1 / srcSize.Width, 1 / srcSize.Height);

            //var bmpOut = new RenderTargetBitmap(1, 1, 96d, 96d, PixelFormats.Pbgra32); //assumes 96 dpi
            var bmpOut = new RenderTargetBitmap((int)(ptDpi.X / 96d),
                                                (int)(ptDpi.Y / 96d),
                                                ptDpi.X, ptDpi.Y, PixelFormats.Default); //generalized for monitors with different dpi

            DrawingVisual dv = new DrawingVisual();
            using(DrawingContext dc = dv.RenderOpen()) {
                dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrcRec },
                                 null, //no Pen
                                 new Rect(0, 0, 1d, 1d));
            }
            bmpOut.Render(dv);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);

            return Color.FromRgb(bytes[2], bytes[1], bytes[0]);
        }

        private Point GetScreenDPI(Visual v) {
            //System.Windows.SystemParameters
            PresentationSource source = PresentationSource.FromVisual(v);
            Point ptDpi;
            if(source != null) {
                ptDpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11,
                                   96.0 * source.CompositionTarget.TransformToDevice.M22);
            } else
                ptDpi = new Point(96d, 96d); //default value.

            return ptDpi;
        }

        private void ColorAdjustment_MouseMove(object sender, MouseEventArgs e) {
            if(e.LeftButton == MouseButtonState.Pressed) {
                Point selectedColorPos = e.GetPosition(ColorAdjustment);
                Color pixelColor = GetPixelColor(ColorAdjustment, selectedColorPos);

                selectedR = pixelColor.R;
                selectedG = pixelColor.G;
                selectedB = pixelColor.B;

                rOutput.Text = selectedR + "";
                gOutput.Text = selectedG + "";
                bOutput.Text = selectedB + "";

                ColorSelectionMarker.Margin = new Thickness(e.GetPosition(CP).X - 5, e.GetPosition(CP).Y - 5, 0, 0);
            }
        }

        private void RgbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(colors.Count != 0) {
                selectedValue = (int)e.NewValue - 1;

                SliderR = colors[selectedValue][0];
                SliderG = colors[selectedValue][1];
                SliderB = colors[selectedValue][2];

                double x = ColorSelectionMarker.Margin.Left - 5;
                double y = ColorSelectionMarker.Margin.Top - 5;

                Point selectedColorPos = new Point(x, y);
                Color pixelColor = GetPixelColor(ColorAdjustment, selectedColorPos);

                selectedR = pixelColor.R;
                selectedG = pixelColor.G;
                selectedB = pixelColor.B;

                rOutput.Text = selectedR + "";
                gOutput.Text = selectedG + "";
                bOutput.Text = selectedB + "";
            }
        }

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public int[] GetSelectedRgbColor() {
            return new int[] { selectedR, selectedG, selectedB };
        }

        public void SetRgbColor(int r, int g, int b) {
            selectedR = r;
            selectedG = g;
            selectedB = b;

            rOutput.Text = selectedR + "";
            gOutput.Text = selectedG + "";
            bOutput.Text = selectedB + "";
        }
    }
}
