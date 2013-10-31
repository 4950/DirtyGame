#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using DirtyGame.game.Core;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Systems;
using EntityFramework;
using EntityFramework.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

#endregion

namespace DirtyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Dirty : Game
    {

        private World world;
        private GraphicsDeviceManager graphics;
        private Renderer renderer;
        private EntityFactory entityFactory;
        private ResourceManager resourceManager;

        public Dirty()
        {
            graphics = new GraphicsDeviceManager(this);
            resourceManager = new ResourceManager(Content);                       
            Camera cam = new Camera();
            renderer = new Renderer(graphics, cam);
            world = new World();
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            world.AddSystem(new SpriteRenderSystem(renderer));
            AnimationSystem animationSys = new AnimationSystem();
            world.AddSystem(animationSys);

            //Testing purposes can be changed
            SpriteSheet playerSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("playerSheet"), "Content\\PlayerAnimation.xml");

            Entity e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(0.0f, 0.0f), "Up");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(50.0f, 0.0f), "Down");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(100.0f, 0.0f), "Left");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(150.0f, 0.0f), "Right");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(200.0f, 0.0f), "IdleUp");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(250.0f, 0.0f), "IdleDown");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(300.0f, 0.0f), "IdleLeft");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(350.0f, 0.0f), "IdleRight");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(0.0f, 50.0f), "AttackUp");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(100.0f, 50.0f), "AttackDown");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(200.0f, 50.0f), "AttackLeft");
            e.Refresh();
            e = entityFactory.CreateTestEntity(playerSpriteSheet, new Vector2(300.0f, 50.0f), "AttackRight");
            e.Refresh();

            SpriteSheet pokemonNPCSpriteSheet = new SpriteSheet(resourceManager.GetResource<Texture2D>("Pokemon NPC Sprite Sheet"), "Content\\PokemonNPCAnimation.xml");

            e = entityFactory.CreateTestEntity(pokemonNPCSpriteSheet, new Vector2(0.0f, 200.0f), "WalkLeft");
            e.Refresh();
        }

        protected override void LoadContent()
        {
        
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            renderer.Render();
            base.Draw(gameTime);
        }
    }
}
