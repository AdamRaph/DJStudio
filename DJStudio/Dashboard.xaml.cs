using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Media.Audio;
using Windows.Media.Render;
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
    public class AudioPlayer
    {
        private AudioGraph _graph;
        private Dictionary<string, AudioFileInputNode> _fileInputs = new Dictionary<string, AudioFileInputNode>();
        private AudioDeviceOutputNode _deviceOutput;

        public void PlaybackSpeedFactor(string key, double playSpeed)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var sound = _fileInputs[key];
                sound.PlaybackSpeedFactor = playSpeed;

            }
        }
    

    public async Task LoadFileAsync(IStorageFile file)
    {
        if (_deviceOutput == null)
        {
            await CreateAudioGraph();
        }

        var fileInputResult = await _graph.CreateFileInputNodeAsync(file);

        if (!_fileInputs.ContainsKey(file.Name))
        {
            _fileInputs.Add(file.Name, fileInputResult.FileInputNode);
            fileInputResult.FileInputNode.Stop();
            fileInputResult.FileInputNode.AddOutgoingConnection(_deviceOutput);
        }

    }

    public void Play(string key, double gain)
    {
        var sound = _fileInputs[key];
        sound.OutgoingGain = gain;
        sound.Seek(TimeSpan.Zero);
        sound.Start();
    }
    public void Stop(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            var sound = _fileInputs[key];
            sound.Stop();
        }

    }

    private async Task CreateAudioGraph()
    {
        var settings = new AudioGraphSettings(AudioRenderCategory.Media);
        var result = await AudioGraph.CreateAsync(settings);
        _graph = result.Graph;
        var deviceOutputNodeResult = await _graph.CreateDeviceOutputNodeAsync();
        _deviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        _graph.ResetAllNodes();
        _graph.Start();
    }
}

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

    public Dashboard()
    {


        this.InitializeComponent();



        var ellipse1 = new Ellipse
        {
            Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue),
            Width = 200,
            Height = 200
        };

        HeaderPanel.Children.Add(ellipse1);

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
}
}
