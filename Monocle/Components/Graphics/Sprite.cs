using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Sprite : Image
    {
        public float Rate = 1f;
        public bool UseRawDeltaTime;
        public Action<string> OnFinish;
        public Action<string> OnLoop;
        public Action<string> OnAnimate;

        private MTexture atlas;
        private string path;
        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;
        private float animationTimer;
        private int width;
        private int height;

        public Sprite(MTexture atlas, string path)
            : base(null, true)
        {
            this.atlas = atlas;
            this.path = path;

            animations = new Dictionary<string, Animation>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Reset(MTexture atlas, string path)
        {
            this.atlas = atlas;
            this.path = path;
            animations = new Dictionary<string, Animation>(StringComparer.InvariantCultureIgnoreCase);
            currentAnimation = null;
            OnFinish = null;
            OnLoop = null;
            OnAnimate = null;
            Animating = false;
        }

        public override void Update()
        {
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
                    CurrentAnimationFrame += Math.Sign(animationTimer);
                    animationTimer -= Math.Sign(animationTimer) * currentAnimation.Delay;

                    //End of Animation
                    if (CurrentAnimationFrame < 0 || CurrentAnimationFrame >= currentAnimation.Frames.Length)
                    {
                        //Looped
                        if (currentAnimation.Goto != null)
                        {
                            currentAnimation = animations[currentAnimation.Goto.Choose()];
                            if (CurrentAnimationFrame < 0)
                                CurrentAnimationFrame = currentAnimation.Frames.Length - 1;
                            else
                                CurrentAnimationFrame = 0;

                            CurrentAnimationFrame -= Math.Sign(CurrentAnimationFrame) * currentAnimation.Frames.Length;
                            Texture = currentAnimation.Frames[CurrentAnimationFrame];

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
                            var id = CurrentAnimationID;
                            CurrentAnimationID = null;
                            currentAnimation = null;
                            animationTimer = 0;
                            if (OnFinish != null)
                                OnFinish(id);
                        }
                    }
                    else
                    {
                        //Continue Animation
                        Texture = currentAnimation.Frames[CurrentAnimationFrame];
                        if (OnAnimate != null)
                            OnAnimate(CurrentAnimationID);
                    }
                }
            }
        }

        #region Define Animations

        public void AddLoop(string id, string path, float delay)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path),
                Goto = new Chooser<string>(id, 1f)
            };
        }

		public void AddLoop(string id, string path, float delay, params int[] frames)
		{
			animations[id] = new Animation()
			{
				Delay = delay,
				Frames = GetFrames(path, frames),
				Goto = new Chooser<string>(id, 1f)
			};
		}

		public void Add(string id, string path)
        {
            animations[id] = new Animation()
            {
                Delay = 0,
                Frames = GetFrames(path),
                Goto = null
            };
        }

        public void Add(string id, string path, float delay)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path),
                Goto = null
            };
        }

        public void Add(string id, string path, float delay, params int[] frames)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path, frames),
                Goto = null
            };
        }

        public void Add(string id, string path, float delay, string into)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path),
                Goto = Chooser<string>.FromString<string>(into)
            };
        }

        public void Add(string id, string path, float delay, Chooser<string> into)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path),
                Goto = into
            };
        }

        public void Add(string id, string path, float delay, string into, params int[] frames)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path, frames),
                Goto = Chooser<string>.FromString<string>(into)
            };
        }

        public void Add(string id, string path, float delay, Chooser<string> into, params int[] frames)
        {
            animations[id] = new Animation()
            {
                Delay = delay,
                Frames = GetFrames(path, frames),
                Goto = into
            };
        }

        private MTexture[] GetFrames(string path, int[] frames = null)
        {
            MTexture[] ret;

            if (frames == null)
                ret = atlas.GetAtlasSubtextures(this.path + path).ToArray();
            else
            {
                var allFrames = atlas.GetAtlasSubtextures(this.path + path).ToArray();
                var finalFrames = new MTexture[frames.Length];
                for (int i = 0; i < frames.Length; i++)
                    finalFrames[i] = allFrames[frames[i]];
                ret = finalFrames;
            }

#if DEBUG
            if (ret.Length == 0)
                throw new Exception("No frames found for animation path '" + this.path + path + "'!");
#endif

            width = Math.Max(ret[0].Width, width);
            height = Math.Max(ret[0].Height, height);

            return ret;
        }

        public void ClearAnimations()
        {
            animations.Clear();
        }

        #endregion

        #region Animation Playback

        public void Play(string id, bool restart = false)
        {
            if (CurrentAnimationID != id || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id.ToString());
#endif
                CurrentAnimationID = id;
                currentAnimation = animations[id];
                animationTimer = 0;
                Animating = currentAnimation.Frames.Length > 1;
                CurrentAnimationFrame = 0;
                Texture = currentAnimation.Frames[0];
            }
        }

        public void Reverse(string id, bool restart = false)
        {
            Play(id, restart);
            if (Rate > 0)
                Rate *= -1;
        }

        public bool Has(string id)
        {
            return animations.ContainsKey(id);
        }

        public void Stop()
        {
            Animating = false;
            currentAnimation = null;
            CurrentAnimationID = null;
        }

        #endregion     

        #region Properties

        public bool Animating
        {
            get; private set;
        }

        public string CurrentAnimationID
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
                return width;
            }
        }

        public override float Height
        {
            get
            {
                return height;
            }
        }

        #endregion

        private class Animation
        {
            public float Delay;
            public MTexture[] Frames;
            public Chooser<string> Goto;
        }
    }
}
