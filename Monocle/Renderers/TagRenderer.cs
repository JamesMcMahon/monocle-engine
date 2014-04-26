using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagRenderer : Renderer
    {
        public int Tag;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public Effect Effect;
        public Camera Camera;

        public TagRenderer(int tag)
        {
            Tag = tag;
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointClamp;
            Camera = new Camera();
        }

        public override void Render(Scene scene)
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None, RasterizerState.CullNone, Effect, Camera.Matrix);

            foreach (var entity in scene.TagLists[Tag])
                if (entity.Visible)
                    entity.Render();

            if (Engine.Instance.Commands.Open)
                foreach (var entity in scene.Entities)
                    entity.DebugRender();

            Draw.SpriteBatch.End();
        }
    }
}
