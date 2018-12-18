using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// Lib for doing color stuff!
/// 

namespace _Color
{
    public enum ColorSpace { rgb, ryb };  // Colors are always expressed as RGB, but this affects what colors are considered complementary.
    public enum ColorMixMode { additive, subtractive };

    public static class ColorOps
    {
        public static Color randomColor()
        {
            /// Returns a random color.
            return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }


        public static Color invertColor(Color in_color)
        {
            /// Given a color, returns its inverse.
            return new Color(1 - in_color.r, 1 - in_color.g, 1 - in_color.b);
        }

        public static Vector4 RGBtoCMYK(Color in_color)
        {
            /// Returns the CMYK representation of an RGB color.
            float k = 1 - Mathf.Max(in_color.r, in_color.g, in_color.b);
            float c = (1 - in_color.r - k) / (1 - k);
            float m = (1 - in_color.g - k) / (1 - k);
            float y = (1 - in_color.b - k) / (1 - k);
            return new Vector4(c, m, y, k);
        }

        public static Color RGBtoRYB(Color in_color)
        {
            /// Given an RGB color, returns its RYB equivalent. From http://www.deathbysoftware.com/colors/index.html
            /// We return a Color object to allow other ops (eg. hue shifting). Obviously, a Color holding RYB values will not render correctly.
            /// 
            float red, green, blue;
            red = in_color.r;
            green = in_color.g;
            blue = in_color.b;

            // Strip white out
            float white = Mathf.Min(red, green, blue);

            red -= white;
            green -= white;
            blue -= white;

            float maxGreen = Mathf.Max(red, green, blue);

            // Get the yellow out of the red+green

            float yellow = Mathf.Min(red, green);

            red -= yellow;
            green -= yellow;

            // If this unfortunate conversion combines blue and green, then cut each in half to
            // preserve the value's maximum range.
            if (blue > 0 && green > 0)
            {
                blue /= 2.0f;
                green /= 2.0f;
            }

            // Redistribute the remaining green.
            yellow += green;
            blue += green;

            // Normalize to values.
            float maxYellow = Mathf.Max(red, yellow, blue);

            if (maxYellow > 0)
            {
                float iN = maxGreen / maxYellow;

                red *= iN;
                yellow *= iN;
                blue *= iN;
            }

            // Add the white back in.
            red += white;
            yellow += white;
            blue += white;

            return new Color(red, yellow, blue);
        }

        public static Color RYBtoRGB(Color in_color)
        {
            /// Converts an RYB color back to RGB. From http://www.deathbysoftware.com/colors/index.html
            ///
            float red, yellow, blue;
            red = in_color.r;
            yellow = in_color.g;
            blue = in_color.b;

            // Remove the whiteness from the color.
            float white = Mathf.Min(red, yellow, blue);

            red -= white;
            yellow -= white;
            blue -= white;

            float maxYellow = Mathf.Max(red, yellow, blue);

            // Get the green out of the yellow and blue
            float green = Mathf.Min(yellow, blue);

            yellow -= green;
            blue -= green;

            if (blue > 0 && blue > 0)
            {
                blue *= 2.0f;
                green *= 2.0f;
            }

            // Redistribute the remaining yellow.
            red += yellow;
            green += yellow;

            // Normalize to values.
            float maxGreen = Mathf.Max(red, green, blue);

            if (maxGreen > 0)
            {
                var iN = maxYellow / maxGreen;

                red *= iN;
                green *= iN;
                blue *= iN;
            }

            // Add the white back in.
            red += white;
            green += white;
            blue += white;

            return new Color(red, green, blue);
        }

        public static Color HueShift(Color inColor, float angle)
        {
            /// Shifts the provided color's hue by the provided angle.
            float h, s, v;
            Color.RGBToHSV(inColor, out h, out s, out v);

            h = (h + (angle / 360.0f)) % 1.0f;

            return Color.HSVToRGB(h, s, v);
        }

        public static Color SaturationShift(Color inColor, float value)
        {
            /// Shifts the provided color's saturation by the provided value.
            float h, s, v;
            Color.RGBToHSV(inColor, out h, out s, out v);

            s = (s + value);
            s = Mathf.Clamp(s, 0f, 1.0f);

            return Color.HSVToRGB(h, s, v);
        }

