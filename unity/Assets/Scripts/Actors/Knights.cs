namespace Tapestry
{
    /// <summary>The four playable knights and how they handle.</summary>
    public class KnightDef
    {
        public string Key, Title, Trait;
        public float Walk, Run, Jump;
        public bool Glide;      // Azura: hold jump while falling to drift down
        public bool Breaker;    // Verdain: his axe breaks masonry from below

        public string Sheet { get { return "heroes/" + Key; } }
        public string MountedSheet { get { return "heroes/" + Key + "_mounted"; } }
    }

    public static class Knights
    {
        public static readonly KnightDef[] All =
        {
            new KnightDef { Key = "rubeus",  Title = "Sir Rubeus",   Trait = "Balanced. The spear-bearer.",                 Walk = 5.5f, Run = 9.0f,  Jump = 14.8f },
            new KnightDef { Key = "azura",   Title = "Dame Azura",   Trait = "Hold JUMP while falling to glide.",           Walk = 5.5f, Run = 8.6f,  Jump = 14.5f, Glide = true },
            new KnightDef { Key = "verdain", Title = "Sir Verdain",  Trait = "Strong. His axe breaks masonry.",             Walk = 5.0f, Run = 8.2f,  Jump = 14.3f, Breaker = true },
            new KnightDef { Key = "aurelia", Title = "Lady Aurelia", Trait = "Swift. Fastest on foot, lighter jumps.",      Walk = 6.5f, Run = 10.6f, Jump = 14.2f },
        };
    }
}
