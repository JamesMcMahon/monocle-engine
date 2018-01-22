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
        public Action<string> OnFrameChange;
        public Action<string> OnLastFrame;
        public Action<string, string> OnChange;

        private Atlas atlas;
        public string Path;
        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;
        private float animationTimer;
        private int width;
        private int height;

        public Sprite(Atlas atlas, string path)
            : base(null, true)
        {
            this.atlas = atlas;
            this.Path = path;
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);
            CurrentAnimationID = "";
        }

        public void Reset(Atlas atlas, string path)
        {
            this.atlas = atlas;
            this.Path = path;
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);

            currentAnimation = null;
            CurrentAnimationID = "";
            OnFinish = null;
            OnLoop = null;
            OnFrameChange = null;
            OnChange = null;
            Animating = false;
        }

        public MTexture GetFrame(string animation, int frame)
        {
            return animations[animation].Frames[frame];
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(Width / 2, Height / 2);
            }
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
                        var was = CurrentAnimationID;
                        if (OnLastFrame != null)
                            OnLastFrame(CurrentAnimationID);

                        // only do stuff if OnLastFrame didn't just change the animation
                        if (was == CurrentAnimationID)
                        {
                            //Looped
                            if (currentAnimation.Goto != null)
                            {
                                CurrentAnimationID = currentAnimation.Goto.Choose();

                                if (OnChange != null)
                                    OnChange(LastAnimationID, CurrentAnimationID);

                                LastAnimationID = CurrentAnimationID;
                                currentAnimation = animations[LastAnimationID];
                                if (CurrentAnimationFrame < 0)
                                    CurrentAnimationFrame = currentAnimation.Frames.Length - 1;
                                else
                                    CurrentAnimationFrame = 0;

                                SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
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
                                CurrentAnimationID = "";
                                currentAnimation = null;
                                animationTimer = 0;
                                if (OnFinish != null)
                                    OnFinish(id);
                            }
                        }
                    }
                    else
                    {
                        //Continue Animation
                        SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                    }
                }
            }
        }

        private void SetFrame(MTexture texture)
        {
            if (texture == Texture)
                return;

            Texture = texture;
            if (Justify.HasValue)
                Origin = new Vector2(Texture.Width * Justify.Value.X, Texture.Height * Justify.Value.Y);
            if (OnFrameChange != null)
                OnFrameChange(CurrentAnimationID);
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

            if (frames == null || frames.Length <= 0)
                ret = atlas.GetAtlasSubtextures(this.Path + path).ToArray();
            else
            {
                var fullPath = this.Path + path;
                var finalFrames = new MTexture[frames.Length];
                for (int i = 0; i < frames.Length; i++)
                {
                    var frame = atlas.GetAtlasSubtexturesAt(fullPath, frames[i]);
                    if (frame == null)

                        throw new Exception("Can't find sprite " + fullPath + " with index " + frames[i]); 
                    finalFrames[i] = frame;
                }
                ret = finalFrames;
            }

#if DEBUG
            if (ret.Length == 0)
                throw new Exception("No frames found for animation path '" + this.Path + path + "'!");
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
        
        public void Play(string id, bool restart = false, bool randomizeFrame = false)
        {
            if (CurrentAnimationID != id || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id);
#endif
                if (OnChange != null)
                    OnChange(LastAnimationID, id);

                LastAnimationID = CurrentAnimationID = id;
                currentAnimation = animations[id];
                Animating = currentAnimation.Delay > 0;

                if (randomizeFrame)
                {
                    animationTimer = Calc.Random.NextFloat(currentAnimation.Delay);
                    CurrentAnimationFrame = Calc.Random.Next(currentAnimation.Frames.Length);
                }
                else
                {
                    animationTimer = 0;
                    CurrentAnimationFrame = 0;
                }

                SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
            }
        }

        public void PlayOffset(string id, float offset, bool restart = false)
        {
            if (CurrentAnimationID != id || restart)
            {
#if DEBUG
                if (!animations.ContainsKey(id))
                    throw new Exception("No Animation defined for ID: " + id);
#endif
                if (OnChange != null)
                    OnChange(LastAnimationID, id);

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

        public IEnumerator ReverseRoutine(string id, bool restart = false)
        {
            Reverse(id, restart);
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
            return id != null && animations.ContainsKey(id);
        }

        public void Stop()
        {
            Animating = false;
            currentAnimation = null;
            CurrentAnimationID = "";
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

        #region Cloning from SpriteBank

        internal Sprite()
            : base(null, true)
        {

        }

        internal Sprite CreateClone()
        {
            return CloneInto(new Sprite());
        }

        internal Sprite CloneInto(Sprite clone)
        {
            clone.Texture = Texture;
            clone.Position = Position;
            clone.Justify = Justify;
            clone.Origin = Origin;

            clone.animations = new Dictionary<string, Animation>(animations, StringComparer.OrdinalIgnoreCase);
            clone.currentAnimation = currentAnimation;
            clone.animationTimer = animationTimer;
            clone.width = width;
            clone.height = height;

            clone.Animating = Animating;
            clone.CurrentAnimationID = CurrentAnimationID;
            clone.LastAnimationID = LastAnimationID;
            clone.CurrentAnimationFrame = CurrentAnimationFrame;

            return clone;
        }

        #endregion

        public void DrawSubrect(Vector2 offset, Rectangle rectangle)
        {
            if (Texture != null)
            {
                var clip = Texture.GetRelativeRect(rectangle);
                var clipOffset = new Vector2(-Math.Min(rectangle.X - Texture.DrawOffset.X, 0), -Math.Min(rectangle.Y - Texture.DrawOffset.Y, 0));
                Draw.SpriteBatch.Draw(Texture.Texture, RenderPosition + offset, clip, Color, Rotation, Origin - clipOffset, Scale, Effects, 0);
            }
        }

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
