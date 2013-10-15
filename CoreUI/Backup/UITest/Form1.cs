using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MTV3D65;
using CoreUI;

namespace UITest
{
    public partial class Form1 : Form
    {

        TVEngine mTV;
        TVInputEngine mInput;
        TVScreen2DText mText;
        TVScene mScene;
        CoreUIEngine mCoreUI;
        bool tmpMouseB1, tmpMouseB2, tmpMouseB3;
        int tmpMouseX, tmpMouseY;

        bool DoLoop;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mTV = new TVEngine();
            mTV.SetDebugMode(true);
            mTV.SetDebugFile(Application.StartupPath + "\\Debug.txt");
            mScene = new TVScene();
            mInput = new TVInputEngine();
            mText = new TVScreen2DText();
            mTV.Init3DWindowed(this.Handle);
            mTV.GetViewport().SetAutoResize(true);
            mInput.Initialize(true, true);

            mCoreUI = new CoreUIEngine(mTV, mScene);

            CoreUI.Visuals.ImageBrush iv = new CoreUI.Visuals.ImageBrush();
            iv.Texture = CoreUIEngine.LoadTexture(Properties.Resources.Leopard);
            iv.SizeMode = SizeMode.Fill;
            mCoreUI.Children.BackgroundVisual = iv;

            Window w = new Window();
            w.Position = new Point(200, 200);
            w.Size = new Point(200, 200);
            w.Title = "Window";
            w.Show();

            CoreUI.Panel p = new CoreUI.Panel();
            p.SizeMode = SizeMode.Fill;
            w.Content = p;

            CoreUI.Elements.Button b2 = new CoreUI.Elements.Button();
            b2.Position = new Point(10, 10);
            b2.Size = new Point(100, 23);
            b2.Text = "Rawr!";
            b2.Font = new Font("Viner Hand ITC", 24);
            p.AddElement(b2);
            
            CoreUI.Elements.Button b3 = new CoreUI.Elements.Button();
            b3.Position = new Point(10, 70);
            b3.Size = new Point(175, 23);
            b3.Text = "Show Modal Dialog";
            b3.Click += new CoreUI.Elements.Button.ClickEventHandler(b3_Click);
            p.AddElement(b3);

            CoreUI.Elements.Button b4 = new CoreUI.Elements.Button();
            b4.Position = new Point(10, 140);
            b4.Size = new Point(175, 23);
            b4.Text = "Show MessageBox";
            b4.Click += new CoreUI.Elements.Button.ClickEventHandler(b4_Click);
            p.AddElement(b4);

            Window ww = new Window();
            ww.Position = new Point(250, 300);
            ww.Size = new Point(300, 200);
            ww.Title = "Another Window!";
            ww.Show();

            CoreUI.Panel p2 = new CoreUI.Panel();
            p2.SizeMode = SizeMode.Fill;
            ww.Content = p2;

            CoreUI.Elements.Label l = new CoreUI.Elements.Label();
            l.Position = new System.Drawing.Point(5, 20);
            l.Size = new Point(175, 23);
            l.Text = "(Click me)This label truncates";
            l.TextMode = CoreUI.Elements.LabelTextMode.Truncate;
            l.MouseUp += new MouseEventHandler(l_MouseUp);
            p2.AddElement(l);

            CoreUI.Elements.Listbox lb = new CoreUI.Elements.Listbox();
            lb.Position = new Point(5, 50);
            lb.Size = new Point(100, 100);
            lb.AddItem("This is");
            lb.AddItem("a listbox!");
            lb.AddItem("which");
            lb.AddItem("also truncates");
            lb.AddItem("in both directions");
            lb.AddItem("See?");
            lb.AddItem("!!!!!!!!!!");
            lb.AddItem("Scrolling!");
            p2.AddElement(lb);

