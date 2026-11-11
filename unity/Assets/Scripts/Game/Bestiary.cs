using System.Collections.Generic;

namespace Tapestry
{
    /// <summary>What the briefing at the start of each level shows: every beast, hazard and boss, with a picture and a word of counsel.</summary>
    public static class Bestiary
    {
        public class Entry
        {
            public string Sprite, Name, Tip;
            public Entry(string sprite, string name, string tip) { Sprite = sprite; Name = name; Tip = tip; }
        }

        /// <summary>Beasts and hazards, by their map character (see the legend in Level.cs).</summary>
        static readonly Dictionary<char, Entry> ByChar = new Dictionary<char, Entry>
        {
            { 's', new Entry("enemies/snail_hound", "Snayle-Hounde", "Stompe it, then kicke its shelle into other beastes.") },
            { 'S', new Entry("enemies/snail_moss", "Mossy Snayle-Hounde", "Stompe it, then holde RUNNE to lifte ye shelle and throwe it.") },
            { 'h', new Entry("enemies/killer_hare", "Killer Hare", "It leapeth at thee. Stompe it as it landeth.") },
            { 'l', new Entry("enemies/fiery_lion", "Fiery Lyon", "It spitteth fyre, and its mane burneth. Shoote it, or trample it upon thy steede.") },
            { 'd', new Entry("enemies/bayeux_hound", "Bayeux Hounde", "Swifte, and it leapeth gappes. Stompe it.") },
            { 'c', new Entry("enemies/camel_dragon", "Camel-Dragoun", "Slowe and stoute. Stompe it.") },
            { 'w', new Entry("enemies/wyvern", "Wyverne", "It hovereth to and fro. Stompe it from above.") },
            { 'u', new Entry("enemies/unicorn", "Charging Unicorne", "It chargeth when it seeth thee. Leape it, then stompe it.") },
            { 'O', new Entry("enemies/owl_face", "Man-faced Owle", "It swoopeth on thee as thou passest beneath.") },
            { 'k', new Entry("enemies/soup_cat", "Soupe Catte", "It flingeth hotte soupe. Stompe it.") },
            { 'g', new Entry("enemies/gull", "Gulle", "It wheeleth above, then diveth at thee.") },
            { 'x', new Entry("enemies/crab", "Crabbe", "It hoppeth to and fro. Stompe it.") },
            { 'f', new Entry("enemies/fish", "Bayeux Fysshe", "Beneath ye waves no beaste may be stomped. Swimme rounde, or shoote.") },
            { 'z', new Entry("enemies/sea_serpent", "Sea-Serpent", "It coileth up and downe. Keepe cleare of it.") },
            { 'm', new Entry("enemies/sea_monk", "Sea-Monke", "It pulseth towarde thee. Swimme awaye.") },
            { 'e', new Entry("enemies/hippocamp", "Hippocampe", "When it reareth and flasheth, it dasheth: move from its path.") },
            { 'b', new Entry("enemies/bird", "Byrde of ye Ayre", "It flyeth straighte at thee. Stompe it, or leape over.") },
            { 'v', new Entry("enemies/stag", "Stampede of Stagges", "Herdes thunder in from ye ryght. Leape them, or wayte upon ye plankes.") },
            { 'j', new Entry("enemies/boar", "Wilde Boare", "It chargeth when it seeth thee. Leape it, then stompe it.") },
            { 'i', new Entry("enemies/otter", "Otter", "It leapeth from ye river as thou passest.") },
            { 'n', new Entry("enemies/swan", "Swanne", "It chargeth along ye water. Stompe it.") },
            { 'a', new Entry("enemies/falcon", "Falcon", "It wheeleth high, then stoopeth on thee.") },
            { 'y', new Entry("enemies/hunter", "Huntsman", "He hurleth speares in long arcs. Stompe him.") },
            { 'q', new Entry("enemies/greyhound", "Greyhounde", "It lyeth in wayte, then raceth at thee.") },
            { 'p', new Entry("enemies/pheasant", "Pheasaunt", "It bursteth up from ye flowers as thou drawest neare.") },
            { 'I', new Entry("enemies/genet", "Genette", "It pounceth in long lowe leapes.") },
            { 'Q', new Entry("scenery/log", "Rolling Logges", "Leape them, or stompe them awaye.") },
            { '*', new Entry("scenery/fire", "Fyre-pittes", "They burne. Leape them.") },
            { 'F', new Entry("scenery/flail_ball", "Iron Flailes", "They swinge rounde. Tyme thy passing.") },
            { ';', new Entry("scenery/stream_foul", "Poysoned Streame", "It hurteth. Beare ye unicorne's horne to ye fountaine, and it runneth pure.") },
            { '<', new Entry("scenery/wind_head", "Wynde-Heddes", "They blowe thee aboute. Leane into ye wynde.") },
            { 'Z', new Entry("scenery/wheel_year", "Wheeles of ye Yeare", "Ryde ye boardes rounde, over ye fyre.") },
            { 'r', new Entry("scenery/raft", "Raftes", "They drifte downe ye river. Ryde them across.") },
            { '_', new Entry("scenery/bridge_axe", "Ye Axe", "Strike it, and ye bridge falleth.") },
        };

