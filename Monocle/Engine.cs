using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class Engine : Game
    {
        static public Engine Instance { get; private set; }
        static public GraphicsDeviceManager Graphics { get; private set; }
        static public Commands Commands { get; private set; }
        static public Pooler Pooler { get; private set; }
        static public int Width { get; private set; }
        static public int Height { get; private set; }
        static public int ViewWidth { get; private set; }
        static public int ViewHeight { get; private set; }
        static public float DeltaTime { get; private set; }
        static public float RawDeltaTime { get; private set; }
        static public float TimeRate = 1f;
        static public float FreezeTimer;
        static public Color ClearColor;
        static public bool ExitOnEscapeKeypress;
        static public RenderTarget2D RenderBuffer;

        private Scene scene;
        private Scene nextScene;
        private string windowTitle;
        private Viewport viewport;
        internal Matrix screenMatrix;
#if DEBUG
        private TimeSpan counterElapsed = TimeSpan.Zero;
        private int counterFrames = 0;
#endif

        public Engine(int width, int height, int windowedScale, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Width = width;
            Height = height;
            Window.Title = this.windowTitle = windowTitle;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            Graphics.SynchronizeWithVerticalRetrace = true;
			Graphics.GraphicsProfile = GraphicsProfile.HiDef;

            if (fullscreen)
                Graphics.IsFullScreen = true;
            else
            {
                Graphics.PreferredBackBufferWidth = Width * windowedScale;
                Graphics.PreferredBackBufferHeight = Height * windowedScale;
                Graphics.IsFullScreen = false;
            }

            Content.RootDirectory = @"Content\";

            IsMouseVisible = false;
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;
        }

        protected virtual void OnGraphicsReset(object sender, EventArgs e)
        {
            UpdateView();

            if (scene != null)
                scene.HandleGraphicsReset();
            if (nextScene != null && nextScene != scene)
                nextScene.HandleGraphicsReset();
        }

        protected virtual void OnGraphicsCreate(object sender, EventArgs e)
        {
            UpdateView();

            if (scene != null)
                scene.HandleGraphicsCreate();
            if (nextScene != null && nextScene != scene)
                nextScene.HandleGraphicsCreate();
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);

            if (scene != null)
                scene.GainFocus();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            if (scene != null)
                scene.LoseFocus();
        }

        protected override void Initialize()
        {
            base.Initialize();

            MInput.Initialize();
            Tracker.Initialize();
            Pooler = new Monocle.Pooler();
            Commands = new Commands();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Monocle.Draw.Initialize(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            RawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = RawDeltaTime * TimeRate;

            //Update input
            MInput.Update();

            if (ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
                return;
            }

            //Update current scene
            if (FreezeTimer > 0)
                FreezeTimer = Math.Max(FreezeTimer - DeltaTime, 0);
            else if (scene != null)
            {
                scene.BeforeUpdate();
                scene.Update();
                scene.AfterUpdate();
            }

            //Debug Console
            if (Commands.Open)
                Commands.UpdateOpen();
            else if (Commands.Enabled)
                Commands.UpdateClosed();

            //Changing scenes
            if (scene != nextScene)
            {
                if (scene != null)
                    scene.End();
                scene = nextScene;
                OnSceneTransition();
                if (scene != null)
                    scene.Begin();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            RenderCore();

            base.Draw(gameTime);
            if (Commands.Open)
                Commands.Render();
#if DEBUG
            //Frame counter
            counterFrames++;
            counterElapsed += gameTime.ElapsedGameTime;
            if (counterElapsed > TimeSpan.FromSeconds(1))
            {
                Window.Title = windowTitle + " " + counterFrames.ToString() + " fps - " + (GC.GetTotalMemory(true) / 1048576f).ToString("F") + " MB";
                counterFrames = 0;
                counterElapsed -= TimeSpan.FromSeconds(1);
            }
#endif
        }

        /// <summary>
        /// Override if you want to change the core rendering functionality of Monocle Engine.
        /// By default, this simply sets the render target to null, clears the screen, and renders the current Scene
        /// </summary>
        protected virtual void RenderCore()
        {
            if (scene != null)
                scene.BeforeRender();

            GraphicsDevice.SetRenderTarget(RenderBuffer);
            GraphicsDevice.Clear(ClearColor);

            if (scene != null)
            {
                scene.Render();
                scene.AfterRender();
            }

            //Draw the buffer scaled up to the screen
            if (RenderBuffer != null)
            {           
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Viewport = viewport;
                GraphicsDevice.Clear(ClearColor);

                Monocle.Draw.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, screenMatrix);
                Monocle.Draw.SpriteBatch.Draw(RenderBuffer, Vector2.Zero, Color.White);
                Monocle.Draw.SpriteBatch.End();
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            MInput.Shutdown();
        }

        #region Scene

        /// <summary>
        /// Called after a Scene ends, before the next Scene begins
        /// </summary>
        protected virtual void OnSceneTransition()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// The currently active Scene. Note that if set, the Scene will not actually change until the end of the Update
        /// </summary>
        static public Scene Scene
        {
            get { return Instance.scene; }
            set { Instance.nextScene = value; }
        }

        #endregion

        #region Screen

        static public Matrix ScreenMatrix
        {
            get
            {
                if (RenderBuffer == null)
                    return Instance.screenMatrix;
                else
                    return Matrix.Identity;
            }
        }

        static public void SetWindowed(int scale = 1)
        {
            Graphics.PreferredBackBufferWidth = Width * scale;
            Graphics.PreferredBackBufferHeight = Height * scale;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        static public void SetFullscreen()
        {
            Graphics.PreferredBackBufferWidth = Instance.GraphicsDevice.DisplayMode.Width;
            Graphics.PreferredBackBufferHeight = Instance.GraphicsDevice.DisplayMode.Height;
            Graphics.IsFullScreen = true;         
            Graphics.ApplyChanges();
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (screenWidth / Width > screenHeight / Height)
            {
                ViewWidth = (int)(screenHeight / Height * Width);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)(screenWidth / Width * Height);
            }

            screenMatrix = Matrix.CreateScale(ViewWidth / (float)Width);

            viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - ViewWidth / 2),
                Y = (int)(screenHeight / 2 - ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            //Debug Log
            //Calc.Log("Update View - " + screenWidth + "x" + screenHeight + " - " + viewport.Width + "x" + viewport.Height + " - " + viewport.X + "," + viewport.Y);
        }

        #endregion
    }
}
