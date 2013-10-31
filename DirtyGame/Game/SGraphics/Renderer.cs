﻿using System;
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

        //I don't know why I have to do this...
        private Map.Map map;

        public Map.Map ActiveMap
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

        public void Render()
        {
            device.Clear(Color.CornflowerBlue);
            map.draw(camera);


           

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
