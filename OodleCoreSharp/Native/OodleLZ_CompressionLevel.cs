namespace OodleCoreSharp
{
    public enum OodleLZ_CompressionLevel : int
    {
        None = 0,
        SuperFast = 1,
        VeryFast = 2,
        Fast = 3,
        Normal = 4,

        Optimal1 = 5,
        Optimal2 = 6,
        Optimal3 = 7,
        Optimal4 = 8,
        Optimal5 = 9,

        HyperFast1 = -1,
        HyperFast2 = -2,
        HyperFast3 = -3,
        HyperFast4 = -4,

        HyperFast = HyperFast1,
        Optimal = Optimal2,
        Max = Optimal5,
        Min = HyperFast4
    }
}