        public static Color ValueShift(Color inColor, float value)
        {
            /// Shifts the provided color's value by the provided value.
            float h, s, v;
            Color.RGBToHSV(inColor, out h, out s, out v);

            v = (v + value);
            v = Mathf.Clamp(v, 0f, 1.0f);

            return Color.HSVToRGB(h, s, v);
        }

        public static Color HueJitter(Color inColor, float angle)
        {
            float toShift = UnityEngine.Random.Range(-angle, angle);
            return HueShift(inColor, toShift);
        }

        public static Color SaturationClamp(Color inColor, float val) { return SaturationClamp(inColor, val, val); }
        public static Color SaturationClamp(Color inColor, float min, float max)
        {
            /// Clamps the provided color's saturation between the provided values.
            float h, s, v;
            Color.RGBToHSV(inColor, out h, out s, out v);
            s = Mathf.Clamp(s, min, max);
            return Color.HSVToRGB(h, s, v);
        }

        public static Color SaturationJitter(Color inColor, float range)
        {
            float toShift = UnityEngine.Random.Range(-range, range);
            return SaturationShift(inColor, toShift);
        }

        public static Color ValueClamp(Color inColor, float val) { return ValueClamp(inColor, val, val); }
        public static Color ValueClamp(Color inColor, float min, float max)
        {
            /// Clamps the provided color's value between the provided values.
            float h, s, v;
            Color.RGBToHSV(inColor, out h, out s, out v);
            v = Mathf.Clamp(v, min, max);
            return Color.HSVToRGB(h, s, v);
        }

        public static Color ValueJitter(Color inColor, float range)
        {
            float toShift = UnityEngine.Random.Range(-range, range);
            return ValueShift(inColor, toShift);
        }

    }

    public static class ColorSchemes
    {
        public delegate List<Color> SchemeDelegate(Color inColor, ColorSpace colorSpace = ColorSpace.ryb);

        // TODO: Weight schemes based on how nice they are?
        // Analogous:                       STRONG
        // Analogous Plus Complementary:    Strong
        // Analogous Plus Split Comp:       Okay (too many colors)
        // Analogous Plus Triadic:          Okay if desaturated (too many colors)
        // Complementary:                   Eh
        // Tetradic:                        Not a fan
        // Reverse Tetradic:                See above
        // Split Complementary:             Okay if desaturated (and even then...)
        // Split Comp Plus Comp:            Okay
        // Split Comp Plus Triadic:         No
        // Square:                          No
        // Triadic:                         Okay if desaturated
        // Triadic Plus Comp:               Eh

        public static List<SchemeDelegate> GetValidSchemes(int numDesiredColors)
        {
            /// Given a number of desired colors, returns a list of functions which will produce that # of colors.
            List<SchemeDelegate> schemeFuncs = new List<SchemeDelegate>();
            if (numDesiredColors == 1)
            {
                // One color? Bounce input color back out.
                SchemeDelegate sameColor = (x, y) => new List<Color> { x };
                schemeFuncs.Add(sameColor);
            }
            else if (numDesiredColors == 2)
            {
                schemeFuncs.Add(new SchemeDelegate(GetComplementaryColor));
            }
            else if (numDesiredColors == 3)
            {
                schemeFuncs.Add(new SchemeDelegate(GetTriadicColors));
                schemeFuncs.Add(new SchemeDelegate(GetAnalogousColors));
                schemeFuncs.Add(new SchemeDelegate(GetSplitComplementaryColors));
            }
            else if (numDesiredColors == 4)
            {
                schemeFuncs.Add(new SchemeDelegate(GetTetradicColors));
                schemeFuncs.Add(new SchemeDelegate(GetReverseTetradicColors));
                schemeFuncs.Add(new SchemeDelegate(GetSquareColors));
                schemeFuncs.Add(new SchemeDelegate(GetTriadicPlusComplementaryColors));
                schemeFuncs.Add(new SchemeDelegate(GetAnalogousPlusComplementaryColors));
                schemeFuncs.Add(new SchemeDelegate(GetSplitComplementaryPlusComplementaryColors));

            }
            else if (numDesiredColors == 5)
            {
                schemeFuncs.Add(new SchemeDelegate(GetTriadicPlusAnalogousColors));
                schemeFuncs.Add(new SchemeDelegate(GetAnalogousPlusSplitComplementaryColors));
                schemeFuncs.Add(new SchemeDelegate(GetSplitComplementaryPlusTriadicColors));
            }
            return schemeFuncs;
        }

