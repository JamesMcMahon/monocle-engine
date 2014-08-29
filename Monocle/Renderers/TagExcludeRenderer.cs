using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class TagExcludeRenderer : Renderer
    {
        public BlendState BlendState;
        public SamplerState SamplerState;
        public Effect Effect;
        public Camera Camera;
        public int ExcludeTag;

        public TagExcludeRenderer(int excludeTag)
        {
            ExcludeTag = excludeTag;
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.LinearClamp;
            Camera = new Camera();
        }

        public override void BeforeRender(Scene scene)
        {

        }

        public override void Render(Scene scene)
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None, RasterizerState.CullNone, Effect, Camera.Matrix * Draw.MasterRenderMatrix);

            foreach (var entity in scene.Entities)
                if (entity.Visible && !entity.Tags.Contains(ExcludeTag))
                    entity.Render();

            if (Engine.Commands.Open)
                foreach (var entity in scene.Entities)
                    if (!entity.Tags.Contains(ExcludeTag))
                        entity.DebugRender();

            Draw.SpriteBatch.End();
        }

        public override void AfterRender(Scene scene)
        {

        }
    }
}
