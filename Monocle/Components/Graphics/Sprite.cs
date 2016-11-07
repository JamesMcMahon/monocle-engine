using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class Sprite : Image
    {
        public float Rate = 1f;
        public bool UseRawDeltaTime;
        public Vector2? Justify;
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
            CurrentAnimationID = "";
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
                            CurrentAnimationID = LastAnimationID = currentAnimation.Goto.Choose();
                            currentAnimation = animations[LastAnimationID];
                            if (CurrentAnimationFrame < 0)
                                CurrentAnimationFrame = currentAnimation.Frames.Length - 1;
                            else
                                CurrentAnimationFrame = 0;

                            SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                            OnAnimate?.Invoke(CurrentAnimationID);
                            OnLoop?.Invoke(CurrentAnimationID);
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
                        SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                        if (OnAnimate != null)
                            OnAnimate(CurrentAnimationID);
                    }
                }
            }
        }

        private void SetFrame(MTexture texture)
        {
            Texture = texture;
            if (Justify.HasValue)
                Origin = new Vector2(Texture.Width * Justify.Value.X, Texture.Height * Justify.Value.Y);
        }

		public void SetAnimationFrame(int frame)
		{
			animationTimer = 0;
			CurrentAnimationFrame = frame % currentAnimation.Frames.Length;
			SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
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
                LastAnimationID = CurrentAnimationID = id;
                currentAnimation = animations[id];
                animationTimer = 0;
                Animating = currentAnimation.Delay > 0;
                CurrentAnimationFrame = 0;
                SetFrame(currentAnimation.Frames[0]);
            }
        }

        public void PlayOffset(string id, float offset, bool restart = false)
        {
            if (CurrentAnimationID != id || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id.ToString());
#endif
                LastAnimationID = CurrentAnimationID = id;
                currentAnimation = animations[id];

                if (currentAnimation.Delay > 0)
                {
                    Animating = true;
                    float at = (currentAnimation.Delay * currentAnimation.Frames.Length) * offset;

                    CurrentAnimationFrame = 0;
                    while (at >= currentAnimation.Delay)
                    {
                        CurrentAnimationFrame++;
                        at -= currentAnimation.Delay;
                    }

                    CurrentAnimationFrame %= currentAnimation.Frames.Length;
                    animationTimer = at;
                    SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                }
                else
                {
                    animationTimer = 0;
                    Animating = false;
                    CurrentAnimationFrame = 0;
                    SetFrame(currentAnimation.Frames[0]);
                }
            }
        }

		public IEnumerator PlayRoutine(string id, bool restart = false)
		{
			Play(id, restart);
            return PlayUtil();
		}

        private IEnumerator PlayUtil()
        {
            while (Animating)
                yield return null;
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

        public string LastAnimationID
        {
            get; private set;
        }

        public int CurrentAnimationFrame
        {
            get; private set;
        }

        public int CurrentAnimationTotalFrames
        {
            get
            {
                if (currentAnimation != null)
                    return currentAnimation.Frames.Length;
                else
                    return 0;
            }
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

        public void LogAnimations()
        {
            StringBuilder str = new StringBuilder();

            foreach (var kv in animations)
            {
                var anim = kv.Value;

                str.Append(kv.Key);
                str.Append("\n{\n\t");
                str.Append(string.Join("\n\t", (object[])anim.Frames));
                str.Append("\n}\n");          
            }

            Calc.Log(str.ToString());
        }

        private class Animation
        {
            public float Delay;
            public MTexture[] Frames;
            public Chooser<string> Goto;
        }
    }
}
