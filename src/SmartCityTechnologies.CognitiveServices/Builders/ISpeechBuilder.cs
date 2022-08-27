using Microsoft.CognitiveServices.Speech;

namespace SmartCityTechnologies.CognitiveServices.Builders
{
    public interface ISpeechBuilder
    {
        public SpeechConfig UseVoice(string? language = null);
        public SpeechConfig UseSpeechVoice(string? language = null);
        public SpeechConfig Build();
    }
}
