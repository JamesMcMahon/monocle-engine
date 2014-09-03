using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Monocle
{
    public class Spritesheet<T> : Image
    {
        public Action<Spritesheet<T>> OnAnimationComplete;
        public Action<Spritesheet<T>> OnAnimate;
        public Action<Spritesheet<T>> OnFrameChange;

        public int FramesX { get; private set; }
        public int FramesY { get; private set; }
        public int AnimationFrame { get; private set; }
        public bool Playing { get; private set; }
        public bool Finished { get; private set; }
        public T CurrentAnimID { get; private set; }
        public Rectangle[] FrameRects { get; private set; }
        public float Rate = 1;

        private Dictionary<T, SpritesheetAnimation> Animations;
        private int currentFrame;
        private SpritesheetAnimation currentAnim;
        private float timer;

        public Spritesheet(Texture texture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(texture, clipRect, true)
        {
            Initialize(frameWidth, frameHeight, frameSep);
        }

        public Spritesheet(Subtexture subTexture, Rectangle? clipRect, int frameWidth, int frameHeight, int frameSep = 0)
            : base(subTexture, clipRect, true)
        {
            Initialize(frameWidth, frameHeight, frameSep);
        }

        public Spritesheet(Texture texture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(texture, null, frameWidth, frameHeight, frameSep)
        {

        }

        public Spritesheet(Subtexture subTexture, int frameWidth, int frameHeight, int frameSep = 0)
            : this(subTexture, null, frameWidth, frameHeight, frameSep)
        {

        }

        private void Initialize(int frameWidth, int frameHeight, int frameSep)
        {
            Animations = new Dictionary<T, SpritesheetAnimation>();

            //Get the amounts of frames
            {
                for (int i = 0; i <= ClipRect.Width - frameWidth; i += frameWidth + frameSep)
                    FramesX++;
                for (int i = 0; i <= ClipRect.Height - frameHeight; i += frameHeight + frameSep)
                    FramesY++;
            }

            //Build the frame rects
            {
                FrameRects = new Rectangle[FramesTotal];
                int x = 0, y = 0;

                for (int i = 0; i < FramesTotal; i++)
                {
                    FrameRects[i] = new Rectangle(ClipRect.X + x, ClipRect.Y + y, frameWidth, frameHeight);

                    if ((i + 1) % FramesX == 0)
                    {
                        x = 0;
                        y += frameHeight + frameSep;
                    }
                    else
                        x += frameWidth + frameSep;
                }
            }
        }

        public override void Update()
        {
            if (Playing && currentAnim.Delay > 0)
            {
                timer += Engine.DeltaTime * Math.Abs(Rate);

                while (timer >= currentAnim.Delay)
                {
                    int oldFrame = currentFrame;
                    timer -= currentAnim.Delay;
                    AnimationFrame += Math.Sign(Rate);

                    if (AnimationFrame == currentAnim.Length)
                    {
                        //Looping
                        AnimationFrame = 0;
                        if (currentAnim.Loop)
                        {
                            currentFrame = currentAnim[0];

                            if (OnAnimate != null)
                                OnAnimate(this);
                            if (OnFrameChange != null && currentFrame != oldFrame)
                                OnFrameChange(this);
                        }
                        else
                        {
                            Finished = true;
                            Playing = false;
                        }

                        if (OnAnimationComplete != null)
                            OnAnimationComplete(this);
                    }
                    else if (AnimationFrame == -1)
                    {
                        //Reverse looping
                        AnimationFrame = currentAnim.Length - 1;
                        if (currentAnim.Loop)
                        {
                            currentFrame = currentAnim[0];

                            if (OnAnimate != null)
                                OnAnimate(this);
                            if (OnFrameChange != null && currentFrame != oldFrame)
                                OnFrameChange(this);
                        }
                        else
                        {
                            Finished = true;
                            Playing = false;
                        }

                        if (OnAnimationComplete != null)
                            OnAnimationComplete(this);
                    }
                    else
                    {
                        currentFrame = currentAnim[AnimationFrame];

                        if (OnAnimate != null)
                            OnAnimate(this);
                        if (OnFrameChange != null && currentFrame != oldFrame)
                            OnFrameChange(this);
                    }
                }
            }
        }

        public int TotalAnimations
        {
            get
            {
                return Animations.Count;
            }
        }

        public int CurrentAnimationFrames
        {
            get
            {
                if (Playing)
                    return currentAnim.Frames.Length;
                else
                    return 0;
            }
        }

        public int CurrentFrame
        {
            get
            {
                return currentFrame;
            }

            set
            {
                if (Playing)
                    Stop();
                if (value != currentFrame)
                {
                    currentFrame = value;
                    if (OnFrameChange != null)
                        OnFrameChange(this);
                }
            }
        }

        public bool Looping
        {
            get
            {
                return currentAnim.Loop;
            }
        }

        public Rectangle CurrentClip
        {
            get
            {
                return FrameRects[currentFrame];
            }
        }

        public int FramesTotal
        {
            get
            {
                return FramesX * FramesY;
            }
        }

        public override float Width
        {
            get
            {
                return FrameRects[0].Width;
            }
        }

        public override float Height
        {
            get
            {
                return FrameRects[0].Height;
            }
        }

        public override void Render()
        {
            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, CurrentClip, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public void Render(int overrideHeight)
        {
            Rectangle rect = CurrentClip;
            rect.Height = overrideHeight;
            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, rect, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public void DrawOutline(int overrideHeight, int offset = 1)
        {
            Vector2 pos = Position;
            Color was = Color;
            Color = Color.Black;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        Position = pos + new Vector2(i * offset, j * offset);
                        Render(overrideHeight);
                    }
                }
            }

            Position = pos;
            Color = was;
        }

        public new void SwapSubtexture(Subtexture setTo, Rectangle? clipRect = null)
        {
            Rectangle old = ClipRect;

            Texture = setTo.Texture;
            ClipRect = clipRect ?? setTo.Rect;

            for (int i = 0; i < FrameRects.Length; i++)
            {
                FrameRects[i].X += ClipRect.X - old.X;
                FrameRects[i].Y += ClipRect.Y - old.Y;
            }
        }

        public void CopyState(Spritesheet<T> other)
        {
            AnimationFrame = other.AnimationFrame;
            Playing = other.Playing;
            Finished = other.Finished;
            CurrentAnimID = other.CurrentAnimID;
            Rate = other.Rate;
            currentFrame = other.currentFrame;
            currentAnim = other.currentAnim;
            timer = other.timer;
        }

        public void SetTimerFrames(float frames)
        {
            timer = Engine.DeltaTime * frames;

            AnimationFrame = (int)(timer / currentAnim.Delay) % currentAnim.Frames.Length;
            currentFrame = currentAnim[AnimationFrame];

            timer %= currentAnim.Delay;
        }

        public void RandomizeFrame()
        {
            if (Playing)
            {
                AnimationFrame = Calc.Random.Next(currentAnim.Frames.Length);
                currentFrame = currentAnim[AnimationFrame];
            }
            else
                currentFrame = Calc.Random.Next(FrameRects.Length);
        }

        /*
         *  Animation definition
         */

        public void Add(T id, float delay, bool loop, params int[] frames)
        {
#if DEBUG
            foreach (var i in frames)
                if (i >= FrameRects.Length)
                    throw new Exception("Specified frames is out of max range for this Sprite.");
#endif

            var anim = new SpritesheetAnimation(delay, loop, frames);
            Animations.Add(id, anim);
        }

        public void Add(T id, float delay, params int[] frames)
        {
            Add(id, delay, true, frames);
        }

        public void Add(T id, int frame)
        {
            Add(id, 0, false, frame);
        }

        /*
         *  Playing animations
         */

        public void Play(T id, bool restart = false)
        {
            if (restart || (!Playing && !Finished) || !CurrentAnimID.Equals(id))
            {
                CurrentAnimID = id;
                currentAnim = Animations[id];

                AnimationFrame = 0;
                currentFrame = currentAnim[AnimationFrame];
                timer = 0;

                Finished = false;
                Playing = true;
            }
        }

        public void Play(T id, int startFrame, bool restart = false)
        {
            if (!Playing || !CurrentAnimID.Equals(id) || restart)
            {
                Play(id, true);
                AnimationFrame = startFrame;
                currentFrame = currentAnim[AnimationFrame];
            }
        }

        public void Stop()
        {
            AnimationFrame = 0;
            Finished = Playing = false;
        }

#if DEBUG
        public void LogAnimations()
        {
            foreach (var kv in Animations)
                Calc.Log(kv.Key.ToString() + ": " + kv.Value.ToString());
        }
#endif

        /*
         *  Animation struct
         */

        private struct SpritesheetAnimation
        {
            public float Delay;
            public int[] Frames;
            public bool Loop;

            public SpritesheetAnimation(float delay, bool loop, int[] frames)
            {
                Delay = delay;
                Loop = loop;
                Frames = frames;
            }

            public int this[int i]
            {
                get { return Frames[i]; }
            }

            public int Length
            {
                get { return Frames.Length; }
            }

            override public string ToString()
            {
                return "{ Delay: " + Delay + ", Loop: " + Loop + ", Frames: [" + string.Join(", ", Frames) + "] }";
            }
        }
    }
}
