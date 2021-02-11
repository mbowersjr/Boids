using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Boids.Core.Entities;

namespace Boids.Core
{
    public class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        public static GridRenderer Grid { get; private set; }

        Flock _flock;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.SynchronizeWithVerticalRetrace = false;

            this.IsMouseVisible = true;

            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            Grid = new GridRenderer(GraphicsDevice, 32, 24);
        }

        protected override void Initialize()
        {
            base.Initialize();

            Grid.Initialize();

            _flock = new Flock();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Boid.BoidSprite = Content.Load<Texture2D>("Images/Boid");
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.Q))
            {
                Exit();
            }

            base.Update(gameTime);

            _flock.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            Grid.Draw();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _flock.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
