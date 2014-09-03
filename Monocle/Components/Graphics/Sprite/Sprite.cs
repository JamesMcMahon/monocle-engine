using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Sprite<T> : GraphicsComponent
    {
        public List<SpriteFrame> Frames { get; private set; }
        private Dictionary<T, SpriteAnimation> animations;
        private int currentFrame;

        public Sprite()
            : base(true)
        {
            Frames = new List<SpriteFrame>();
            animations = new Dictionary<T, SpriteAnimation>();
        }

        #region Adding Frames

        public void AddFrame(SpriteFrame frame)
        {
            Frames.Add(frame);
        }

        public void AddFrame(Subtexture subtexture, Vector2 origin)
        {
            AddFrame(new SpriteFrame(subtexture, origin));
        }

        public void AddFrame(Subtexture subtexture)
        {
            AddFrame(new SpriteFrame(subtexture));
        }

        public void AddFrames(params SpriteFrame[] frames)
        {
            foreach (var frame in frames)
                AddFrame(frame);
        }

        public void AddFrames(params Subtexture[] subtextures)
        {
            foreach (var subtexture in subtextures)
                AddFrame(subtexture);
        }

        #endregion

        #region Adding Animations

        public void AddAnimation(T id, int frame)
        {
            animations[id] =
                new SpriteAnimation()
                {
                    Frames = new int[] { frame },
                    Looping = false,
                    Delay = 0,
                };
        }

        public void AddAnimation(T id, bool looping, float delay, params int[] frames)
        {
            animations[id] =
                new SpriteAnimation()
                {
                    Frames = frames,
                    Looping = looping,
                    Delay = delay,
                };
        }

        #endregion

        public override void Update()
        {

        }

        public override void Render()
        {
            //Draw it
        }

        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }

            set
            {
                if (value < -1 || value >= Frames.Count)
                    throw new Exception("Sprite frame index out of range! For null frame, set index to -1.");
                else
                    currentFrame = value;
            }
        }

        public struct SpriteFrame
        {
            public Subtexture Subtexture;
            public Vector2 Origin;

            /// <summary>
            /// Create a SpriteFrame with a defined origin point
            /// </summary>
            /// <param name="subtexture"></param>
            /// <param name="origin"></param>
            public SpriteFrame(Subtexture subtexture, Vector2 origin)
            {
                Subtexture = subtexture;
                Origin = origin;
            }

            /// <summary>
            /// Create a SpriteFrame with a centered origin point
            /// </summary>
            /// <param name="subtexture"></param>
            public SpriteFrame(Subtexture subtexture)
            {
                Subtexture = subtexture;
                Origin = subtexture.Center;
            }
        }

        public struct SpriteAnimation
        {
            public int[] Frames;
            public float Delay;
            public bool Looping;
        }
    }
}
