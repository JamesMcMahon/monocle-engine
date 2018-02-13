using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace Monocle
{
    public class Engine : Game
    {

        public string Title;
        public Version Version;

        // references
        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Commands Commands { get; private set; }
        public static Pooler Pooler { get; private set; }
        public static Action OverloadGameLoop;

        // screen size
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int ViewWidth { get; private set; }
        public static int ViewHeight { get; private set; }
        public static int ViewPadding
        {
            get { return viewPadding; }
            set
            {
                viewPadding = value;
                Instance.UpdateView();
            }
        }
        private static int viewPadding = 0;
        private static bool resizing;

        // time
        public static float DeltaTime { get; private set; }
        public static float RawDeltaTime { get; private set; }
        public static float TimeRate = 1f;
        public static float FreezeTimer;
        public static int FPS;
        private TimeSpan counterElapsed = TimeSpan.Zero;
        private int fpsCounter = 0;

        // content directory
#if !CONSOLE
        private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        public static string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Instance.Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Instance.Content.RootDirectory); }
#elif XBOXONE
            get { return Instance.Content.RootDirectory; }
#else
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
#endif
        }

        // util
        public static Color ClearColor;
        public static bool ExitOnEscapeKeypress;

        // scene
        private Scene scene;
        private Scene nextScene;
        
        public Engine(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Title = Window.Title = windowTitle;
            Width = width;
            Height = height;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferMultiSampling = false;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.ApplyChanges();

#if PS4 || XBOXONE
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
#else
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (fullscreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
                Graphics.IsFullScreen = false;
            }
#endif

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing)
            {
                resizing = true;

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();

                resizing = false;
            }
        }
#endif

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

#if !CONSOLE
            if (ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
                return;
            }
#endif

            if (OverloadGameLoop != null)
            {
                OverloadGameLoop();
                base.Update(gameTime);
                return;
            }

            //Update current scene
            if (FreezeTimer > 0)
                FreezeTimer = Math.Max(FreezeTimer - RawDeltaTime, 0);
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
                var lastScene = scene;
                if (scene != null)
                    scene.End();
                scene = nextScene;
                OnSceneTransition(lastScene, nextScene);
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

            //Frame counter
            fpsCounter++;
            counterElapsed += gameTime.ElapsedGameTime;
            if (counterElapsed >= TimeSpan.FromSeconds(1))
            {
#if DEBUG
                Window.Title = Title + " " + fpsCounter.ToString() + " fps - " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";
#endif
                FPS = fpsCounter;
                fpsCounter = 0;
                counterElapsed -= TimeSpan.FromSeconds(1);
            }
        }

        /// <summary>
        /// Override if you want to change the core rendering functionality of Monocle Engine.
        /// By default, this simply sets the render target to null, clears the screen, and renders the current Scene
        /// </summary>
        protected virtual void RenderCore()
        {
            if (scene != null)
                scene.BeforeRender();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = Viewport;
            GraphicsDevice.Clear(ClearColor);

            if (scene != null)
            {
                scene.Render();
                scene.AfterRender();
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            MInput.Shutdown();
        }

        public void RunWithLogging()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                ErrorLog.Write(e);
                ErrorLog.Open();
            }
        }

        #region Scene

        /// <summary>
        /// Called after a Scene ends, before the next Scene begins
        /// </summary>
        protected virtual void OnSceneTransition(Scene from, Scene to)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            TimeRate = 1f;
        }

        /// <summary>
        /// The currently active Scene. Note that if set, the Scene will not actually change until the end of the Update
        /// </summary>
        public static Scene Scene
        {
            get { return Instance.scene; }
            set { Instance.nextScene = value; }
        }

        #endregion

        #region Screen

        public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix;

        public static void SetWindowed(int width, int height)
        {
#if !CONSOLE
            if (width > 0 && height > 0)
            {
                resizing = true;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
                resizing = false;
            }
#endif
        }

        public static void SetFullscreen()
        {
#if !CONSOLE
            resizing = true;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;         
            Graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            resizing = false;
#endif
        }
        
        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // get View Size
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

            // apply View Padding
            var aspect = ViewHeight / (float)ViewWidth;
            ViewWidth -= ViewPadding * 2;
            ViewHeight -= (int)(aspect * ViewPadding * 2);

            // update screen matrix
            ScreenMatrix = Matrix.CreateScale(ViewWidth / (float)Width);

            // update viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - ViewWidth / 2),
                Y = (int)(screenHeight / 2 - ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            //Debug Log
            //Calc.Log("Update View - " + screenWidth + "x" + screenHeight + " - " + viewport.Width + "x" + viewport.GuiHeight + " - " + viewport.X + "," + viewport.Y);
        }

        #endregion
    }
}
