using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using SmartCityTechnologies.CognitiveServices.Options;

namespace SmartCityTechnologies.CognitiveServices.Builders
{
    public class SpeechConfigBuilder : ISpeechBuilder
    {
        private string _deaultVoice = "en-US";
        private string _voice = "-JennyNeural";
        private SpeechConfig _config;
        public SpeechConfigBuilder(IOptions<TextSpeechOptions> ops)
        {
            _config = SpeechConfig.FromSubscription(ops.Value.Key, ops.Value.Region);
        }

        public SpeechConfig UseVoice(string? language = null)
        {
            if (!string.IsNullOrEmpty(language))
            {
                _config.SpeechSynthesisVoiceName = language + _voice;
                _deaultVoice = language;
            }
            else
            {
                _config.SpeechSynthesisVoiceName = _deaultVoice;
            }

            return _config;
        }

        public SpeechConfig UseSpeechVoice(string? language = null)
        {
            if (!string.IsNullOrEmpty(language))
            {
                _config.SpeechRecognitionLanguage = language;
                _deaultVoice = language;
            }
            else
            {
                _config.SpeechSynthesisVoiceName = _deaultVoice;
            }

            return _config;
        }

        public SpeechConfig Build()
        {
            return _config;
        }
    }
}
