

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace CongnitiveServerConsole
{
    public class SpeechSynthesizerService
    {
        private ConfigurationReader configurationReader;

        public SpeechSynthesizerService(ConfigurationReader configurationReader)
        {
            this.configurationReader = configurationReader;
        }

        private SpeechConfig GetClient()
        {
            var subscriptionKey = configurationReader.GetKey();
            var region = configurationReader.GetRegion();
            return SpeechConfig.FromSubscription(subscriptionKey, region);
        }

        private SpeechTranslationConfig GetSpeechClient()
        {
            var subscriptionKey = configurationReader.GetKey();
            var region = configurationReader.GetRegion();
            return SpeechTranslationConfig.FromSubscription(subscriptionKey, region);
        }

        public async Task<string> SynthesisToSpeakerAsync(string text)
        {
            var config = GetClient();
            var sb = new StringBuilder();


            using var synthesizer = new SpeechSynthesizer(config, 
                AutoDetectSourceLanguageConfig.FromOpenRange(), 
                AudioConfig.FromDefaultSpeakerOutput());

            using var result = await synthesizer.SpeakTextAsync(text);
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                sb.AppendLine($"Speech synthesized to speaker for text [{text}]");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    sb.AppendLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    sb.AppendLine($"CANCELED: Did you update the subscription info?");
                }
            }
            return sb.ToString();
        }

        public async Task RecognizeSpeechAsync()
        {
            var config = GetClient();
            using var recognizer = new SpeechRecognizer(config);
            Console.WriteLine("Say something...");
            var result = await recognizer.RecognizeOnceAsync();
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"We recognized: {result.Text}");
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
        }

        public async Task TranslationContinuousRecognitionAsync(
            string fromLanguage = "en-US", string targetLanguage = "nl", string voice = "nl-NL-HannaRUS")
        {
            var config = GetSpeechClient();
            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage(targetLanguage);
            config.VoiceName = voice;
            using var recognizer = new TranslationRecognizer(config);
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING in '{fromLanguage}': Text={e.Result.Text}");
                foreach (var element in e.Result.Translations)
                {
                    Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
                }
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.TranslatedSpeech)
                {
                    Console.WriteLine($"\nFinal result: Reason: {e.Result.Reason.ToString()}, recognized text in {fromLanguage}: {e.Result.Text}.");
                    foreach (var element in e.Result.Translations)
                    {
                        Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");

                        SynthesisToSpeakerAsync(element.Value).Wait();
                    }
                }
            };

            recognizer.Synthesizing += (s, e) =>
            {
                var audio = e.Result.GetAudio();
                Console.WriteLine(audio.Length != 0
                    ? $"AudioSize: {audio.Length}"
                    : $"AudioSize: {audio.Length} (end of synthesis data)");
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"\nRecognition canceled. Reason: {e.Reason}; ErrorDetails: {e.ErrorDetails}");
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\nSession started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\nSession stopped event.");
            };

            Console.WriteLine("Say something...");
            await recognizer.StartContinuousRecognitionAsync();

            do
            {
                Console.WriteLine("Press Enter to stop");
            } while (Console.ReadKey().Key != ConsoleKey.Enter);
            await recognizer.StopContinuousRecognitionAsync();
        }
    }
}
