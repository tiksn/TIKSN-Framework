namespace TIKSN.Speech;

public interface ITextToSpeechService
{
    public Task SpeakAsync(string text);
}
