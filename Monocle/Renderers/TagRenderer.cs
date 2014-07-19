using Microsoft.Xna.Framework.Graphics;

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

        public override void BeforeRender(Scene scene)
        {

        }

        public override void Render(Scene scene)
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None, RasterizerState.CullNone, Effect, Camera.Matrix);

            foreach (var entity in scene[Tag])
                if (entity.Visible)
                    entity.Render();

            if (Engine.Commands.Open)
                foreach (var entity in scene[Tag])
                    entity.DebugRender();

            Draw.SpriteBatch.End();
        }

        public override void AfterRender(Scene scene)
        {

        }
    }
}