        public static List<Color> GetComplementaryColor(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 180.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
            }
            outColors.Add(color1);
            return outColors;
        }

        public static List<Color> GetTriadicColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 120.0f);
            Color color2 = ColorOps.HueShift(inColor, 240.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            return outColors;
        }

        public static List<Color> GetAnalogousColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 30.0f);
            Color color2 = ColorOps.HueShift(inColor, 330.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            return outColors;
        }

        public static List<Color> GetSplitComplementaryColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 150.0f);
            Color color2 = ColorOps.HueShift(inColor, 210.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            return outColors;
        }

        public static List<Color> GetTetradicColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 120.0f);
            Color color2 = ColorOps.HueShift(inColor, 180.0f);
            Color color3 = ColorOps.HueShift(inColor, 300.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
                color3 = ColorOps.RYBtoRGB(color3);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            outColors.Add(color3);
            return outColors;
        }

        public static List<Color> GetReverseTetradicColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            /// Like Tetradic, but mirrored.
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 60.0f);
            Color color2 = ColorOps.HueShift(inColor, 180.0f);
            Color color3 = ColorOps.HueShift(inColor, 240.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
                color3 = ColorOps.RYBtoRGB(color3);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            outColors.Add(color3);
            return outColors;
        }

        public static List<Color> GetSquareColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            List<Color> outColors = new List<Color>();
            outColors.Add(inColor);
            if (colorSpace == ColorSpace.ryb)
            {
                inColor = ColorOps.RGBtoRYB(inColor);
            }

            Color color1 = ColorOps.HueShift(inColor, 90.0f);
            Color color2 = ColorOps.HueShift(inColor, 180.0f);
            Color color3 = ColorOps.HueShift(inColor, 270.0f);

            if (colorSpace == ColorSpace.ryb)
            {
                color1 = ColorOps.RYBtoRGB(color1);
                color2 = ColorOps.RYBtoRGB(color2);
                color3 = ColorOps.RYBtoRGB(color3);
            }
            outColors.Add(color1);
            outColors.Add(color2);
            outColors.Add(color3);
            return outColors;
        }

        // Derivative schemes (combinations of other schemes) follow:
        public static List<Color> GetTriadicPlusComplementaryColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 4 colors
            return GetTriadicColors(inColor, colorSpace).Concat(GetComplementaryColor(inColor, colorSpace).GetRange(1, 1)).ToList();
        }

        public static List<Color> GetAnalogousPlusComplementaryColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 4 colors
            return GetAnalogousColors(inColor, colorSpace).Concat(GetComplementaryColor(inColor, colorSpace).GetRange(1, 1)).ToList();
        }

        public static List<Color> GetSplitComplementaryPlusComplementaryColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 4 colors
            return GetSplitComplementaryColors(inColor, colorSpace).Concat(GetComplementaryColor(inColor, colorSpace).GetRange(1, 1)).ToList();
        }

        public static List<Color> GetTriadicPlusAnalogousColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 5 colors
            return GetTriadicColors(inColor, colorSpace).Concat(GetAnalogousColors(inColor, colorSpace).GetRange(1, 2)).ToList();
        }

        public static List<Color> GetAnalogousPlusSplitComplementaryColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 5 colors
            return GetAnalogousColors(inColor, colorSpace).Concat(GetSplitComplementaryColors(inColor, colorSpace).GetRange(1, 2)).ToList();
        }

        public static List<Color> GetSplitComplementaryPlusTriadicColors(Color inColor, ColorSpace colorSpace = ColorSpace.ryb)
        {
            // Derivative, 5 colors
            return GetSplitComplementaryColors(inColor, colorSpace).Concat(GetTriadicColors(inColor, colorSpace).GetRange(1, 2)).ToList();
        }
    }
}
