using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DJStudio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Display : Page
    {

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public Display()
        {
            this.InitializeComponent();
           

            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            dispatcherTimer.Start();
            dispatcherTimer.Tick += DispatcherTimer_Tick;


        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            
            var desiredCenterX = TheCanvas.Width / 2;
            var desiredCenterY = TheCanvas.Height / 2;


            var colors = new[] {Windows.UI.Colors.Yellow, Windows.UI.Colors.Red, Windows.UI.Colors.Green, Windows.UI.Colors.Blue , Windows.UI.Colors.Pink ,Windows.UI.Colors.White };

            var rnd = new Random();
            var rndnum1 = (double)(rnd.Next(1, 20));

           
            var w1 = rndnum1 * 20;
            var h1 = Math.Abs(rndnum1 * 25);


            var colorsIndex = (rnd.Next(0, colors.Length));



            var ellipse1 = new Ellipse
            {
                Stroke = new SolidColorBrush(colors[colorsIndex]),
                StrokeThickness = (double) (rnd.Next(1, 5)),
                Width = w1,
                Height = h1
            };

            TheCanvas.Children.Clear();

            double left1 = desiredCenterX - (w1 / 2);
            double top1 = desiredCenterY - (h1 / 2);

            ellipse1.Margin = new Thickness(left1, top1, 0, 0);
            TheCanvas.Children.Add(ellipse1);
           
            

          

        }
    }
}
