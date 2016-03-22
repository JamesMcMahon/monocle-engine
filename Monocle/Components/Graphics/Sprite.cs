using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Sprite<T> : Image
    {
        private MTexture[] frames;

        public int CurrentFrame;
        public float Rate = 1f;

        public Sprite(MTexture atlas, string key)
            : base(null, true)
        {
            frames = atlas.GetAtlasSubtextures(key).ToArray();
        }

        public override void Render()
        {
            if (CurrentFrame >= 0 && CurrentFrame < frames.Length)
                Texture = frames[CurrentFrame];
            else
                Texture = null;
            base.Render();
        }

        public void Add(T id, bool loop, float delay, params int[] frames)
        {
            
        }

        private struct Animation
        {
            public float Delay;
            public int[] Frames;
            public bool Loop;
            public Action<int> OnFrameChange;
            public Action OnComplete;
        }
    }
}
