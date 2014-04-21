using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CleanGame.Game.Util
{
    class SoundSystem : Singleton<SoundSystem>
    {
        private Dirty game;
        private List<String> backgroundMusic;
        private String currentBackground;
        private Random rand;
        private System.Windows.Media.MediaPlayer player;

        public float MaxVolume { get; set; }
        public bool Loop { get; set; }

        public SoundSystem()
        {

            backgroundMusic = new List<string>();
            rand = new Random();

            player = new System.Windows.Media.MediaPlayer();
        }

        private void PlaySong(string s)
        {
            if (File.Exists(App.Path + "Sound\\Music\\" + s))
            {
                currentBackground = s;
                player.Volume = 0;
                if (!Settings.Instance.Global.DefaultUser.DisableBackgroundMusic)
                {
                    player.Open(new Uri(App.Path + "Sound\\Music\\" + s));
                    player.Play();
                }
            }
        }

        public void SetGame(Dirty game)
        {
            this.game = game;
        }

        public void PlayEffect(string effect)
        {
            if (!Settings.Instance.Global.DefaultUser.DisableSoundEffects)
            {
                SoundEffect s = game.resourceManager.GetResource<SoundEffect>("Sound\\" + effect);
                SoundEffectInstance i = s.CreateInstance();
                i.Volume = .5f;
                i.Play();
            }
        }
        public void AddBackgroundMusic(string song)
        {
            if (File.Exists(App.Path + "Sound\\Music\\" + song))
                if (!backgroundMusic.Contains(song))
                    backgroundMusic.Add(song);
        }
        public void PlayRand()
        {
            if (backgroundMusic.Count > 0)
            {
                String s = currentBackground;
                while (s == currentBackground)
                {
                    int i = rand.Next(0, backgroundMusic.Count - 1);
                    s = backgroundMusic[i];
                }
                PlaySong(s);
            }
        }
        public void RemoveBackgroundMusic(string song)
        {
            backgroundMusic.Remove(song);
            if (currentBackground == song)
                PlayRand();
        }
        public void Update()
        {
            if (currentBackground != null)
            {
                double pos = player.Position.TotalMilliseconds;
                if (player.NaturalDuration.HasTimeSpan && player.NaturalDuration.TimeSpan.TotalMilliseconds - pos == 0)
                {
                    if (Loop && currentBackground != null)
                        PlaySong(currentBackground);
                    else if (backgroundMusic.Count == 1)
                        PlaySong(backgroundMusic[0]);
                    else if (backgroundMusic.Count > 1)
                    {
                        String s = currentBackground;
                        while (s == currentBackground)
                        {
                            int i = rand.Next(0, backgroundMusic.Count - 1);
                            s = backgroundMusic[i];
                        }
                        PlaySong(s);
                    }
                }
                else if (player.NaturalDuration.HasTimeSpan && player.NaturalDuration.TimeSpan.TotalMilliseconds - pos < 5000)//fade out
                {
                    player.Volume = (player.NaturalDuration.TimeSpan.TotalMilliseconds - pos) / 5000.0d * MaxVolume;
                }
                else if (pos < 5000)//fade in
                {
                    player.Volume = (pos / 5000.0d) * MaxVolume;
                }
            }
        }
        public void PlayBackgroundMusic(string song)
        {
            if (!backgroundMusic.Contains(song))
                backgroundMusic.Add(song);
            PlaySong(song);
        }
    }
}
