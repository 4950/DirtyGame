using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CleanGame.Game.Util
{
    public class Settings : Singleton<Settings>
    {
        public class GlobalSettings
        {
            public List<UserSettings> Users;
            public UserSettings DefaultUser { get; set; }
            public bool Fullscreen { get; set; }
            public Vector2 Resolution { get; set; }
        }
        public class UserSettings
        {
            //defaults
            private bool disableBackgroundMusic = false;
            private bool captureMouse = true;
            private bool disableSoundEffects = false;
            private String name = "Test";

            public String Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            public bool CaptureMouse
            {
                get
                {
                    return captureMouse;
                }
                set
                {
                    captureMouse = value;
                }
            }
            public bool DisableBackgroundMusic
            {
                get
                {
                    return disableBackgroundMusic;
                }
                set
                {
                    disableBackgroundMusic = value;
                }
            }
            public bool DisableSoundEffects
            {
                get
                {
                    return disableSoundEffects;
                }
                set
                {
                    disableSoundEffects = value;
                }
            }
        }

        public GlobalSettings Global { get; set; }

        public Settings()
        {
        }
        private UserSettings CreateUser()
        {
            UserSettings u = new UserSettings();
            return u;
        }
        private void CreateGlobal()
        {
            Global = new GlobalSettings();
            Global.Fullscreen = false;
            Global.Resolution = new Vector2(800, 600);
            Global.Users = new List<UserSettings>();

            Global.DefaultUser = CreateUser();
            Global.Users.Add(Global.DefaultUser);
        }
        public void LoadSettings()
        {
            string path = App.Path + "settings.xml";
            if (File.Exists(path))
            {
                XmlReader read = XmlReader.Create(path);

                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(GlobalSettings));

                    Global = xs.Deserialize(read) as GlobalSettings;
                }
                catch (Exception)//failed to read users, create new
                {
                    CreateGlobal();
                }
                finally
                {
                    read.Close();
                }
            }
            else//settings do not exist, create new
                CreateGlobal();
        }
        public void SaveSettings()
        {
            string path = App.Path + "settings.xml";

            if (Global != null && Directory.Exists(Path.GetDirectoryName(path)))
            {
                XmlWriterSettings sett = new XmlWriterSettings();
                sett.Indent = true;
                sett.IndentChars = "\t";

                XmlWriter writer = XmlWriter.Create(path, sett);

                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(GlobalSettings));

                    xs.Serialize(writer, Global);
                }
                catch (Exception)
                {
                }
                finally
                {
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }
}
