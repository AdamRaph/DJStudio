using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DJStudio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        public async void LoadAnotherWindow()
        {

            var viewId = 0;

            var newView = CoreApplication.CreateNewView();
            await newView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(Display), null);
                    Window.Current.Content = frame;

                    viewId = ApplicationView.GetForCurrentView().Id;



                    Window.Current.Activate();
                });

            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewId);



        }

        RadialController myController;



        public string CurrentSoundKey { get; set; }
        public AudioPlayer Songplayer = new AudioPlayer();
        public AudioPlayer DjEffectplayer = new AudioPlayer();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();




        public Dashboard()
        {

            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,5);
            dispatcherTimer.Start();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            this.InitializeComponent();




            LoadAnotherWindow();






            // Create a reference to the RadialController.
            myController = RadialController.CreateForCurrentView();

            // Create an icon for the custom tool.
            var icon =
              RandomAccessStreamReference.CreateFromUri(
                new Uri("ms-appx:///Assets/StoreLogo.png"));

            // Create a menu item for the custom tool.
            var myItem =
              RadialControllerMenuItem.CreateFromIcon("Sample", icon);
            var myItem2 =
              RadialControllerMenuItem.CreateFromIcon("Sample2", icon);

            // Add the custom tool to the RadialController menu.
            myController.Menu.Items.Add(myItem);
            myController.Menu.Items.Add(myItem2);

            // Declare input handlers for the RadialController.
            myController.ButtonClicked += MyController_ButtonClicked;
            myController.RotationChanged += MyController_RotationChanged;


        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            var n = Songplayer.GetAudioFileInputNode(CurrentSoundKey);
            if (n != null)
            {
                var x = n.Position.TotalSeconds;


                //var ellipse1 = new Ellipse();
                //Random randomGen = new Random();
                //var value1 = randomGen.Next(1, 10);
                //var value2 = randomGen.Next(1, 10);

                //Random rnd = new Random();
                //Windows.UI.Color clr = Windows.UI.Color.FromArgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                //ellipse1.Fill = new SolidColorBrush(clr);
                //ellipse1.Width = (int)(value1 * 10/ value2);
                //ellipse1.Height = (int)(value1 * 20/ value2);

                //HeaderPanel.Children.Add(ellipse1);
            }

        }

        // Handler for rotation input from the RadialController.
        private void MyController_RotationChanged(RadialController sender,
          RadialControllerRotationChangedEventArgs args)
        {
            if (RotationSlider.Value + args.RotationDeltaInDegrees > 100)
            {
                RotationSlider.Value = 100;
                return;
            }
            else if (RotationSlider.Value + args.RotationDeltaInDegrees < 0)
            {
                RotationSlider.Value = 0;
                return;
            }
            RotationSlider.Value += args.RotationDeltaInDegrees;
        }

        // Handler for click input from the RadialController.
        private void MyController_ButtonClicked(RadialController sender,
          RadialControllerButtonClickedEventArgs args)
        {
            ButtonToggle.IsOn = !ButtonToggle.IsOn;
        }
        private void PlaySpeedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            Songplayer.PlaybackSpeedFactor(CurrentSoundKey, playSpeedSlider.Value);


        }


        private async void songslistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songslistView.SelectedIndex >= 0)
            {
                var currentSong = songslistView.Items[songslistView.SelectedIndex] as Windows.UI.Xaml.Controls.ListViewItem;
                if (currentSong != null)
                {

                    Songplayer.Stop(CurrentSoundKey);
                    var path = $"ms-appx:///Assets/Audio/{currentSong.Content}.mp3";
                    var wav = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));

                    await Songplayer.LoadFileAsync(wav);

                    Songplayer.Play($"{currentSong.Content}.mp3", 0.5);
                    CurrentSoundKey = $"{currentSong.Content}.mp3";
                }

            }


        }

        private async void Effect_Click(object sender, RoutedEventArgs e)
        {
            var currentEffect = sender as Button;
            if (currentEffect != null)
            {
                var path = $"ms-appx:///Assets/Audio/Effects/{currentEffect.Content}.mp3";
                var wav = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));

                await DjEffectplayer.LoadFileAsync(wav);

                DjEffectplayer.Play($"{currentEffect.Content}.mp3", 0.5);



            }
        }
        // Change effect paramters to reflect UI control
        private void Eq1Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Songplayer.EqEffectDefinition != null)
            {
                double currentValue = ConvertRange(eq1Slider.Value);
                Songplayer.EqEffectDefinition.Bands[0].Gain = currentValue;
            }
        }

        private void Eq2Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Songplayer.EqEffectDefinition != null)
            {
                double currentValue = ConvertRange(eq2Slider.Value);
                Songplayer.EqEffectDefinition.Bands[1].Gain = currentValue;
            }
        }

        private void Eq3Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Songplayer.EqEffectDefinition != null)
            {
                double currentValue = ConvertRange(eq3Slider.Value);
                Songplayer.EqEffectDefinition.Bands[2].Gain = currentValue;
            }
        }

        private void Eq4Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Songplayer.EqEffectDefinition != null)
            {
                double currentValue = ConvertRange(eq4Slider.Value);
                Songplayer.EqEffectDefinition.Bands[3].Gain = currentValue;
            }
        }

        // Mapping the 0-100 scale of the slider to a value between the min and max gain
        private double ConvertRange(double value)
        {
            // These are the same values as the ones in xapofx.h
            const double fxeq_min_gain = 0.126;
            const double fxeq_max_gain = 7.94;

            double scale = (fxeq_max_gain - fxeq_min_gain) / 100;
            return (fxeq_min_gain + ((value) * scale));
        }

        //public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        //{
        //   // if (buffer == null) throw new ArgumentNullException("buffer");

        //    Func<CancellationToken, IProgress<uint>, Task<IBuffer>> taskProvider =
        //    (token, progress) => ReadBytesAsync(buffer, count, token, progress, options);

        //    return AsyncInfo.Run(taskProvider);
        //}

        //private async Task<IBuffer> ReadBytesAsync(IBuffer buffer, uint count, CancellationToken token, IProgress<uint> progress, InputStreamOptions options)
        //{

        //    return buffer;
        //}

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var n = Songplayer.GetAudioFileInputNode(CurrentSoundKey);
            if (n != null)
            {
                var x = n.Position;
                var ellipse1 = new Ellipse();
                ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
                ellipse1.Width = 200;
                ellipse1.Height = 200;

                HeaderPanel.Children.Add(ellipse1);
            }
        }

    }
}
