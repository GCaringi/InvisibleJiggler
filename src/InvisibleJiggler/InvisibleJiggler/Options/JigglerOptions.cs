namespace InvisibleJiggler.Options
{
    public record JigglerOptions
    {
        public int MinIntervalMs { get; init; } = 500;
        public int MaxIntervalMs { get; init; } = 2000;
        public int MinDistance { get; init; } = 1;
        public int MaxDistance { get; init; } = 15;
    }
}
