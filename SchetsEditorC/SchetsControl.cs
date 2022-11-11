using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static Schets;

public class SchetsControl : UserControl
{   
    public Schets schets;
    private Color penkleur= Color.Black;
    private int pengrootte = 3;

    public Color PenKleur
    { get { return penkleur; }
    }
    public int PenGrootte
    { get { return pengrootte; } }
    public Schets Schets
    { get { return schets;   }
    }
    public SchetsControl()
    {   this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {   schets.Teken(pea.Graphics);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {   schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {   Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
    }
    public void Schoon(object o, EventArgs ea)
    {   schets.Schoon();
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }
    public void VeranderKleur(Button kleurKiezer)
    {   Color kleurNaam = kleurKiezer.BackColor;
        penkleur = kleurNaam;
    }

    public void TekenBitmapUitLijst()
    {
        schets.Schoon();
        foreach (ObjectGetekend Objectg in schets.Objectengetekend)
        { 
            if(Objectg.type.ToString() == "tekst")
            {
                Objectg.type.Letter(this, Objectg.c.ToCharArray()[0], Objectg.kleur, true);

            }else Objectg.type.Teken(this, Objectg.start, Objectg.eind, Objectg.kleur, Objectg.dikte);
        }
        this.Invalidate();
    }
    
}