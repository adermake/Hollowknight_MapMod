using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Randomizer_Map
{
    public static class SpriteLoader
    {
        public static Dictionary<string, Sprite> Sprites;

        public static Sprite GetSprite(string spriteName)
        {
            return Sprites[spriteName];
        }

        public static void LoadSprites()
        {
            Sprites = new Dictionary<string, Sprite>();
            foreach (string resource in typeof(SpriteLoader).Assembly.GetManifestResourceNames().Where(name => name.ToLower().EndsWith(".png")))
            {
                try
                {
                    using Stream stream = typeof(SpriteLoader).Assembly.GetManifestResourceStream(resource);
                    if (stream == null)
                    {
                        continue;
                    }

                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    // Create texture from bytes
                    Texture2D tex = new(1, 1, TextureFormat.RGBA32, false);
                    tex.LoadImage(buffer, true);
                    tex.filterMode = FilterMode.Point;

                    string resName = Path.GetFileNameWithoutExtension(resource);
                    string[] pieces = resName.Split('.');
                    resName = pieces[pieces.Length - 1];

                    // Create sprite from texture
                    Sprites.Add(resName,
                        Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));
                }
                catch (Exception e)
                {
                   
                }
            }
        }

    }
}
