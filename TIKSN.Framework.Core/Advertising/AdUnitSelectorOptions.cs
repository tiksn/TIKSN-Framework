namespace TIKSN.Advertising;

public class AdUnitSelectorOptions
{
    public AdUnitSelectorOptions()
    {
        this.IsDebuggerSensitive = true;
        this.IsDebug = false;
    }

    public bool IsDebug { get; set; }

    public bool IsDebuggerSensitive { get; set; }
}