        /// <summary>Bosses, by their map character: how to vanquish each one.</summary>
        static readonly Dictionary<char, Entry> Bosses = new Dictionary<char, Entry>
        {
            { 'K', new Entry("bosses/cockatrice", "Ye Cockatrice", "Stompe its crowned hedde thrice, or strike ye axe to drop ye bridge. Beware its stony gaze.") },
            { 'J', new Entry("bosses/whale_boss", "Aspidochelone, ye Island-Whale", "When it surfaceth, strike its eye: stompe it or shoote it, three tymes.") },
            { '4', new Entry("scenery/wind_head", "Ye Foure Windes", "Each hedde in turne draweth breath and gloweth golde: touche it or shoote it then.") },
            { '5', new Entry("bosses/great_boar", "Ye Greate Boar", "Leape it, that it ramme ye wall; then stompe it whyle it is dazed. Three blowes.") },
            { '6', new Entry("bosses/spotted_unicorn", "Ye Spotted Unicorne", "It may not be harmed. Leape it, that it ramme ye GATE thrice: then it is spared, and shall beare thee.") },
        };

        /// <summary>Everything a knight should know before this level: beasts and hazards in the order he meets them, and its boss.</summary>
        public static List<Entry> For(LevelDef def, out Entry boss)
        {
            boss = null;
            var list = new List<Entry>();
            var seen = new HashSet<string>();
            if (def.HuntingHounds) Add(list, seen, new Entry("enemies/talbot", "Talbot Houndes", "Ye hunte's houndes runne in from behinde. Keepe moving, or stompe them."));
            if (def.Chase) Add(list, seen, new Entry("scenery/hunt_line", "Ye Hunte", "It sweepeth on from behinde and never falleth farre backe. Runne!"));
            if (def.Wind) Add(list, seen, new Entry("scenery/gust", "Gustes of Wynde", "Ye wynde bloweth by turnes, from before and behinde."));
            if (def.LeapingFish) Add(list, seen, new Entry("enemies/fish", "Leaping Fysshe", "They burst from ye sea in arcs. Stompe them in ye ayre."));
            int w = 0;
            foreach (var row in def.Map) if (row.Length > w) w = row.Length;
            for (int x = 0; x < w; x++)
                foreach (var row in def.Map)
                {
                    if (x >= row.Length) continue;
                    char c = row[x];
                    if (c == '>') c = '<';
                    Entry e;
                    if (Bosses.TryGetValue(c, out e)) { boss = e; continue; }
                    if (c == '~' && !def.Underwater) { Add(list, seen, new Entry("scenery/sea", "Ye Sea", "Falle in, and thou art loste.")); continue; }
                    if (ByChar.TryGetValue(c, out e)) Add(list, seen, e);
                }
            return list;
        }

        static void Add(List<Entry> list, HashSet<string> seen, Entry e)
        {
            if (seen.Add(e.Name)) list.Add(e);
        }
    }
}
