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
        static public float DeltaTime { get; private set; }
        static public float TimeRate = 1f;
        static public Color ClearColor;
        static public bool ExitOnEscapeKeypress;

        private Scene scene;
        private Scene nextScene;
        private string windowTitle;
#if DEBUG
        private TimeSpan counterElapsed = TimeSpan.Zero;
        private int counterFrames = 0;
#endif

        public Engine(int width, int height, string windowTitle)
        {
            Instance = this;

            Width = width;
            Height = height;
            Window.Title = this.windowTitle = windowTitle;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.PreferredBackBufferWidth = Width;
            Graphics.PreferredBackBufferHeight = Height;
#if DEBUG
            Graphics.IsFullScreen = false;
            Graphics.SynchronizeWithVerticalRetrace = false;
#else
            Graphics.IsFullScreen = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
#endif

            Content.RootDirectory = @"Content\";

            IsMouseVisible = false;
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;
        }

        private void OnGraphicsReset(object sender, EventArgs e)
        {
            UpdateView();

            if (scene != null)
                scene.HandleGraphicsReset();
            if (nextScene != null)
                nextScene.HandleGraphicsReset();
        }

        protected virtual void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            int drawWidth;
            int drawHeight;

            if (screenWidth / Width > screenHeight / Height)
            {
                drawWidth = (int)(screenHeight / Height * Width);
                drawHeight = (int)screenHeight;
            }
            else
            {
                drawWidth = (int)screenWidth;
                drawHeight = (int)(screenWidth / Width * Height);
            }

            Monocle.Draw.MasterRenderMatrix = Matrix.CreateScale(drawWidth / (float)Width);

            GraphicsDevice.Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - drawWidth / 2),
                Y = (int)(screenHeight / 2 - drawHeight / 2),
                Width = drawWidth,
                Height = drawHeight,
                MinDepth = 0,
                MaxDepth = 1
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            UpdateView();
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
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * TimeRate;

            //Update input
            MInput.Update();

            if (ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
                return;
            }

            //Update current scene
            if (scene != null)
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
            if (scene != null)
                scene.BeforeRender();

            RenderCore();

            if (scene != null)
                scene.AfterRender();

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
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearColor);

            if (scene != null)
                scene.Render();
        }

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
    }
}
