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
        public bool Animating { get; private set; }
        public bool Finished { get; private set; }
        public float Rate;

        private int currentFrame;
        private Dictionary<T, SpriteAnimation> animations;
        private SpriteAnimation currentAnimation;
        private T currentAnimationID;
        private float delayCounter;
        private int currentAnimationFrame;

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
#if DEBUG
            if (frame < -1 || frame >= Frames.Count)
                throw new Exception("Specified frame index is out of range. For null frame, use index -1.");
#endif

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
#if DEBUG
            if (frames.Length == 0)
                throw new Exception("Animation must have at least one frame. For null frame, use index -1.");
            foreach (var frame in frames)
                if (frame < -1 || frame >= Frames.Count)
                    throw new Exception("Specified frame index is out of range. For null frame, use index -1.");
            if (delay <= 0)
                throw new Exception("Frame delay must be larger than zero.");
#endif

            animations[id] =
                new SpriteAnimation()
                {
                    Frames = frames,
                    Looping = looping,
                    Delay = delay,
                };
        }

        #endregion

        #region Animation Control

        public void Play(T id, bool restart = true)
        {
#if DEBUG
            if (!animations.ContainsKey(id))
                throw new Exception("Animation id '" + id.ToString() + "' is undefined");
#endif

            if (restart || !currentAnimationID.Equals(id))
            {
                currentAnimationID = id;
                currentAnimation = animations[id];
                Animating = true;
                delayCounter = 0;
                currentAnimationFrame = 0;
                currentFrame = currentAnimation.Frames[0];
                Finished = (currentAnimation.Frames.Length == 1);
            }
        }

        public void Stop()
        {
            currentAnimationID = default(T);

        }

        #endregion

        public override void Update()
        {

        }

        public override void Render()
        {
            //Draw it!
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
                {
                    Animating = false;
                    Finished = false;
                    currentFrame = value;
                    CurrentAnimation = default(T);
                }
            }
        }

        public T CurrentAnimation
        {
            get
            {
                return currentAnimationID;
            }

            set
            {
                Play(currentAnimationID, false);
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
