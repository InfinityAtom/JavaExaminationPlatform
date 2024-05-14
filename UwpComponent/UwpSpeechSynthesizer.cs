using System.Linq;
using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace UwpComponent
{
    public sealed class UwpSpeechSynthesizer : Grid
    {
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private MediaElement mediaElement = new MediaElement();

        public UwpSpeechSynthesizer()
        {
            Children.Add(mediaElement); // Add the MediaElement to the Grid
            synthesizer.Voice = SpeechSynthesizer.AllVoices.First(v => v.DisplayName.Contains("Aria"));
        }

        public async void Speak(string text)
        {
            var stream = await synthesizer.SynthesizeTextToStreamAsync(text);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }
    }
}
