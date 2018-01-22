using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public class Spritesheet<T> : Image
    {
        public int CurrentFrame;
        public float Rate = 1;
        public bool UseRawDeltaTime;
        public Action<T> OnFinish;
        public Action<T> OnLoop;
        public Action<T> OnAnimate;

        private Dictionary<T, Animation> animations;
        private Animation currentAnimation;
        private float animationTimer;
        private bool played;

        public Spritesheet(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
            : base(texture, true)
        {
            SetFrames(texture, frameWidth, frameHeight, frameSep);
            animations = new Dictionary<T, Animation>();
        }

        public void SetFrames(MTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
        {
            List<MTexture> frames = new List<MTexture>();
            int x = 0, y = 0;

            while (y <= texture.Height - frameHeight)            
            {
                while (x <= texture.Width - frameWidth)
                {
                    frames.Add(texture.GetSubtexture(x, y, frameWidth, frameHeight));
                    x += frameWidth + frameSep;
                }

                y += frameHeight + frameSep;
                x = 0;
            }

            Frames = frames.ToArray();
        }

        public override void Update()
        {
            if (Animating && currentAnimation.Delay > 0)
            {
                //Timer
                if (UseRawDeltaTime)
                    animationTimer += Engine.RawDeltaTime * Rate;
                else
                    animationTimer += Engine.DeltaTime * Rate;

                //Next Frame
                if (Math.Abs(animationTimer) >= currentAnimation.Delay)
                {
                    CurrentAnimationFrame += Math.Sign(animationTimer);
                    animationTimer -= Math.Sign(animationTimer) * currentAnimation.Delay;

                    //End of Animation
                    if (CurrentAnimationFrame < 0 || CurrentAnimationFrame >= currentAnimation.Frames.Length)
                    {
                        //Looped
                        if (currentAnimation.Loop)
                        {
                            CurrentAnimationFrame -= Math.Sign(CurrentAnimationFrame) * currentAnimation.Frames.Length;
                            CurrentFrame = currentAnimation.Frames[CurrentAnimationFrame];

                            if (OnAnimate != null)
                                OnAnimate(CurrentAnimationID);
                            if (OnLoop != null)
                                OnLoop(CurrentAnimationID);
                        }
                        else
                        {
                            //Ended
                            if (CurrentAnimationFrame < 0)
                                CurrentAnimationFrame = 0;
                            else
                                CurrentAnimationFrame = currentAnimation.Frames.Length - 1;

                            Animating = false;
                            animationTimer = 0;
                            if (OnFinish != null)
                                OnFinish(CurrentAnimationID);
                        }
                    }
                    else
                    {
                        //Continue Animation
                        CurrentFrame = currentAnimation.Frames[CurrentAnimationFrame];
                        if (OnAnimate != null)
                            OnAnimate(CurrentAnimationID);
                    }
                }
            }
        }

        public override void Render()
        {
            Texture = Frames[CurrentFrame];
            base.Render();
        }

        #region Animation Definition

        public void Add(T id, bool loop, float delay, params int[] frames)
        {
#if DEBUG
            foreach (var i in frames)
                if (i >= Frames.Length)
                    throw new IndexOutOfRangeException("Specified frames is out of max range for this Spritesheet");
#endif

            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = frames,
                Loop = loop,
            };
        }

        public void Add(T id, float delay, params int[] frames)
        {
            Add(id, true, delay, frames);
        }

        public void Add(T id, int frame)
        {
            Add(id, false, 0, frame);
        }

        public void ClearAnimations()
        {
            animations.Clear();
        }

        #endregion

        #region Animation Playback

        public bool IsPlaying(T id)
        {
            if (!played)
                return false;
            else if (CurrentAnimationID == null)
                return id == null;
            else
                return CurrentAnimationID.Equals(id);
        }

        public void Play(T id, bool restart = false)
        {
            if (!IsPlaying(id) || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id.ToString());
#endif
                CurrentAnimationID = id;
                currentAnimation = animations[id];
                animationTimer = 0;              
                CurrentAnimationFrame = 0;
                played = true;

                Animating = currentAnimation.Frames.Length > 1;
                CurrentFrame = currentAnimation.Frames[0];
            }
        }

        public void Reverse(T id, bool restart = false)
        {
            Play(id, restart);
            if (Rate > 0)
                Rate *= -1;
        }

        public void Stop()
        {
            Animating = false;
            played = false;
        }

        #endregion

        #region Properties

        public MTexture[] Frames
        {
            get; private set;
        }

        public bool Animating
        {
            get; private set;
        }

        public T CurrentAnimationID
        {
            get; private set;
        }

        public int CurrentAnimationFrame
        {
            get; private set;
        }

        public override float Width
        {
            get
            {
                if (Frames.Length > 0)
                    return Frames[0].Width;
                else
                    return 0;
            }
        }

        public override float Height
        {
            get
            {
                if (Frames.Length > 0)
                    return Frames[0].Height;
                else
                    return 0;
            }
        }

        #endregion

        private struct Animation
        {
            public float Delay;
            public int[] Frames;
            public bool Loop;
        }
    }
}
