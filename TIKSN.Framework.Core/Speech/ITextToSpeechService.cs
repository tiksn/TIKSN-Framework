using System.Threading.Tasks;

namespace TIKSN.Speech
{
    public interface ITextToSpeechService
    {
        Task SpeakAsync(string text);
    }
}
