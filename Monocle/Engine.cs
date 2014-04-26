using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Monocle
{
    public class Engine : Game
    {
        public const int MAX_TAG = 30;

        static public Engine Instance { get; private set; }
        static public float DeltaTime { get; private set; }
        static public float TimeRate = 1f;
        static public bool ConsoleEnabled;

        public GraphicsDeviceManager Graphics { get; private set; }
        public Screen Screen { get; private set; }
        public Commands Commands { get; private set; }

        private Scene scene;
        private Scene nextScene;
        private string windowTitle;
        
#if DEBUG
        private TimeSpan counterElapsed = TimeSpan.Zero;
        private int counterFrames = 0;
#endif

        public Engine(int width, int height, float scale, string windowTitle)
        {
            this.windowTitle = windowTitle;

            Instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = false;
            Screen = new Screen(this, width, height, scale);

            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            MInput.Initialize();
            Screen.Initialize();           
            Graphics.DeviceReset += OnGraphicsReset;
            Window.Title = windowTitle;
            Commands = new Commands();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Content.RootDirectory = @"Content\";
            Monocle.Draw.Initialize(GraphicsDevice);
        }

        private void OnGraphicsReset(object sender, EventArgs e)
        {
            Screen.Initialize();

            if (scene != null)
                scene.HandleGraphicsReset();
            if (nextScene != null)
                nextScene.HandleGraphicsReset();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * TimeRate;

            MInput.Update();
            if (scene != null)
                scene.Update();

            //Debug Console
            if (ConsoleEnabled)
            {
                if (Commands.Open)
                    Commands.UpdateOpen();
                else
                    Commands.UpdateClosed();
            }

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
            if (Screen.RenderTarget.IsDisposed)
                Screen.Initialize();

            GraphicsDevice.SetRenderTarget(Screen.RenderTarget);
            GraphicsDevice.Clear(Screen.ClearColor);

            if (scene != null)
                scene.Render();

            if (scene != null)
                scene.PostRender();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            Screen.Render();

            if (scene != null)
                scene.PostScreen();

            base.Draw(gameTime);
            if (ConsoleEnabled && Commands.Open)
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

        public Scene Scene
        {
            get { return scene; }
            set { nextScene = value; }
        }

        protected virtual void OnSceneTransition()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
