using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

public class Schets
{
    public Bitmap bitmap;
    public List<ObjectGetekend> Objectengetekend = new List<ObjectGetekend>();
    public List<ObjectGetekend> savedObjectengetekend = new List<ObjectGetekend>();

    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }

    public class ObjectGetekend
    {
        public ISchetsTool type;
        public Point start;
        public Point eind;
        public Color kleur;
        public int dikte;
        public string c = "";

        public ObjectGetekend (ISchetsTool type, Point start, Point eind, Color kleur, int dikte, string c)
        {
            this.type = type; 
            this.start = start;
            this.eind = eind;
            this.kleur = kleur;
            this.dikte = dikte; 
            this.c = c;
        }


    }

    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }
}