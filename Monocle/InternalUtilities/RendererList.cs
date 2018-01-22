using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public class RendererList
    {
        public List<Renderer> Renderers;
        private List<Renderer> adding;
        private List<Renderer> removing;
        private Scene scene;

        internal RendererList(Scene scene)
        {
            this.scene = scene;

            Renderers = new List<Renderer>();
            adding = new List<Renderer>();
            removing = new List<Renderer>();
        }
        
        internal void UpdateLists()
        {
            if (adding.Count > 0)
                foreach (var renderer in adding)
                    Renderers.Add(renderer);
            adding.Clear();
            if (removing.Count > 0)
                foreach (var renderer in removing)
                    Renderers.Remove(renderer);
            removing.Clear();
        }

        internal void Update()
        {
            foreach (var renderer in Renderers)
                renderer.Update(scene);
        }

        internal void BeforeRender()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible)
                    continue;
                Draw.Renderer = Renderers[i];
                Renderers[i].BeforeRender(scene);
            }
        }

        internal void Render()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible)
                    continue;
                Draw.Renderer = Renderers[i];
                Renderers[i].Render(scene);
            }
        }

        internal void AfterRender()
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                if (!Renderers[i].Visible)
                    continue;
                Draw.Renderer = Renderers[i];
                Renderers[i].AfterRender(scene);
            }
        }

        public void MoveToFront(Renderer renderer)
        {
            Renderers.Remove(renderer);
            Renderers.Add(renderer);
        }

        public void Add(Renderer renderer)
        {
            adding.Add(renderer);
        }

        public void Remove(Renderer renderer)
        {
            removing.Add(renderer);
        }

    }
}
