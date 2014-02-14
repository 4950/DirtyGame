using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CleanGame.Game.SGraphics.Commands
{
    class BeginBatchDraw : RenderCommand, IComparable<BeginBatchDraw>
    {
        private SpriteSortMode sortMode;
        private BlendState blendState;
        private SamplerState samplerState;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;
        private Effect effect;
        private Matrix transformMatrix;

        public BeginBatchDraw(Matrix transform)
            : base(RenderCommand.CommandType.BeginBatchDraw)
        {
            sortMode = SpriteSortMode.BackToFront;
            blendState = BlendState.AlphaBlend;
            samplerState = SamplerState.LinearClamp;
            depthStencilState = DepthStencilState.Default;
            rasterizerState = RasterizerState.CullCounterClockwise;
            effect = null;
            transformMatrix = transform;
        }

        public override void Execute(SpriteBatch spriteBatch, Renderer r)
        {
        	spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public int CompareTo(BeginBatchDraw other)
        {
            if (sortMode != other.sortMode || blendState != other.blendState || samplerState != other.samplerState ||
                depthStencilState != other.depthStencilState || rasterizerState != other.rasterizerState ||
                effect != other.effect || !transformMatrix.Equals(other.transformMatrix))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }        
    }
}
