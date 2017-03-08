using LiteGuard;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace TIKSN.Speech
{
	public class TextToSpeechService : ITextToSpeechService
	{
		private readonly IOptions<TextToSpeechOptions> _options;
		private readonly SpeechSynthesizer speaker;

		public TextToSpeechService(IOptions<TextToSpeechOptions> options)
		{
			Guard.AgainstNullArgument(nameof(options), options);

			_options = options;

			speaker = new SpeechSynthesizer();
		}

		public Task SpeakAsync(string text)
		{
			var promptBuilder = new PromptBuilder();

			if (!string.IsNullOrEmpty(_options.Value.VoiceId))
			{
				var voice = speaker.GetInstalledVoices().SingleOrDefault(v => v.Enabled && v.VoiceInfo.Id == _options.Value.VoiceId); //TODO: Cache in memory cache

				if (voice != null)
				{
					promptBuilder.StartVoice(voice.VoiceInfo);
					promptBuilder.AppendText(text);
					promptBuilder.EndVoice();
				}
			}

			if (promptBuilder.IsEmpty)
				promptBuilder.AppendText(text);

			speaker.SpeakAsync(promptBuilder);

			return Task.FromResult<object>(null);
		}
	}
}