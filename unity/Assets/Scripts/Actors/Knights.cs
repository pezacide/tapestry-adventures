namespace Tapestry
{
    /// <summary>The four playable knights and how they handle.</summary>
    public class KnightDef
    {
        public string Key, Title, Trait;
        public float Walk, Run, Jump;
        public bool Glide;      // Azura: hold jump while falling to drift down
        public int Hearts = 3;  // Verdain is sturdier

        public string Sheet { get { return "heroes/" + Key; } }
        public string MountedSheet { get { return "heroes/" + Key + "_mounted"; } }
    }

    public static class Knights
    {
        public static readonly KnightDef[] All =
        {
            new KnightDef { Key = "rubeus",  Title = "Sir Rubeus",   Trait = "Stedfaste in alle thynges. He beareth ye speare.",                 Walk = 5.5f, Run = 9.0f,  Jump = 14.8f },
            new KnightDef { Key = "azura",   Title = "Dame Azura",   Trait = "Holde JUMPE whyle she falleth, and she shall glyde.",           Walk = 5.5f, Run = 8.6f,  Jump = 14.5f, Glide = true },
            new KnightDef { Key = "verdain", Title = "Sir Verdain",  Trait = "Stoute of herte: he hath foure hertes, not three.",      Walk = 5.0f, Run = 8.2f,  Jump = 14.3f, Hearts = 4 },
            new KnightDef { Key = "aurelia", Title = "Lady Aurelia", Trait = "Swifte of foote, yet lyghter of leape.",      Walk = 6.5f, Run = 10.6f, Jump = 14.2f },
        };
    }
}
