using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Sprite<T> : Image
    {
        private MTexture[] frames;
        private Dictionary<T, Animation> animations;

        public int CurrentFrame;
        public float Rate = 1f;
        public bool UseRawDeltaTime;
        public Action<T> OnFinish;
        public Action<T> OnLoop;
        public Action<T> OnAnimate;

        private bool Animating;
        private Animation currentAnimation;
        private T currentAnimationID;
        private int currentAnimationFrame;
        private float animationTimer;

        public Sprite(MTexture atlas, string key)
            : base(null, true)
        {
            frames = atlas.GetAtlasSubtextures(key).ToArray();
            animations = new Dictionary<T, Animation>();
        }

        public override void Update()
        {
            base.Update();

            if (Animating)
            {
                //Timer
                if (UseRawDeltaTime)
                    animationTimer += Engine.RawDeltaTime * Rate;
                else
                    animationTimer += Engine.DeltaTime * Rate;

                //Next Frame
                if (Math.Abs(animationTimer) >= currentAnimation.Delay)
                {
                    currentAnimationFrame += Math.Sign(animationTimer);
                    animationTimer -= Math.Sign(animationTimer) * currentAnimation.Delay;

                    //End of Animation
                    if (currentAnimationFrame < 0 || currentAnimationFrame >= currentAnimation.Frames.Length)
                    {
                        //Looped
                        if (currentAnimation.Loop)
                        {
                            currentAnimationFrame -= Math.Sign(currentAnimationFrame) * currentAnimation.Frames.Length;
                            CurrentFrame = currentAnimation.Frames[currentAnimationFrame];

                            if (OnAnimate != null)
                                OnAnimate(currentAnimationID);
                            if (OnLoop != null)
                                OnLoop(currentAnimationID);
                        }
                        else
                        {
                            //Ended
                            if (currentAnimationFrame < 0)
                                currentAnimationFrame = 0;
                            else
                                currentAnimationFrame = currentAnimation.Frames.Length - 1;

                            Animating = false;
                            animationTimer = 0;
                            if (OnFinish != null)
                                OnFinish(currentAnimationID);
                        }
                    }
                    else
                    {
                        //Continue Animation
                        CurrentFrame = currentAnimation.Frames[currentAnimationFrame];
                        if (OnAnimate != null)
                            OnAnimate(currentAnimationID);
                    }
                }
            }
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
            animations[id] = new Animation()
            {
                Loop = loop,
                Delay = delay,
                Frames = frames
            };
        }

        public void Add(T id, float delay, params int[] frames)
        {
            Add(id, false, delay, frames);
        }

        public void Add(T id, int frame)
        {
            Add(id, false, .1f, frame);
        }

        public void Play(T id, bool restart = false)
        {
            if (!Animating || !currentAnimation.Equals(id) || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id.ToString());
#endif
                currentAnimationID = id;
                currentAnimation = animations[id];
                animationTimer = 0;
                Animating = currentAnimation.Frames.Length > 1;
                currentAnimationFrame = 0;

                CurrentFrame = currentAnimation.Frames[0];
            }
        }

        public void Reverse(T id, bool restart = false)
        {
            Play(id, restart);
            if (Rate > 0)
                Rate *= -1;
        }

        public void SetFrames(MTexture atlas, string key)
        {
            frames = atlas.GetAtlasSubtextures(key).ToArray();
        }

        public void ClearAnimations()
        {
            animations.Clear();
        }

        public override float Width
        {
            get
            {
                if (frames.Length > 0)
                    return frames[0].Width;
                else
                    return 0;
            }
        }

        public override float Height
        {
            get
            {
                if (frames.Length > 0)
                    return frames[0].Height;
                else
                    return 0;
            }
        }

        private struct Animation
        {
            public float Delay;
            public int[] Frames;
            public bool Loop;
        }
    }
}
