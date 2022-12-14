using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using static Schets;

public class SchetsWin : Form
{   
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    Button kleurKiezer;

    bool vast;

    ISchetsTool[] tempTools = { new PenTool()
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new RandTool()
                                , new CirkelTool()
                                , new TekstTool()
                                , new ObjectGumTool()
                                };

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        this.Close();
    }

    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()         
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new RandTool ()
                                , new CirkelTool() , new TekstTool()
                                , new ObjectGumTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };

        this.ClientSize = new Size(700, 500);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                    {   vast=true;  
                                        huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisLos (schetscontrol, mea.Location);
                                        vast = false; 
                                    };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                    {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar, kleurKiezer.BackColor, false); 
                                    };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        
        ToolStripDropDownItem savemenu = new ToolStripMenuItem("Opslaan");
        ToolStripDropDownItem openmenu = new ToolStripMenuItem("Openen");
        savemenu.DropDownItems.Add("Opslaan als afbeelding...", null, this.saving);
        savemenu.DropDownItems.Add("Opslaan als object...", null, this.SaveObject);
        menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {savemenu});
        openmenu.DropDownItems.Add("Open..", null, this.openen);
        openmenu.DropDownItems.Add("Open object", null, this.openObject);
        menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {openmenu});
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menuStrip.Items.Add(menu);

    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            try 
            {
                item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            } 
            catch 
            {
                item.Image = new Bitmap($"Icons/{tool.ToString()}.png");
            }
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
        //erbij gedaan
        //menu.DropDownItems.Add("Kies kleur", null, KleurMenu);
        menuStrip.Items.Add(menu);

    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {   
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);
            
        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";  
        clear.Location = new Point(  0, 0); 
        clear.Click += schetscontrol.Schoon;        
            
        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate"; 
        rotate.Location = new Point( 80, 0); 
        rotate.Click += schetscontrol.Roteer; 
           
        Label penkleur = new Label(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur:"; 
        penkleur.Location = new Point(440, 3); 
        penkleur.AutoSize = true; 
        
        //erbij gedaan
        kleurKiezer = new Button(); paneel.Controls.Add(kleurKiezer);
        kleurKiezer.BackColor = Color.White;
        kleurKiezer.Location = new Point(510, 0);
        kleurKiezer.Click += KleurMenu;
    }
    //GEWIJZIGD!
    private void saving(object sender, System.EventArgs e)
    {
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        saveFileDialog1.Filter = "JPeg Image|*.jpg|PNG Image|*.png";
        saveFileDialog1.Title = "Save an Image File";
        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        {
            schetscontrol.schets.bitmap.Save(saveFileDialog1.FileName); 
        }    
    }

    //GEWIJZIGD!
   private void openen(object sender, System.EventArgs e)
    {
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        {
            Bitmap bit = new Bitmap(openFileDialog1.FileName);
                 schetscontrol.schets.bitmap = bit;
        }
        schetscontrol.Invalidate();
    }
    //GEWIJZIGD!
    private void SaveObject(object sender, EventArgs e)
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Text File | *.txt";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string space = "";
            foreach (ObjectGetekend obj in schetscontrol.schets.Objectengetekend)
            { 
                space += $"{obj.type.ToString()}~{obj.start.X}~{obj.eind.Y}~{obj.eind.X}~{obj.eind.Y}~{obj.kleur.A}-{obj.kleur.R}-{obj.kleur.G}-{obj.kleur.B}~{obj.dikte}~{obj.c};";
            }
            StreamWriter schrijver = new StreamWriter(dialog.OpenFile());

            schrijver.Write(space);
            schrijver.Dispose();
            schrijver.Close();
        }
        
    }
    //erbij gedaan
    private void KleurMenu(object sender, EventArgs e) {
        ColorDialog colorPicker = new ColorDialog(); 
        if (colorPicker.ShowDialog() == DialogResult.OK)
        {
            kleurKiezer.BackColor = colorPicker.Color;
            schetscontrol.VeranderKleur(kleurKiezer);
        }
    }

    public void openObject(object sender, EventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Text File | *.txt";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
                // Maak StreamReader  
                using (StreamReader reader = new StreamReader(dialog.FileName))
                {
                    string line;
                    schetscontrol.schets.Objectengetekend.Clear();
                    // Lees regel voor regel  
                    while ((line = reader.ReadLine()) != null)
                    {
                        string fullLine = line;
                        string[] objectStrings = fullLine.Split(";");
                        foreach (string objectString in objectStrings) {
                            string[] ObjProp= objectString.Split("~");
                            if (ObjProp.Length > 1) { 

                                ISchetsTool tool = tempTools[0];

                                foreach (ISchetsTool t in tempTools) {
                                    if (t.ToString() == ObjProp[0]) {
                                        tool = t;
                                    }
                                }

                            string[] rgbS = ObjProp[5].Split("-");

                                 ObjectGetekend getobj = new ObjectGetekend(   
                                        tool,
                                        new Point(Int32.Parse(ObjProp[1]), Int32.Parse(ObjProp[2])),
                                        new Point(Int32.Parse(ObjProp[3]), Int32.Parse(ObjProp[4])),
                                        Color.FromArgb(Int32.Parse(rgbS[0]), Int32.Parse(rgbS[1]), Int32.Parse(rgbS[2]), Int32.Parse(rgbS[3])), 
                                        Int32.Parse(ObjProp[6]),
                                        ObjProp[7]
                                    );
                                schetscontrol.schets.Objectengetekend.Add(getobj);
                            }
                        }
                        schetscontrol.TekenBitmapUitLijst();
                    }
                }
        }

    }
}