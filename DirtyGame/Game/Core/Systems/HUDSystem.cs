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
            : base(SystemDescriptions.MonsterSystem.Aspect, SystemDescriptions.MonsterSystem.Priority)
        {
            pbDisplay = new Panel();
            mPBs = new List<ProgressBar>();
            this.UIEngine = UIEngine;
            renderer = r;
            UIEngine.Children.AddElement(pbDisplay);
        }
        public override void OnEntityAdded(EntityFramework.Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
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
            if (e.HasComponent<MonsterComponent>())
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
                if (e.HasComponent<MonsterComponent>() && i < mPBs.Count)
                {
                    
                    ProgressBar pb = mPBs[i];
                    Vector2 pos = e.GetComponent<Spatial>().Position;
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
