namespace TIKSN.Versioning
{
    public enum Milestone
    {
        // Unstable
        PreAlpha = 0,

        Alpha = 1,
        Beta = 2,
        ReleaseCandidate = 3,

        // Stable
        RTM = 4, // Release to manufacturing OR Release to marketing

        GA = 5, // General availability
    }
}