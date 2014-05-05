using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class EverythingRenderer : Renderer
    {
        public BlendState BlendState;
        public SamplerState SamplerState;
        public Effect Effect;
        public Camera Camera;

        public EverythingRenderer()
        {
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

            scene.Entities.Render();
            if (Engine.Instance.Commands.Open)
                scene.Entities.DebugRender();

            Draw.SpriteBatch.End();
        }

        public override void AfterRender(Scene scene)
        {

        }
    }
}
