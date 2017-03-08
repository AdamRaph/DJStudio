using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace DJStudio
{
    public class AudioPlayer
    {
        public   EqualizerEffectDefinition EqEffectDefinition;
        private AudioGraph _graph;
        private readonly Dictionary<string, AudioFileInputNode> _fileInputs = new Dictionary<string, AudioFileInputNode>();
        private AudioDeviceOutputNode _deviceOutput;

        public AudioPlayer()
        {
            
        }

        public AudioFileInputNode GetAudioFileInputNode(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var sound = _fileInputs[key];
                return sound;

            }
            return null;
        }

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
        private void CreateEqEffect(string key)
        {
            // See the MSDN page for parameter explanations
            // https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.xapofx.fxeq_parameters(v=vs.85).aspx
            EqEffectDefinition = new EqualizerEffectDefinition(_graph);
            EqEffectDefinition.Bands[0].FrequencyCenter = 100.0f;
            EqEffectDefinition.Bands[0].Gain = 4.033f;
            EqEffectDefinition.Bands[0].Bandwidth = 1.5f;
            EqEffectDefinition.Bands[1].FrequencyCenter = 900.0f;
            EqEffectDefinition.Bands[1].Gain = 1.6888f;
            EqEffectDefinition.Bands[1].Bandwidth = 1.5f;
            EqEffectDefinition.Bands[2].FrequencyCenter = 5000.0f;
            EqEffectDefinition.Bands[2].Gain = 2.4702f;
            EqEffectDefinition.Bands[2].Bandwidth = 1.5f;
            EqEffectDefinition.Bands[3].FrequencyCenter = 12000.0f;
            EqEffectDefinition.Bands[3].Gain = 5.5958f;
            EqEffectDefinition.Bands[3].Bandwidth = 2.0f;

            if (!string.IsNullOrEmpty(key))
            {
                var sound = _fileInputs[key];
                sound.EffectDefinitions.Add(EqEffectDefinition);
                sound.EnableEffectsByDefinition(EqEffectDefinition);

            }

           
        }

        public void Play(string key, double gain)
        {
            var sound = _fileInputs[key];
            CreateEqEffect(key);
            sound.OutgoingGain = gain;
            sound.Seek(new TimeSpan(0));
           

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
}