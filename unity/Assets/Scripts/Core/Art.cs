using System.Collections.Generic;
using UnityEngine;

namespace Tapestry
{
    /// <summary>Loads sprite sheets from Resources/Art and slices them into frames.</summary>
    public static class Art
    {
        public const float DesignPerTile = 48f;    // design units in one world unit (one tile)
        static readonly Dictionary<string, Sprite[]> frames = new Dictionary<string, Sprite[]>();
        static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static ArtCatalog.Sheet Sheet(string key)
        {
            ArtCatalog.Sheet s;
            if (!ArtCatalog.Sheets.TryGetValue(key, out s))
                Debug.LogError("Tapestry: no art sheet called " + key);
            return s;
        }

        public static Texture2D Texture(string key)
        {
            Texture2D t;
            if (textures.TryGetValue(key, out t)) return t;
            t = Resources.Load<Texture2D>(Sheet(key).Path);
            if (t == null) Debug.LogError("Tapestry: missing texture Resources/" + Sheet(key).Path);
            else { t.filterMode = FilterMode.Bilinear; t.wrapMode = TextureWrapMode.Clamp; }
            textures[key] = t;
            return t;
        }

        /// <summary>All frames of a sheet; pivot at the bottom-centre of each frame.</summary>
        public static Sprite[] Frames(string key)
        {
            Sprite[] f;
            if (frames.TryGetValue(key, out f)) return f;
            var sheet = Sheet(key);
            var tex = Texture(key);
            int n = Mathf.Max(1, sheet.Frames);
            f = new Sprite[n];
            if (tex != null)
            {
                // Work from the real texture size so import rescaling can never misalign frames.
                float fw = tex.width / (float)n;
                float ppu = tex.height / (sheet.FrameH / DesignPerTile);
                for (int i = 0; i < n; i++)
                    f[i] = Sprite.Create(tex, new Rect(i * fw, 0, fw, tex.height), new Vector2(0.5f, 0f), ppu, 0, SpriteMeshType.FullRect);
            }
            frames[key] = f;
            return f;
        }

        public static Sprite Single(string key) { return Frames(key)[0]; }

        /// <summary>Make a GameObject that shows one sprite.</summary>
        public static SpriteRenderer Make(string key, Transform parent, Vector2 pos, int order, int frame = 0)
        {
            var go = new GameObject(key);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Frames(key)[frame];
            sr.sortingOrder = order;
            return sr;
        }
    }

    /// <summary>A sprite that sits on a physics body's bottom-centre, flips to face left/right,
    /// and is nudged so the drawing's visible content (not its padded frame) is what lines up.</summary>
    public class SpriteView
    {
        public readonly GameObject Root;
        public readonly SpriteRenderer Renderer;
        Sprite[] frames;
        ArtCatalog.Sheet sheet;
        public bool FacingRight = true;
        public string Key { get; private set; }

        public SpriteView(string key, Transform parent, int order)
        {
            Root = new GameObject(key);
            Root.transform.SetParent(parent, false);
            var child = new GameObject("sprite");
            child.transform.SetParent(Root.transform, false);
            Renderer = child.AddComponent<SpriteRenderer>();
            Renderer.sortingOrder = order;
            SetSheet(key);
        }

        public void SetSheet(string key)
        {
            if (key == Key) return;
            Key = key;
            sheet = Art.Sheet(key);
            frames = Art.Frames(key);
            SetFrame(0);
            Place();
        }

        public int FrameCount { get { return frames.Length; } }

        public void SetFrame(int i)
        {
            if (frames.Length == 0) return;
            Renderer.sprite = frames[Mathf.Clamp(i, 0, frames.Length - 1)];
        }

        public void Face(bool right)
        {
            if (right == FacingRight) return;
            FacingRight = right;
            Place();
        }

        Vector2 baseLocal;
        float tilt, tiltCentreY;

        void Place()
        {
            Renderer.flipX = !FacingRight;
            float cx = (sheet.MinX + sheet.MaxX) * 0.5f - sheet.FrameW * 0.5f;
            float bottomPad = sheet.FrameH - sheet.MaxY;
            baseLocal = new Vector2((FacingRight ? -cx : cx) / Art.DesignPerTile, -bottomPad / Art.DesignPerTile);
            ApplyTilt();
        }

        /// <summary>Turn the drawing by `degrees` about its own middle, and set that middle at height `centreY`
        /// above the body's feet (e.g. a knight lying flat to swim). Zero degrees puts it back upright.</summary>
        public void SetTilt(float degrees, float centreY)
        {
            tilt = degrees; tiltCentreY = centreY;
            ApplyTilt();
        }

        void ApplyTilt()
        {
            var t = Renderer.transform;
            if (tilt == 0f) { t.localPosition = new Vector3(baseLocal.x, baseLocal.y, 0f); t.localEulerAngles = Vector3.zero; return; }
            // the drawing's visible middle, measured up from its own feet
            float hc = (sheet.FrameH - (sheet.MinY + sheet.MaxY) * 0.5f) / Art.DesignPerTile;
            float a = tilt * Mathf.Deg2Rad, c = Mathf.Cos(a), s = Mathf.Sin(a);
            var v = new Vector2(baseLocal.x, -hc);   // from the middle back to the sprite's pivot
            var r = new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
            t.localPosition = new Vector3(r.x, tiltCentreY + r.y, 0f);
            t.localEulerAngles = new Vector3(0f, 0f, tilt);
        }

        public void SetPosition(Vector2 p) { Root.transform.localPosition = new Vector3(p.x, p.y, 0f); }
        public void Destroy() { Object.Destroy(Root); }
    }
}
