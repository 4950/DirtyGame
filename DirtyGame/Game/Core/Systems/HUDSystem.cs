using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework.Systems;
using EntityFramework;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Components;
using Microsoft.Xna.Framework;
using CoreUI;
using DirtyGame.game.SGraphics;
using CoreUI.Elements;

namespace DirtyGame.game.Core.Systems
{
    public class HUDSystem : EntitySystem
    {
        private List<ProgressBar> mPBs;
        private Panel pbDisplay;
        private CoreUIEngine UIEngine;
        private Renderer renderer;

        public HUDSystem(Renderer r, CoreUIEngine UIEngine)
            : base(SystemDescriptions.HUDSystem.Aspect, SystemDescriptions.HUDSystem.Priority)
        {
            pbDisplay = new Panel();
            mPBs = new List<ProgressBar>();
            this.UIEngine = UIEngine;
            renderer = r;
            UIEngine.Children.AddElement(pbDisplay);
        }
        public override void OnEntityAdded(EntityFramework.Entity e)
        {
            if (e.HasComponent<StatsComponent>())
            {
                ProgressBar pb = new ProgressBar();
                pb.Size = new System.Drawing.Point(50, 5);
                pb.Value = 100;
                pbDisplay.AddElement(pb);
                mPBs.Add(pb);
            }
        }

        public override void OnEntityRemoved(EntityFramework.Entity e)
        {
            if (e.HasComponent<StatsComponent>())
            {
                pbDisplay.RemoveElement(mPBs[mPBs.Count - 1]);
                mPBs.RemoveAt(mPBs.Count - 1);
            }
        }
        public override void ProcessEntities(IEnumerable<EntityFramework.Entity> entities, float dt)
        {
            int i = 0;
            foreach (Entity e in entities)
            {
                if (e.HasComponent<StatsComponent>() && i < mPBs.Count)
                {
                    StatsComponent hc = e.GetComponent<StatsComponent>();
                    SpatialComponent sc = e.GetComponent<SpatialComponent>();

                    ProgressBar pb = mPBs[i];
                    pb.Maximum = hc.MaxHealth;
                    pb.Value = (int)hc.CurrentHealth;

                    Vector2 pos = sc.Position + new Vector2(sc.Width/2 - pb.Size.X/2, -20);
                    pos = Vector2.Transform(pos, renderer.ActiveCamera.Transform);
                    if (pos.X >= 0 && pos.Y >= 0 && pos.X <= renderer.ActiveCamera.size.X && pos.Y <= renderer.ActiveCamera.size.Y)
                    {
                        pb.Visibility = Visibility.Visible;
                        pb.Position = new System.Drawing.Point((int)pos.X, (int)pos.Y);
                    }
                    else
                    {
                        pb.Visibility = Visibility.Hidden;
                    }
                    i++;
                }
            }
        }
    }
}
