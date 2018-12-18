using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Color;

namespace ColorPalette
{
    public static class ColorPalette
    {
        public static Texture2D GetPaletteSheet()
        {
            Texture2D palette = new Texture2D(60, 20, TextureFormat.RGBA32, false);
            palette.wrapMode = TextureWrapMode.Clamp;
            palette.filterMode = FilterMode.Point;

            Color testColor = GetNiceColor();

            // Fill sheet w/ palettes
            List<Color> colorz;
            for (int y=0; y < palette.height; y++)
            {
                //Color testColor = GetNiceColor();
                colorz = GetRandomColorPalette(UnityEngine.Random.Range(2, 5) + 1, testColor);
                int pixelsPerColor = palette.width / colorz.Count;
                int offset = 0;
                foreach (Color color in colorz)
                {
                    for (int x=0; x < pixelsPerColor; x++)
                    {
                        palette.SetPixel(x + (offset*pixelsPerColor), y, color);
                    }
                    offset += 1;
                }
            }
            palette.Apply();
            return palette;
        }

        public static Color GetNiceColor()
        {
            return UnityEngine.Random.ColorHSV(0.0f, 1.0f, .225f, .725f, .45f, .55f);
        }

        public static List<Color> GetRandomColorPalette() { return GetRandomColorPalette(UnityEngine.Random.Range(0, 5) + 1, GetNiceColor()); }
        public static List<Color> GetRandomColorPalette(int numColors) { return GetRandomColorPalette(numColors, GetNiceColor()); }
        public static List<Color> GetRandomColorPalette(Color color1) { return GetRandomColorPalette(UnityEngine.Random.Range(0, 5) + 1, color1); }
        public static List<Color> GetRandomColorPalette(int numColors, Color color1)
        {
            /// Returns a random color palette with the specified number of colors.
            List<Color> colors;

            // Get a list of functions which will return the # of colors we want.
            List<ColorSchemes.SchemeDelegate> options = ColorSchemes.GetValidSchemes(numColors);
            // Execute one of the functions at random.
            int optionToUse = UnityEngine.Random.Range(0, options.Count);
            //Debug.Log("using option " + optionToUse);
            colors = options[optionToUse](color1);

            for (int i = 0; i < colors.Count; i++)
            {
                // Magic numbers that make for nice variance
                colors[i] = _Color.ColorOps.HueJitter(colors[i], 22.5f);
                colors[i] = _Color.ColorOps.SaturationJitter(colors[i], .15f);
                colors[i] = _Color.ColorOps.ValueJitter(colors[i], .4f);
            }

            // TODO: Small chance that one of the colors get slammed to black/white? Slam whichever is closest to those anyway?
            // TODO: Or slam whichever color is bad--close to grey?
            return colors;
        }


    }



}
