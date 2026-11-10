namespace Tapestry
{
    /// <summary>Everything that makes one level: its map, look, music and the building at its end.</summary>
    public class LevelDef
    {
        public string Title;          // "Worlde 1-1"
        public string Name;           // "Ye Meadowe of Beastes"
        public string Inscription;
        public string[] Map;          // ASCII, top row first (legend in Level.cs)
        public Decor[] Props;         // background props on the ground
        public string Castle;         // end building sprite
        public float CastleX;         // its doorway, in tiles
        public string Music;          // Resources/Audio/music/<name>
        public string FarLayer = "backgrounds/far", NearLayer = "backgrounds/near";
        public bool Entrance;         // a short lead-in: no pole; walking into the building goes straight on to the next part
    }

    public static class Levels
    {
        /// <summary>The levels in order. Add new ones here.</summary>
        public static readonly LevelDef[] All = { Level1.Def, Level2.Def, Level3a.Def, Level3.Def };
    }
}
