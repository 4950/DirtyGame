using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreUI
{
    public class MessageBox
    {
        public enum MessageBoxButttons { Ok, OkCancel, YesNo, YesNoCancel };
        public enum MessageBoxResultButtons { Ok, Cancel, Yes, No };
        public delegate void MessageBoxResultDelegate(object sender, MessageBoxResultButtons ResultButton);
        public event MessageBoxResultDelegate DialogResult;
        private MessageBoxButttons mButtons;
        private Window messagewin;
        private bool closed = false;

        private MessageBox(String Text, String Title, MessageBoxButttons Buttons)
        {
            mButtons = Buttons;

            messagewin = new Window();
            messagewin.Size = new System.Drawing.Point(200, 100);
            messagewin.Center();
            messagewin.Title = Title;
            messagewin.Closed += new Element.ElementEventHandler(messagewin_WindowClosed);

            CoreUI.Panel p = new CoreUI.Panel();
            p.SizeMode = SizeMode.Fill;
            messagewin.Content = p;

            Elements.Label l = new Elements.Label();
            l.Position = new System.Drawing.Point(5, 0);
            l.Size = new System.Drawing.Point(190, 70);
            l.Text = Text;
            l.TextMode = CoreUI.Elements.LabelTextMode.Truncate;
            p.AddElement(l);

            Elements.Button bOk;
            Elements.Button bCancel;
            Elements.Button bYes;
            Elements.Button bNo;
            switch (Buttons)
            {
                case MessageBoxButttons.Ok:
                    bOk = new Elements.Button();
                    bOk.Size = new System.Drawing.Point(60, 23);
                    bOk.Position = new System.Drawing.Point(135, 72);
                    bOk.Text = "Ok";
                    bOk.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bOk);
                    break;
                case MessageBoxButttons.OkCancel:
                    bCancel = new Elements.Button();
                    bCancel.Size = new System.Drawing.Point(60, 23);
                    bCancel.Position = new System.Drawing.Point(135, 72);
                    bCancel.Text = "Cancel";
                    bCancel.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bCancel);

                    bOk = new Elements.Button();
                    bOk.Size = new System.Drawing.Point(60, 23);
                    bOk.Position = new System.Drawing.Point(70, 72);
                    bOk.Text = "Ok";
                    bOk.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bOk);
                    break;
                case MessageBoxButttons.YesNo:
                    bNo = new Elements.Button();
                    bNo.Size = new System.Drawing.Point(60, 23);
                    bNo.Position = new System.Drawing.Point(135, 72);
                    bNo.Text = "No";
                    bNo.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bNo);

                    bYes = new Elements.Button();
                    bYes.Size = new System.Drawing.Point(60, 23);
                    bYes.Position = new System.Drawing.Point(70, 72);
                    bYes.Text = "Yes";
                    bYes.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bYes);
                    break;
                case MessageBoxButttons.YesNoCancel:
                    bCancel = new Elements.Button();
                    bCancel.Size = new System.Drawing.Point(60, 23);
                    bCancel.Position = new System.Drawing.Point(135, 72);
                    bCancel.Text = "Cancel";
                    bCancel.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bCancel);

                    bNo = new Elements.Button();
                    bNo.Size = new System.Drawing.Point(60, 23);
                    bNo.Position = new System.Drawing.Point(70, 72);
                    bNo.Text = "No";
                    bNo.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bNo);

                    bYes = new Elements.Button();
                    bYes.Size = new System.Drawing.Point(60, 23);
                    bYes.Position = new System.Drawing.Point(5, 72);
                    bYes.Text = "Yes";
                    bYes.Click += new CoreUI.Elements.Button.ClickEventHandler(Button_Click);
                    p.AddElement(bYes);
                    break;
            }
            messagewin.ShowDialog();
        }

        void messagewin_WindowClosed(object sender)
        {
            if (!closed)
            {
                MessageBoxResultButtons result = MessageBoxResultButtons.Ok;
                switch (mButtons)
                {
                    case MessageBoxButttons.Ok:
                        result = MessageBoxResultButtons.Ok;
                        break;
                    case MessageBoxButttons.OkCancel:
                        result = MessageBoxResultButtons.Cancel;
                        break;
                    case MessageBoxButttons.YesNo:
                        result = MessageBoxResultButtons.No;
                        break;
                    case MessageBoxButttons.YesNoCancel:
                        result = MessageBoxResultButtons.No;
                        break;
                }
                closed = true;
                if (DialogResult != null)
                    DialogResult(this, result);
            }
        }

        private void Button_Click(object sender)
        {
            messagewin.Close();
            MessageBoxResultButtons result = MessageBoxResultButtons.Ok;
            switch (((Elements.Button)sender).Text)
            {
                case "Ok":
                    result = MessageBoxResultButtons.Ok;
                    break;
                case "Cancel":
                    result = MessageBoxResultButtons.Cancel;
                    break;
                case "Yes":
                    result = MessageBoxResultButtons.Yes;
                    break;
                case "No":
                    result = MessageBoxResultButtons.No;
                    break;
            }
            closed = true;
            if (DialogResult != null)
                DialogResult(this, result);
        }

        public static MessageBox Show(String Text)
        {
            return new MessageBox(Text, "", MessageBoxButttons.Ok);
        }
        public static MessageBox Show(String Text, String Title)
        {
            return new MessageBox(Text, Title, MessageBoxButttons.Ok);
        }
        public static MessageBox Show(String Text, String Title, MessageBoxButttons Buttons)
        {
            return new MessageBox(Text, Title, Buttons);
        }
    }
}