            CoreUI.Elements.ComboBox cb = new CoreUI.Elements.ComboBox();
            cb.Position = new Point(110, 50);
            cb.Size = new Point(100, 10);
            cb.AddItem("ComboBox!");
            cb.AddItem("~Select Me~");
            p2.AddElement(cb);

            CoreUI.Elements.CheckBox chb = new CoreUI.Elements.CheckBox();
            chb.Position = new Point(110, 70);
            chb.Size = new Point(100, 15);
            chb.Text = "A Checkbox";
            chb.IsThreeState = true;
            p2.AddElement(chb);

            CoreUI.Elements.RadioButton rdb = new CoreUI.Elements.RadioButton();
            rdb.Position = new Point(110, 90);
            rdb.Size = new Point(100, 15);
            rdb.Text = "A RadioButton";
            rdb.IsThreeState = true;
            p2.AddElement(rdb);

            CoreUI.Elements.Button b = new CoreUI.Elements.Button();
            b.Position = new Point(100, 100);
            b.Size = new Point(130, 23);
            b.Text = "Relaunch Window";
            b.Tag = w;
            b.Click += new CoreUI.Elements.Button.ClickEventHandler(b_Click);
            mCoreUI.Children.AddElement(b);

            CoreUI.MessageBox.Show("Warning, Virus Detected!\nEat it?", "Warning", CoreUI.MessageBox.MessageBoxButttons.OkCancel);

            Window www = new Window();
            www.Position = new Point(400, 50);
            www.Size = new Point(175, 50);
            www.Style = Window.WindowStyle.None;
            www.Show();

            CoreUI.Elements.Label l2 = new CoreUI.Elements.Label();
            l2.Position = new System.Drawing.Point(0, 0);
            l2.Text = "Window without chrome!\n(This is a label)";
            l2.TextMode = CoreUI.Elements.LabelTextMode.SizeToContent;
            www.Content = l2;

            DoLoop = true;
            this.Show();
            MainLoop();
        }

        void l_MouseUp(object sender, MouseEventArgs e)
        {
            ((CoreUI.Elements.Label)sender).TextMode = CoreUI.Elements.LabelTextMode.SizeToContent;
            ((CoreUI.Elements.Label)sender).Text = "but can expand to fill content!\nEven if its multiple lines with different sizes";
        }

        void b4_Click(object sender)
        {
            CoreUI.MessageBox.Show("Is this a messagebox?", "Question!", CoreUI.MessageBox.MessageBoxButttons.YesNo);
        }

        void b_Click(object sender)
        {
            ((CoreUI.Elements.Button)sender).Text = "!&$@$%!(%$&@%$%&!*%$&@*$%*&!%&$@";
            ((Window)((CoreUI.Elements.Button)sender).Tag).Show();
        }

        void b3_Click(object sender)
        {
            Window w = new Window();
            w.Position = new Point(50, 50);
            w.Size = new Point(200, 100);
            w.Title = "OMG, Modality!";
            w.ShowDialog();

            CoreUI.Elements.Button b = new CoreUI.Elements.Button();
            b.Position = new Point(10, 10);
            b.Size = new Point(100, 23);
            b.Text = "Close";
            b.Click += new CoreUI.Elements.Button.ClickEventHandler(Modalb_click);
            w.Content = b;
        }
        void Modalb_click(object sender)
        {
            ((Window)((CoreUI.Elements.Button)sender).Parent).Close();
        }
        private void MainLoop()
        {
            while (DoLoop)
            {
                Application.DoEvents();
                mInput.GetAbsMouseState(ref tmpMouseX, ref tmpMouseY, ref tmpMouseB1, ref tmpMouseB2, ref tmpMouseB3);
                mCoreUI.GetInput(tmpMouseX, tmpMouseY, tmpMouseB1, tmpMouseB2, tmpMouseB3);
                mTV.Clear(false);
                mScene.RenderAll(true);
                mCoreUI.Render();
                mTV.RenderToScreen();
                mCoreUI.Update();
            }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoLoop = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            mCoreUI.Resize();
        }
    }
}
