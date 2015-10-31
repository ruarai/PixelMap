﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelMapSharp
{
    public struct Pixel
    {
        /// <summary>
        /// Creates a pixel from a System.Drawing color.</summary>
        public Pixel(Color c)
        {
            A = c.A;
            R = c.R;
            G = c.G;
            B = c.B;
        }
        /// <summary>
        /// Creates a pixel from RGB values.</summary>
        public Pixel(byte r, byte g, byte b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a pixel from ARGB values.</summary>
        public Pixel(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a pixel from HSL-A values.</summary>
        public Pixel(double hue, double saturation, double lightness, byte alpha)
        {
            double q = lightness < 0.5 ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
            double p = 2 * lightness - q;

            hue /= 360f;

            double r = hueToRGB(p, q, hue + 1 / 3f);
            double g = hueToRGB(p, q, hue);
            double b = hueToRGB(p, q, hue - 1 / 3f);

            A = alpha;
            R = window(r * 255);
            G = window(g * 255);
            B = window(b * 255);
        }

        /// <summary>
        /// Creates a pixel from HSL values.</summary>
        public Pixel(double hue, double saturation, double lightness)
            : this(hue, saturation, lightness, 255)
        { }

        private static double hueToRGB(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1 / 6f) return p + (q - p) * 6 * t;
            if (t < 1 / 2f) return q;
            if (t < 2 / 3f) return p + (q - p) * (2 / 3f - t) * 6;
            return p;
        }

        private static byte window(double c)
        {
            return (byte)Math.Min(Math.Max(0, c), 255);
        }

        /// <summary>
        /// The alpha value of the Pixel.</summary>
        public byte A;
        /// <summary>
        /// The red value of the Pixel.</summary>
        public byte R;
        /// <summary>
        /// The green value of the Pixel.</summary>
        public byte G;
        /// <summary>
        /// The blue value of the Pixel.</summary>
        public byte B;


        /// <summary>
        /// The Hue value of the Pixel, radially spanning from 0 to 360 degrees.</summary>
        public double Hue
        {
            get
            {
                double r = R / 255.0f;
                double g = G / 255.0f;
                double b = B / 255.0f;

                double max = r;
                double min = r;

                if (g > max) max = g;
                if (b > max) max = b;

                if (g < min) min = g;
                if (b < min) min = b;

                double delta = max - min;

                double hue = 0.0f;

                if (r == max)
                {
                    hue = (g - b) / delta;
                }
                else if (g == max)
                {
                    hue = 2 + (b - r) / delta;
                }
                else if (b == max)
                {
                    hue = 4 + (r - g) / delta;
                }
                hue *= 60;

                if (hue < 0.0f)
                {
                    hue += 360.0f;
                }

                if (double.IsNaN(hue))
                    return 0;
                return hue;
            }
            set
            {
                this = new Pixel(value, Saturation, Lightness, A);
            }
        }

        /// <summary>
        /// The saturation value of the Pixel.</summary>
        public double Saturation
        {
            get
            {
                double r = R / 255.0f;
                double g = G / 255.0f;
                double b = B / 255.0f;

                double l;
                double s = 0;

                double max = r;
                double min = r;

                if (g > max) max = g;
                if (b > max) max = b;

                if (g < min) min = g;
                if (b < min) min = b;

                // if max == min, then there is no color and
                // the saturation is zero.
                if (max != min)
                {
                    l = (max + min) / 2;

                    if (l <= .5)
                    {
                        s = (max - min) / (max + min);
                    }
                    else
                    {
                        s = (max - min) / (2 - max - min);
                    }
                }
                return s;
            }
            set
            {
                this = new Pixel(Hue, value, Lightness, A);
            }
        }
        /// <summary>
        /// The lightness value of the Pixel.</summary>
        public double Lightness
        {
            get
            {
                double r = R / 255.0f;
                double g = G / 255.0f;
                double b = B / 255.0f;

                double max = r;
                double min = r;

                if (g > max) max = g;
                if (b > max) max = b;

                if (g < min) min = g;
                if (b < min) min = b;

                return (max + min) / 2;
            }
            set
            {
                this = new Pixel(Hue, Saturation, value, A);
            }
        }

        /// <summary>
        /// The GDI color equivalent.</summary>
        public Color Color
        {
            get { return Color.FromArgb(A, R, G, B); }
            set { this = new Pixel(value.A, value.R, value.G, value.B); }
        }
    }
}
