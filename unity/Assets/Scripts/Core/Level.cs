using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    public struct Decor
    {
        public string Key; public float X;
        public Decor(string key, float x) { Key = key; X = x; }
    }

    /// <summary>The tile grid: builds the stitched tiles, answers collision queries, and handles block bumps.
    /// One world unit = one 48-design-unit tile. Tile (x, y) covers [x, x+1] x [y, y+1]; y = 0 is the bottom row.
    ///
    /// Map legend:  , brick that gives many bezants if bumped again and again (up to ten in five seconds)
    ///              . hidden block (cannot be seen or touched until a knight bumps it from beneath; it hides a golden chalice)
    ///              # earth  B brick (breaks)  ? brick hiding a bezant  H brick hiding Bayard  X used block  = plank (one-way)
    ///              1 brick hiding a golden chalice (life)  M roast capon (heart)  W crossbowe  R holy relique (star)
    ///              * fire pit (burns)  _ drawbridge (falls when the axe is struck)  F flail block (iron flail swings round it)
    ///              A the axe that drops the bridge  K the Cockatrice  J the Island-Whale  N the rescued monk  Q log pile (rolls logs)
    ///              g gull  x crab  f fish  z sea-serpent  m sea-monk  e hippocamp  Y longship rocking at anchor  U longship sailing to and fro
    ///              < wind-head blowing left  > wind-head blowing right  b bird of the air
    ///              v stampede (herds of stags charge in from the right)  Z turning wheel (4 boards ride round it)  4 Ye Foure Windes
    ///              j wild boar  i otter  n swan  a falcon  r raft launch (rafts drift right on the current)  5 Ye Greate Boar
    ///              D boss gate (solid until the boss is beaten, then it crumbles)
    ///              ; poisoned stream (hurts until purified; then the knight wades it)  8 brick hiding ye unicorne (the second mount)
    ///              ( gold mirror pane  ) silver mirror pane (only one set is solid at a time)  7 mirror block (bump: they change places)
    ///              % basket of flowers (springs thee high)  9 organ pipe (rises and falls with the musick)  $ Ye Lyon and ye Unicorne
    ///              { lapdog  } fox  @ monkey (flings comfits)  ! parakeet
    ///              y huntsman (hurls spears)  q greyhound  p pheasant  I genet  0 ye fountaine  + ye unicorne's horne  6 Ye Spotted Unicorne
    ///              V vault stone (unbreakable)  L lift (glides sideways)  E lift (rises and falls)
    ///              ^ treetop (one-way)  | tree trunk (scenery)  T/t tower top/body (2 wide)  ~ sea  o bezant  P start  G goal pole
    ///              s snail-hound (red shell, kick)  S mossy snail-hound (green shell, carry & throw)  h killer hare  l fiery lion  d bayeux hound  c camel-dragon
    ///              w wyvern  u unicorn  O man-faced owl  k soup cat</summary>
    public class Level
    {
        public readonly int Width, Height;
        readonly char[,] tiles;
        readonly Dictionary<int, SpriteRenderer> views = new Dictionary<int, SpriteRenderer>();
        readonly Transform root;
        readonly List<Bump> bumps = new List<Bump>();
        readonly Dictionary<int, float> multiStart = new Dictionary<int, float>();
        readonly Dictionary<int, int> multiLeft = new Dictionary<int, int>();

        public Vector2 Start = new Vector2(2.5f, 2f);
        public float GoalX = -1f;
        public readonly List<KeyValuePair<char, Vector2>> Spawns = new List<KeyValuePair<char, Vector2>>();

        class Bump { public SpriteRenderer View; public float T; public Vector3 Home; }

        readonly LevelDef skin;

        public Level(string[] map, Transform parent, LevelDef def = null)
        {
            root = parent;
            skin = def ?? new LevelDef();
            Height = map.Length;
            int w = 0;
            foreach (var r in map) w = Mathf.Max(w, r.Length);
            Width = w;
            tiles = new char[Width, Height];
            for (int row = 0; row < Height; row++)
            {
                int y = Height - 1 - row;
                for (int x = 0; x < Width; x++)
                {
                    char c = x < map[row].Length ? map[row][x] : ' ';
                    tiles[x, y] = ' ';
                    switch (c)
                    {
                        case '#': case 'B': case '?': case 'H': case 'X': case '=': case 'T': case 't': case '~':
                        case '1': case 'M': case 'W': case 'R': case '^': case '|': case 'V': case '*': case '_': case 'D': case ';': case '8': case '(': case ')': case '7': case '%':
                        case ',': case '.':
                            tiles[x, y] = c; break;
                        case 'F': tiles[x, y] = 'F'; Spawns.Add(new KeyValuePair<char, Vector2>(c, new Vector2(x + 0.5f, y + 0.5f))); break;
                        case 'P': Start = new Vector2(x + 0.5f, y); break;
                        case 'G': GoalX = x + 0.5f; Spawns.Add(new KeyValuePair<char, Vector2>(c, new Vector2(x + 0.5f, y))); break;
                        case ' ': break;
                        default: Spawns.Add(new KeyValuePair<char, Vector2>(c, new Vector2(x + 0.5f, y))); break;
                    }
                }
            }
            ShuffleRewards();
            BuildViews();
            ShadePanes();
        }

        /// <summary>Every play, the prizes in the reward bricks are shuffled among the bricks (so no two plays are alike):
        /// bezants, Bayard, the unicorn, the capon, the crossbowe and the chalice change places. The holy relique stays in its
        /// own brick (the beasts that wait after it are set for it), and the chalice is kept out of the last third of the level
        /// and well away from the relique, as it was laid out.</summary>
        void ShuffleRewards()
        {
            var spots = new List<Vector2Int>(); var prizes = new List<char>(); var relics = new List<int>();
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    char c = tiles[x, y];
                    if (c == 'R') relics.Add(x);
                    else if (c == '?' || c == 'H' || c == '1' || c == 'M' || c == 'W' || c == '8') { spots.Add(new Vector2Int(x, y)); prizes.Add(c); }
                }
            if (spots.Count < 2) return;
            var rng = new System.Random();
            for (int attempt = 0; attempt < 40; attempt++)
            {
                for (int i = prizes.Count - 1; i > 0; i--)   // Fisher-Yates
                {
                    int j = rng.Next(i + 1);
                    char t = prizes[i]; prizes[i] = prizes[j]; prizes[j] = t;
                }
                bool ok = true;
                for (int i = 0; i < spots.Count && ok; i++)
                {
                    if (prizes[i] != '1') continue;
                    int x = spots[i].x;
                    if (x > Width * 0.68f) ok = false;                              // not near the end
                    foreach (int rx in relics) if (Mathf.Abs(x - rx) < 20) ok = false;   // nor beside the relique
                }
                if (ok) break;
                if (attempt == 39) return;   // no fair shuffle found: keep the laid-out order
            }
            for (int i = 0; i < spots.Count; i++) tiles[spots[i].x, spots[i].y] = prizes[i];
        }


        static int Key(int x, int y) { return x * 1024 + y; }

        public char Get(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return ' ';
            return tiles[x, y];
        }

        /// <summary>Solid for walls/floors/ceilings. The level edges are walls; below the map is open (pits).</summary>
        public bool Solid(int x, int y)
        {
            if (x < 0 || x >= Width) return true;
            switch (Get(x, y))
            {
                case '#': case 'B': case '?': case 'H': case 'X': case 'T': case 't':
                case '1': case 'M': case 'W': case 'R': case 'V': case '_': case 'F': case 'D': case '8': case '7': case '%': case ',': return true;
                case '(': return MirrorState == 0;
                case ')': return MirrorState == 1;
                default: return false;
            }
        }

        /// <summary>A hidden chalice block: no collision, no picture, until a knight bumps it from below.</summary>
        public bool Hidden(int x, int y) { return Get(x, y) == '.'; }

        public bool Platform(int x, int y) { char c = Get(x, y); return c == '=' || c == '^'; }
        public bool Water(int x, int y) { return Get(x, y) == '~'; }
        public bool Fire(int x, int y) { return Get(x, y) == '*'; }
        public bool Basket(int x, int y) { return Get(x, y) == '%'; }

        /// <summary>World 6-1 (Sight): the gold panes are solid while MirrorState is 0, the silver ones while it is 1.
        /// The pane-set that is not solid is drawn faint, like a reflection.</summary>
        public int MirrorState { get; private set; }
        readonly List<KeyValuePair<char, SpriteRenderer>> panes = new List<KeyValuePair<char, SpriteRenderer>>();
        public void FlipMirrors()
        {
            MirrorState = 1 - MirrorState;
            ShadePanes();
        }
        void ShadePanes()
        {
            foreach (var p in panes)
                if (p.Value != null) p.Value.color = new Color(1f, 1f, 1f, (p.Key == '(') == (MirrorState == 0) ? 1f : 0.22f);
        }

        /// <summary>World 5: the poisoned stream hurts until ye unicorne's horne is brought to ye fountaine.</summary>
        public bool Purified { get; private set; }
        public bool Foul(int x, int y) { return !Purified && Get(x, y) == ';'; }
        public void Purify()
        {
            if (Purified) return;
            Purified = true;
            var pure = Art.Single("scenery/stream_pure");
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    SpriteRenderer v;
                    if (tiles[x, y] == ';' && views.TryGetValue(Key(x, y), out v) && v != null) v.sprite = pure;
                }
        }

        readonly List<SpriteRenderer> fires = new List<SpriteRenderer>();
        float fireT;

        /// <summary>Every drawbridge tile, nearest the axe (rightmost) first, for collapsing.</summary>
        public List<Vector2Int> BridgeTiles()
        {
            var list = new List<Vector2Int>();
            for (int x = Width - 1; x >= 0; x--)
                for (int y = 0; y < Height; y++)
                    if (tiles[x, y] == '_') list.Add(new Vector2Int(x, y));
            return list;
        }

        /// <summary>Crumble every boss-gate tile (once the boss is beaten). Returns how many fell.</summary>
        public int OpenGates()
        {
            int n = 0;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (tiles[x, y] == 'D') { RemoveTile(x, y); n++; }
            return n;
        }

        /// <summary>Splinter the boss gate (the unicorn rams it): knock out its top tiles, keeping only `fractionLeft` of them.
        /// Returns the tiles that fell.</summary>
        public List<Vector2Int> CrackGate(float fractionLeft)
        {
            var gate = new List<Vector2Int>();
            for (int x = 0; x < Width; x++)
                for (int y = Height - 1; y >= 0; y--)
                    if (tiles[x, y] == 'D') gate.Add(new Vector2Int(x, y));
            gate.Sort((a, b) => b.y.CompareTo(a.y));   // top first
            int keep = Mathf.CeilToInt(gate.Count * Mathf.Clamp01(fractionLeft) - 0.01f);
            var fell = gate.GetRange(0, gate.Count - keep);
            foreach (var t in fell) RemoveTile(t.x, t.y);
            return fell;
        }

        public void RemoveTile(int x, int y)
        {
            tiles[x, y] = ' ';
            SpriteRenderer v;
            if (views.TryGetValue(Key(x, y), out v)) { if (v != null) Object.Destroy(v.gameObject); views.Remove(Key(x, y)); }
        }

        /// <summary>Height of the top surface in a column, scanning down from the top of the map.</summary>
        public float GroundAt(float x)
        {
            int tx = Mathf.Clamp(Mathf.FloorToInt(x), 0, Width - 1);
            for (int y = Height - 1; y >= 0; y--)
                if (Solid(tx, y)) return y + 1;
            return 0f;
        }

        void BuildViews()
        {
            var towerDrawn = new bool[Width, Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    char c = tiles[x, y];
                    string key = null; float dy = 0f;
                    switch (c)
                    {
                        case '#': key = Get(x, y + 1) == '#' ? skin.GroundFill : skin.GroundTop; break;
                        case 'B': case '?': case 'H': case '1': case 'M': case 'W': case 'R': case '8': case ',': key = skin.Brick; break;   // item bricks look like any other brick
                        case 'X': key = "scenery/roundel_used"; break;
                        case '~': key = "scenery/sea"; break;
                        case '=': key = skin.Plank; dy = 1f - Art.Sheet(skin.Plank).FrameH / Art.DesignPerTile; break;
                        case '^': key = "scenery/canopy"; dy = 1f - 26f / Art.DesignPerTile; break;
                        case '|': key = "scenery/trunk"; break;
                        case 'V': case 'F': key = skin.Vault; break;
                        case 'D': key = string.IsNullOrEmpty(skin.Gate) ? skin.Brick : skin.Gate; break;
                        case ';': key = "scenery/stream_foul"; break;
                        case '(': key = "scenery/mirror_gold"; break;
                        case ')': key = "scenery/mirror_silver"; break;
                        case '7': key = "scenery/mirror_block"; break;
                        case '%': key = "scenery/basket"; break;
                        case '*': key = "scenery/fire"; break;
                        case '_': key = "scenery/drawbridge"; dy = 1f - 26f / Art.DesignPerTile; break;
                        case 'T':
                        case 't':
                            // towers are two tiles wide: draw one sprite per pair, centred between the two tiles
                            if (!towerDrawn[x, y])
                            {
                                towerDrawn[x, y] = true;
                                if (x + 1 < Width && tiles[x + 1, y] == c) towerDrawn[x + 1, y] = true;
                                views[Key(x, y)] = Art.Make(c == 'T' ? "scenery/tower_top" : "scenery/tower_body", root, new Vector2(x + 1f, y), 0);
                            }
                            break;
                    }
                    if (key != null)
                    {
                        views[Key(x, y)] = Art.Make(key, root, new Vector2(x + 0.5f, y + dy), c == '~' || c == ';' ? 6 : c == '*' ? 6 : c == '|' ? -6 : c == '^' ? 1 : 0, c == '*' ? (x + y) % 2 : 0);
                        if (c == '*') fires.Add(views[Key(x, y)]);
                        if (c == '(' || c == ')') panes.Add(new KeyValuePair<char, SpriteRenderer>(c, views[Key(x, y)]));
                    }
                }
        }

        public enum BumpResult { None, Bezant, Bayard, Unicorn, Mirror, Chalice, Capon, Crossbow, Relic, Broke, Thud }

        /// <summary>A knight's head hit tile (x, y) from below.</summary>
        public BumpResult HitFromBelow(int x, int y)
        {
            char c = Get(x, y);
            SpriteRenderer v;
            views.TryGetValue(Key(x, y), out v);
            switch (c)
            {
                case '?': case 'H': case '1': case 'M': case 'W': case 'R': case '8':
                    tiles[x, y] = 'X';   // an item brick gives its item and leaves a used block behind
                    if (v != null) v.sprite = Art.Single("scenery/roundel_used");
                    StartBump(v);
                    switch (c)
                    {
                        case 'H': return BumpResult.Bayard;
                        case '8': return BumpResult.Unicorn;
                        case '1': return BumpResult.Chalice;
                        case 'M': return BumpResult.Capon;
                        case 'W': return BumpResult.Crossbow;
                        case 'R': return BumpResult.Relic;
                        default: return BumpResult.Bezant;
                    }
                case ',':
                    {
                        // many-bezant brick: each bump pays a bezant until ten are given or five seconds have passed since the first
                        int k = Key(x, y);
                        float now = Time.time;
                        if (!multiStart.ContainsKey(k)) { multiStart[k] = now; multiLeft[k] = 10; }
                        StartBump(v);
                        multiLeft[k]--;
                        if (multiLeft[k] <= 0 || now - multiStart[k] > 5f)
                        {
                            tiles[x, y] = 'X';
                            if (v != null) v.sprite = Art.Single("scenery/roundel_used");
                            multiStart.Remove(k); multiLeft.Remove(k);
                        }
                        return BumpResult.Bezant;
                    }
                case '.':
                    {
                        // the hidden block shows itself, and gives its chalice
                        tiles[x, y] = 'X';
                        var nv = Art.Make("scenery/roundel_used", root, new Vector2(x + 0.5f, y), 0);
                        views[Key(x, y)] = nv;
                        StartBump(nv);
                        return BumpResult.Chalice;
                    }
                case '7': StartBump(v); return BumpResult.Mirror;
                case 'B':
                    tiles[x, y] = ' ';
                    if (v != null) { Object.Destroy(v.gameObject); views.Remove(Key(x, y)); }
                    return BumpResult.Broke;
                case 'X':
                case 'V':
                case 'F':
                    return BumpResult.Thud;
            }
            return BumpResult.None;
        }

        void StartBump(SpriteRenderer v)
        {
            if (v == null) return;
            foreach (var b in bumps) if (b.View == v) return;
            bumps.Add(new Bump { View = v, T = 0f, Home = v.transform.localPosition });
        }

        public void Tick(float dt)
        {
            fireT += dt;
            if (fireT > 0.16f)   // the flames flicker
            {
                fireT = 0f;
                var f = Art.Frames("scenery/fire");
                foreach (var v in fires) if (v != null) v.sprite = v.sprite == f[0] ? f[1] : f[0];
            }
            for (int i = bumps.Count - 1; i >= 0; i--)
            {
                var b = bumps[i];
                b.T += dt;
                float k = b.T / 0.18f;
                if (b.View == null || k >= 1f) { if (b.View != null) b.View.transform.localPosition = b.Home; bumps.RemoveAt(i); continue; }
                b.View.transform.localPosition = b.Home + new Vector3(0f, Mathf.Sin(k * Mathf.PI) * 0.25f, 0f);
            }
        }
    }

    /// <summary>Axis-aligned box that moves through the tile grid. Pos is the bottom-centre.</summary>
    public class Body
    {
        public Vector2 Pos, Vel;
        public float W, H;
        public bool UsePlatforms = true, Grounded, HitCeiling, HitWall;
        public bool HitsHidden;     // true for knights: hidden blocks stop their heads (and show themselves)
        public int CeilX, CeilY;
        public float PrevBottom;

        public Body(Vector2 pos, float w, float h) { Pos = pos; W = w; H = h; }

        public float Left { get { return Pos.x - W * 0.5f; } }
        public float Right { get { return Pos.x + W * 0.5f; } }
        public float Top { get { return Pos.y + H; } }
        public Rect Box { get { return new Rect(Left, Pos.y, W, H); } }

        public bool Overlaps(Body o)
        {
            return Left < o.Right && Right > o.Left && Pos.y < o.Top && Top > o.Pos.y;
        }

        public void Step(Level lv, float dt)
        {
            Grounded = HitCeiling = HitWall = false;
            PrevBottom = Pos.y;
            float dx = Vel.x * dt, dy = Vel.y * dt;
            // sub-step so fast things never tunnel through a tile
            int n = Mathf.Max(1, Mathf.CeilToInt(Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) / 0.4f));
            for (int i = 0; i < n; i++)
            {
                MoveX(lv, dx / n);
                MoveY(lv, dy / n);
            }
        }

        /// <summary>Push the body sideways through the grid (wind).</summary>
        public void Nudge(Level lv, float dx) { MoveX(lv, dx); }

        void MoveX(Level lv, float dx)
        {
            if (dx == 0f) return;
            Pos.x += dx;
            int y0 = Mathf.FloorToInt(Pos.y + 0.01f), y1 = Mathf.FloorToInt(Top - 0.01f);
            if (dx > 0f)
            {
                int tx = Mathf.FloorToInt(Right);
                for (int y = y0; y <= y1; y++)
                    if (lv.Solid(tx, y)) { Pos.x = tx - W * 0.5f - 0.001f; Vel.x = 0f; HitWall = true; return; }
            }
            else
            {
                int tx = Mathf.FloorToInt(Left);
                for (int y = y0; y <= y1; y++)
                    if (lv.Solid(tx, y)) { Pos.x = tx + 1 + W * 0.5f + 0.001f; Vel.x = 0f; HitWall = true; return; }
            }
        }

        void MoveY(Level lv, float dy)
        {
            if (dy == 0f) return;
            float before = Pos.y;
            Pos.y += dy;
            int x0 = Mathf.FloorToInt(Left + 0.01f), x1 = Mathf.FloorToInt(Right - 0.01f);
            if (dy < 0f)
            {
                int ty = Mathf.FloorToInt(Pos.y);
                for (int x = x0; x <= x1; x++)
                {
                    bool plat = UsePlatforms && lv.Platform(x, ty) && before >= ty + 1 - 0.02f;
                    if (lv.Solid(x, ty) || plat) { Pos.y = ty + 1; Vel.y = 0f; Grounded = true; return; }
                }
            }
            else
            {
                int ty = Mathf.FloorToInt(Top);
                int best = -1; float bestDist = 99f;
                for (int x = x0; x <= x1; x++)
                    if (lv.Solid(x, ty) || (HitsHidden && lv.Hidden(x, ty)))
                    {
                        float d = Mathf.Abs(x + 0.5f - Pos.x);
                        if (d < bestDist) { bestDist = d; best = x; }
                    }
                if (best >= 0)
                {
                    Pos.y = ty - H - 0.001f; Vel.y = 0f; HitCeiling = true; CeilX = best; CeilY = ty;
                }
            }
        }

        /// <summary>Is there floor just ahead of this body in the given direction? (for beasts that won't walk off ledges)</summary>
        public bool FloorAhead(Level lv, float dir)
        {
            int tx = Mathf.FloorToInt(Pos.x + dir * (W * 0.5f + 0.1f));
            int ty = Mathf.FloorToInt(Pos.y - 0.1f);
            return lv.Solid(tx, ty) || lv.Platform(tx, ty);
        }
    }
}
