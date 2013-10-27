#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using DirtyGame.game.Core;
using DirtyGame.game.Core.Systems;
using DirtyGame.game.Core.Systems.Render;
using DirtyGame.game.SGraphics;
using DirtyGame.game.Systems;
using DirtyGame.game.Map;
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

        private Map map;

        public Dirty()
        {
            graphics = new GraphicsDeviceManager(this);
            resourceManager = new ResourceManager(Content);                       
            renderer = new Renderer(graphics, new Camera());
            world = new World();
            entityFactory = new EntityFactory(world.EntityMgr, resourceManager);
            world.AddSystem(new SpriteRenderSystem(renderer));
            world.AddSystem(new PlayerControlSystem());
            world.AddSystem(new CameraUpdateSystem(renderer));
            world.AddSystem(new MapBoundarySystem(renderer));
            map = new Map(graphics.GraphicsDevice);
            
            Entity e = entityFactory.CreatePlayerEntity();
            e.Refresh();
        }

        protected override void LoadContent()
        {
            map.LoadMap("Cave.tmx", graphics.GraphicsDevice, Content);
            renderer.ActiveMap = map;
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
