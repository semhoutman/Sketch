using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Diagnostics;
using static Schets;
using System.Windows.Forms;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c, Color kleur, bool opened);
    void Teken(SchetsControl s, Point start, Point end, Color kleur, int pendikte);
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;
    protected Color color;
    protected int pengrootte;
    protected int newPengrootte = 0;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
    }
    
    public virtual void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        if (!color.IsEmpty) 
        {
            kwast = new SolidBrush(color);
            color = Color.Empty;
        }
        pengrootte = s.PenGrootte;
        if (newPengrootte != 0)
        {
            pengrootte = newPengrootte;
            newPengrootte = 0;
        }
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c, Color Kleur, bool opened);
   
    public virtual void Teken(SchetsControl s, Point start, Point end, Color kleur, int dikte) {  }
    
    
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c, Color kleur, bool opened)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();
            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
             if (!opened) 
             {
                s.schets.Objectengetekend.Add(new ObjectGetekend(this, this.startpunt, new Point(this.startpunt.X + (int)sz.Width, this.startpunt.Y + (int)sz.Height), kleur, pengrootte, c.ToString()));
             }
            startpunt.X += (int)sz.Width;
            s.Invalidate();
        }
    }
        public override void Teken(SchetsControl s, Point start, Point end, Color kleur, int pendikte) { }

    
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }
    public static Pen MaakPen(Brush b, int dikte)
    {   Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
        s.schets.Objectengetekend.Add(new ObjectGetekend(this, this.startpunt, p, ((SolidBrush)kwast).Color, pengrootte, ""));
    }
    public override void Teken(SchetsControl s, Point start, Point end, Color kleur, int pendikte) {
        color = kleur;
        newPengrootte = pendikte;
        base.MuisLos(s, start);
        this.Compleet(s.MaakBitmapGraphics(), start, end);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c, Color kleur, bool opened)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {   this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawRectangle(MaakPen(kwast,pengrootte), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }
    public override void Compleet(Graphics g, Point p1, Point p2)
    {   g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
    }
    public double dCirkel(int x, int y, int bX, int eX, int bY, int eY) {
        double a = (eX - bX)/2;
        double b = (eY - bY)/2;
        double mX = bX + a;
        double mY = bY + b;
        double dCirkel = ((x-mX)*(x-mX))/(a*a) + ((y-mY)*(y-mY))/(b*b);
        return dCirkel;
    }

    public ObjectGetekend checkbounds(SchetsControl s, Point p)
    {
        int x = p.X;
        int y = p.Y;
        ObjectGetekend eindObject = null;
        foreach (ObjectGetekend getobj in s.schets.Objectengetekend)
        {
            int? checkXb= getobj.start.X < getobj.eind.X ? getobj.start.X : getobj.eind.X;
            int? checkXe = getobj.start.X > getobj.eind.X ? getobj.start.X : getobj.eind.X;
            int? checkYb = getobj.start.Y < getobj.eind.Y ? getobj.start.Y : getobj.eind.Y;
            int? checkYe = getobj.start.Y > getobj.eind.Y ? getobj.start.Y : getobj.eind.Y;

            // niks met bounding te maken
            if (getobj.type.ToString() == "lijn")
            {
                double afstand = (Math.Abs((getobj.eind.X - getobj.start.X) * (getobj.start.Y - y) - 
                    (getobj.start.X - x) * (getobj.eind.Y - getobj.start.Y))) / (Math.Sqrt((getobj.eind.X - getobj.start.X) * 
                    (getobj.eind.X - getobj.start.X) + (getobj.eind.Y - getobj.start.Y) * (getobj.eind.Y - getobj.start.Y)));
                if (afstand <= getobj.dikte && afstand >= -1*getobj.dikte)
                {
                    eindObject = getobj;
                }
            }
            
            if ((x >= checkXb && x <= checkXe) && (y >= checkYb && y <= checkYe))
            {
                switch (getobj.type.ToString()) {
                    case "vlak": case "tekst":
                        eindObject = getobj;
                        break;

                    case "cirkel":
                        // formule voor ellipse:
                        // (x-mx)^2/a^2 + (y-my)^2/b^2
                        double DCirkel = dCirkel(x, y, (int)checkXb, (int)checkXe, (int)checkYb, (int)checkYe);
                        if (DCirkel <= 1+(double)((double)getobj.dikte/100))
                        {
                            eindObject = getobj;
                        }
                        break;

                    case "rand":
                        double dCirkelrand = dCirkel(x, y, (int)checkXb, (int)checkXe, (int)checkYb, (int)checkYe);
                        if (dCirkelrand <= 1+(double)((double)getobj.dikte/100) && dCirkelrand >= 1-(double)((double)getobj.dikte/100))
                        {
                            eindObject = getobj;
                        }
                        break;

                     case "kader":
                        bool randl = ((x >= checkXb - getobj.dikte && x <= checkXb + getobj.dikte) && (y >= checkYb && y <= checkYe));
                        bool randr = ((x >= checkXe - getobj.dikte && x <= checkXe + getobj.dikte) && (y >= checkYb && y <= checkYe));
                        bool randb = ((x >= checkXb && x <= checkXe) && (y >= checkYb - getobj.dikte && y <= checkYb + getobj.dikte));
                        bool rando = ((x >= checkXb && x <= checkXe) && (y >= checkYe - getobj.dikte && y <= checkYe + getobj.dikte));
                        if (randl || randr || randb || rando)
                        {
                            eindObject = getobj;
                        }
                        break;
                }
            }
        }
        return eindObject;
    }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {   this.MuisLos(s, p);
        this.MuisVast(s, p);
    }
}
    
/*public class GumTool : PenTool
{
      public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {   g.DrawLine(MaakPen(Brushes.White, pengrootte+2), p1, p2);
    }
}*/

//VERANDERD!
public class ObjectGumTool : PenTool
{
    public override string ToString() { return "delete"; }

    public override void MuisLos(SchetsControl s, Point p)
    { 
        verwijderObject(s, checkbounds(s,p));
    }

    private void verwijderObject(SchetsControl s, ObjectGetekend obj)
    {
        if (obj != null) 
        {
            s.schets.Objectengetekend.Remove(obj);
            s.TekenBitmapUitLijst();
        }
    }
}

//VERANDERD!
public class RandTool : TweepuntTool
{
    public override string ToString() { return "rand"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}
//VERANDERD!
public class CirkelTool : RandTool
{
    public override string ToString() { return "cirkel"; }

    public override void Compleet(Graphics g, Point p1, Point p2)
    {
        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }
}

