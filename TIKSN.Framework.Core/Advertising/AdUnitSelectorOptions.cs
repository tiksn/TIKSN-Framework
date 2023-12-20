namespace TIKSN.Advertising;

public class AdUnitSelectorOptions
{
    public AdUnitSelectorOptions()
    {
        this.IsDebuggerSensitive = true;
        this.IsDebug = false;
    }

    public bool IsDebuggerSensitive { get; set; }
    public bool IsDebug { get; set; }
}
