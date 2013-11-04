using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirtyGame.game.SGraphics.Commands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DirtyGame.game.Map;

namespace DirtyGame.game.SGraphics
{
    public class Renderer
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private SpriteBatch spriteBatch;
        private List<RenderInstance> renderInstances;
        private Camera camera;
        private Camera cam2;

        //I don't know why I have to do this...
        private TileMap map;

        public TileMap ActiveMap
        {
            get
            {
                return map;
            }
            set
            {
                map = value;
            }
        }

        public Camera ActiveCamera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public Renderer(GraphicsDeviceManager graphics, Camera camera)
        {
            this.graphics = graphics;
            device = graphics.GraphicsDevice;
            renderInstances = new List<RenderInstance>();
            spriteBatch = new SpriteBatch(device);
            this.camera = camera; 
            cam2 = new Camera();
            cam2.Zoom(0.05f);
            renderTarget = new RenderTarget2D(
                device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                false,
                device.PresentationParameters.BackBufferFormat,
                DepthFormat.None);            
        }

        public void Submit(RenderInstance renderInstance)
        {
            renderInstances.Add(renderInstance);   
            
        }

        public void Submit(RenderGroup renderGroup)
        {
            // for now...just put the group commands in all of the instances
            foreach (RenderInstance instance in renderGroup.Instances)
            {
                foreach (RenderCommand command in renderGroup.Commands)
                {
                    instance.AddCommand(command);    
                }
                renderInstances.Add(instance);
            }
        }

        RenderTarget2D renderTarget;

        public void Render()
        {
            
            device.Clear(Color.Black);                        
            device.SetRenderTarget(renderTarget);
            device.Clear(Color.Black);

          
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam2.Transform);
            foreach (TileData tile in map.Tiles)
            {
                Rectangle srcRect;
                Rectangle dst = tile.DstRect;
                dst.Width = 1;
                dst.Height = 1;

                if (tile.Passable)
                {
                    srcRect = new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    srcRect = tile.SrcRect;
                    continue;
                }
                spriteBatch.Draw(map.Tileset.Texture, tile.DstRect, srcRect, Color.White);
            }
            spriteBatch.End();


            device.SetRenderTarget(null);
            


            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, ActiveCamera.Transform);
            foreach (TileData tile in map.Tiles)
            {
                Rectangle srcRect;
                if (tile.Passable)
                {
                    srcRect = new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    srcRect = tile.SrcRect;
                    continue;
                }
                spriteBatch.Draw(map.Tileset.Texture, tile.DstRect, srcRect, Color.White);
            }
            spriteBatch.End();




           

            // sort everything by sortkey
            renderInstances.Sort();

            BeginBatchDraw lastBeginBatchDraw = null;
            bool inBatch = false;

            foreach (RenderInstance instance in renderInstances)
            {
                foreach (RenderCommand command in instance.Commands)
                {
                    switch (command.Type)
                    {
                        case RenderCommand.CommandType.BatchDrawSprite:
                            // drawcall...shouldnt be in commands list
                            break;
                        case RenderCommand.CommandType.BatchDrawText:
                            // drawcall...shouldnt be in commands list
                            break;   
                        case RenderCommand.CommandType.BeginBatchDraw:
                            if (lastBeginBatchDraw == null)
                            {
                                lastBeginBatchDraw = (BeginBatchDraw) command;
                                inBatch = true;
                            }
                            else if (lastBeginBatchDraw.CompareTo((BeginBatchDraw) command) == -1)
                            {
                                spriteBatch.End();
                                lastBeginBatchDraw = (BeginBatchDraw) command;
                                inBatch = true;
                            }
                            else
                            {
                                // dont execute the command
                                continue;
                            }
                            break;   
                        default:
                            break;
                    }
                    command.Execute(spriteBatch);
                }
                instance.DrawCall.Execute(spriteBatch);
            }

            if (inBatch)
            {
                spriteBatch.End();
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
               SamplerState.LinearClamp, DepthStencilState.Default,
               RasterizerState.CullNone);

            spriteBatch.Draw(renderTarget, new Rectangle(0, 325, 200, 150), Color.White);

            spriteBatch.End();



            renderInstances.Clear();
        }

        public int GetViewportWidth()
        {
            return graphics.GraphicsDevice.Viewport.Width;
        }

        public int GetViewportHeight()
        {
            return graphics.GraphicsDevice.Viewport.Height;
        }
    }
}
