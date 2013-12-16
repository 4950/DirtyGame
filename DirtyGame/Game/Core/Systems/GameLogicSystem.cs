using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.Core.Components;
using DirtyGame.game.Core.Components.Render;
using DirtyGame.game.Core.Systems.Monster;
using DirtyGame.game.Core.Systems.Util;
using DirtyGame.game.Core.Events;
using Microsoft.Xna.Framework;
using EntityFramework.Systems;
using EntityFramework;
using DirtyGame.game.SGraphics;
using CoreUI.Elements;
using CoreUI.DrawEngines;

namespace DirtyGame.game.Core.Systems
{
    public class GameLogicSystem : EntitySystem
    {

        public int monstersdefeated;
        public int monstersalive;
        private Dirty game;
        private Label roundLabel;
        private float roundLblTime;

        private List<Entity> spawners = new List<Entity>();

        public override void OnEntityAdded(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                monstersalive++;
            }
        }
        public void SetupNextRound()
        {
            foreach (Entity ee in spawners)//clear old spawners
            {
                game.world.DestroyEntity(ee);
            }

            roundLabel.Text = "~Round " + game.CurrentLevel + "~";
            roundLabel.Visibility = CoreUI.Visibility.Visible;
            roundLblTime = 3f;

            Entity monsterWeapon = game.entityFactory.CreateRangedWeaponEntity("Monsterbow", "bow", "bow", 400, 20 + 20 * (game.CurrentLevel / 5f), 10, "arrow", -1, 3f);
            monsterWeapon.Refresh();
            MonsterData rangedData = MonsterData.RangedMonster;
            rangedData.weapon = monsterWeapon;
            rangedData.Health += (int)(rangedData.Health * (game.CurrentLevel / 5f));

            Entity monsterMelee = game.entityFactory.CreateMeleeWeaponEntity("Monstersword", "sword", 50, 15 + 15 * (game.CurrentLevel / 5f), -1, 2f, game.resourceManager.GetResource<SpriteSheet>("SwordMeleeSpriteSheet"));
            monsterMelee.Refresh();
            MonsterData meleeData = MonsterData.BasicMonster;
            meleeData.weapon = monsterMelee;
            meleeData.Health += (int)(meleeData.Health * (game.CurrentLevel / 5f));

            int numRanged = 2 + 2 * game.CurrentLevel;
            int numMelee = 2 + 2 * game.CurrentLevel;

            switch (game.CurrentLevel)
            {
                case 1:
                    numRanged = 0;
                    break;
                case 2:
                    numRanged /= 2;
                    numMelee /= 2;
                    break;
            }

            Entity e = game.entityFactory.CreateSpawner(100, 100, game.resourceManager.GetResource<SpriteSheet>("playerSheet"), new Rectangle(0, 0, 46, 46), rangedData, numRanged / 2, new TimeSpan(0, 0, 0, 0, 1000));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 100, game.resourceManager.GetResource<SpriteSheet>("monsterSheet_JUNK"), new Rectangle(0, 0, 46, 46), meleeData, numMelee / 2, new TimeSpan(0, 0, 0, 0, 2000));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(100, 300, game.resourceManager.GetResource<SpriteSheet>("playerSheet"), new Rectangle(0, 0, 46, 46), rangedData, numRanged / 2, new TimeSpan(0, 0, 0, 0, 3000));
            e.Refresh();
            spawners.Add(e);
            e = game.entityFactory.CreateSpawner(300, 300, game.resourceManager.GetResource<SpriteSheet>("monsterSheet_JUNK"), new Rectangle(0, 0, 46, 46), meleeData, numMelee / 2, new TimeSpan(0, 0, 0, 0, 500));
            e.Refresh();
            spawners.Add(e);
        }
        private void AdvanceLevel()
        {
            game.CurrentLevel++;
        }
        public override void OnEntityRemoved(Entity e)
        {
            if (e.HasComponent<MonsterComponent>())
            {
                monstersdefeated++;
                if (--monstersalive == 0)
                {
                    /*
                    game.GameWon = true;

                    Event gamestate = new Event();
                    gamestate.name = "GameStateGameOver";
                    EventManager.Instance.TriggerEvent(gamestate);*/


                    //next game round
                    AdvanceLevel();
                    SetupNextRound();
                }
            }
            else if (e.HasComponent<PlayerComponent>())
            {
                game.GameWon = false;

                Event gamestate = new Event();
                gamestate.name = "GameStateGameOver";
                EventManager.Instance.TriggerEvent(gamestate);
            }
        }

        public override void ProcessEntities(IEnumerable<Entity> entities, float dt)
        {
            if (roundLblTime > 0)
            {
                roundLblTime -= dt;
                if (roundLblTime <= 0)
                {
                    roundLblTime = 0;
                    roundLabel.Visibility = CoreUI.Visibility.Hidden;
                }
            }
            for (int i = 0; i < entities.Count(); i++)
            {
                Entity e = entities.ElementAt(i);

                HealthComponent hc = e.GetComponent<HealthComponent>();
                if (hc.CurrentHealth <= 0)//dead
                {
                    World.DestroyEntity(e);
                    i--;
                }

            }
        }

        public override void Initialize()
        {
            monstersdefeated = 0;

            roundLabel = new Label();
            roundLabel.Size = new System.Drawing.Point(200, 40);
            roundLabel.Position = new System.Drawing.Point(200, 200);
            roundLabel.Foreground = new MonoGameColor(Microsoft.Xna.Framework.Color.Black);
            roundLabel.TextPosition = TextPosition.Center;
            roundLabel.Visibility = CoreUI.Visibility.Hidden;
            game.UIEngine.Children.AddElement(roundLabel);
        }

        public GameLogicSystem(Dirty game) : base(SystemDescriptions.GameLogicSystem.Aspect, SystemDescriptions.GameLogicSystem.Priority)
        {
            this.game = game;
        }
    }
}
