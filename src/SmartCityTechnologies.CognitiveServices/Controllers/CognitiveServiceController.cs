using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using SmartCityTechnologies.CognitiveServices.Builders;
using SmartCityTechnologies.CognitiveServices.Models;
using SmartCityTechnologies.CognitiveServices.Options;

namespace SmartCityTechnologies.CognitiveServices.Controllers
{
    [Route("api/azure-service/")]
    [ApiController]
    public class CognitiveServiceController : ControllerBase
    {
        private ISpeechBuilder _speechBuilder;
        private FaceDetectionOptions _faceDetectionOptions;
        public CognitiveServiceController(ISpeechBuilder speechBuilder, IOptions<FaceDetectionOptions> faceDetectionOptions)
        {
            _speechBuilder = speechBuilder;
            _faceDetectionOptions = faceDetectionOptions.Value;
        }


        [HttpPost("get-face-properties")]
        public async Task<IActionResult> GetFaceProps(IFormFile file)
        {

            if (file != null)
            {
                IFaceClient faceClient = new FaceClient(
                   new ApiKeyServiceClientCredentials(_faceDetectionOptions.Key),
                   new System.Net.Http.DelegatingHandler[] { });

                faceClient.Endpoint = _faceDetectionOptions.Endpoint;

                var requiredFaceAttributes = new FaceAttributeType?[] {
                    FaceAttributeType.Age,
                    FaceAttributeType.Gender,
                    FaceAttributeType.Smile,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Emotion,
                };

                var faces = await faceClient.Face.DetectWithStreamAsync(file.OpenReadStream());

                if (faces.Any())
                {
                    return Ok(faces.FirstOrDefault());
                }
                else
                {
                    return BadRequest(new { Message = "Face was not recognized" });
                }
            }
            else
            {
                return BadRequest(new
                {
                    ErrorMsg = "File empty or corrupted"
                });
            }

        }

        [HttpPost("convert-text-to-speech")]
        [Produces("application/octet-stream", Type = typeof(FileResult))]
        public async Task<IActionResult> ConvertTextToSpeach([FromBody] TextToSpeechModel model)
        {
            var config = _speechBuilder.UseVoice(model.PreferedLang ?? string.Empty);
            using (var speechSynthesizer = new SpeechSynthesizer(config))
            {
                var result = await speechSynthesizer.SpeakTextAsync(model.Text ?? "There is not text to recognize");

                var name = $"speech_{Guid.NewGuid()}.wav";
                return File(result.AudioData, "application/octet-stream", name);
            }
        }

        [HttpPost("convert-speech-to-text")]
        public async Task<IActionResult> ConvertSpeechToText([FromForm] SpeechToTextModel model)
        {
            var config = _speechBuilder.UseSpeechVoice(model.PreferedLang ?? string.Empty);
            using (var streamRead = model.File.OpenReadStream())
            {
                using (var memory = new MemoryStream())
                {
                    await streamRead.CopyToAsync(memory);
                    using (var pushStream = AudioInputStream.CreatePushStream())
                    {
                        pushStream.Write(memory.ToArray());

                        using (var audioConfig = AudioConfig.FromStreamInput(pushStream))
                        {
                            using (var recognizer = new SpeechRecognizer(config, audioConfig))
                            {
                                var result = await recognizer.RecognizeOnceAsync();
                                return Ok(new { RecognizedText = result.Text });
                            }
                        }
                    }
                }
            }
        }
    }
}
