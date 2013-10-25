﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenTK.Graphics;

namespace DirtyGame.game.SGraphics
{

    public abstract class RenderCommand    
    {
        public enum CommandType
        {         
            BeginBatchDraw,
            BatchDrawSprite,
            BatchDrawText,                       
        }

        private CommandType type;

        public CommandType Type
        {
            get
            {
                return type;
            }
        }

        public RenderCommand(CommandType type)
        {
            this.type = type;
        }
        
        public abstract void Execute(SpriteBatch spriteBatch);
    }

}
