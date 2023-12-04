namespace TIKSN.Speech;

public interface ITextToSpeechService
{
    Task SpeakAsync(string text);
}
