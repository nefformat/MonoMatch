using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMatch.Enums;
using MonoMatch.Screens;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoMatch
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MyGame : Game
    {
        GraphicsDeviceManager graphics;
        IScreen screen;
        SpriteBatch spriteBatch;
        MouseState lastMouseState;
        SpriteFont font;
        GameStatus lastGameStatus;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 830; // GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = 730; // GraphicsDevice.DisplayMode.Height;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("myFont");
            TextureLoad();

            screen = new StartScreen(spriteBatch, font);


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState currentMouseState = Mouse.GetState();

            if (lastMouseState.LeftButton == ButtonState.Released &&
                currentMouseState.LeftButton == ButtonState.Pressed &&
                currentMouseState.X >= 0 &&
                graphics.PreferredBackBufferWidth > currentMouseState.X &&
                currentMouseState.Y >= 0 &&
                graphics.PreferredBackBufferHeight > currentMouseState.Y)
            {
                screen.Click(currentMouseState.X, currentMouseState.Y);
            }

            GameStatus currentGameStatus = screen.Update();

            if (lastGameStatus != currentGameStatus)
                switch (currentGameStatus)
                {
                    case GameStatus.StartScreen:
                        screen = new StartScreen(spriteBatch, font);
                        break;
                    case GameStatus.GameScreen:
                        screen = new GameScreen(spriteBatch, font);
                        break;
                    case GameStatus.GameOverScreen:
                        screen = new GameOverScreen(spriteBatch, font);
                        break;
                }
            lastGameStatus = currentGameStatus;

            lastMouseState = currentMouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            screen.Draw();

            base.Draw(gameTime);
        }


        private void TextureLoad()
        {
            Textures.Images.Add("startscreen", Content.Load<Texture2D>("startscreen"));
            Textures.Images.Add("gameoverscreen", Content.Load<Texture2D>("gameoverscreen"));
            Textures.Images.Add("ribbon", Content.Load<Texture2D>("ribbon"));

            Textures.Images.Add("leftdestroyer", Content.Load<Texture2D>("leftdestroyer"));
            Textures.Images.Add("rightdestroyer", Content.Load<Texture2D>("rightdestroyer"));
            Textures.Images.Add("updestroyer", Content.Load<Texture2D>("updestroyer"));
            Textures.Images.Add("downdestroyer", Content.Load<Texture2D>("downdestroyer"));

            Textures.Cells.Add((CellType.Cell1, BonusType.None), Content.Load<Texture2D>("cell1"));
            Textures.Cells.Add((CellType.Cell2, BonusType.None), Content.Load<Texture2D>("cell2"));
            Textures.Cells.Add((CellType.Cell3, BonusType.None), Content.Load<Texture2D>("cell3"));
            Textures.Cells.Add((CellType.Cell4, BonusType.None), Content.Load<Texture2D>("cell4"));
            Textures.Cells.Add((CellType.Cell5, BonusType.None), Content.Load<Texture2D>("cell5"));

            Textures.Cells.Add((CellType.Cell1, BonusType.Bomb), Content.Load<Texture2D>("cell1bomb"));
            Textures.Cells.Add((CellType.Cell2, BonusType.Bomb), Content.Load<Texture2D>("cell2bomb"));
            Textures.Cells.Add((CellType.Cell3, BonusType.Bomb), Content.Load<Texture2D>("cell3bomb"));
            Textures.Cells.Add((CellType.Cell4, BonusType.Bomb), Content.Load<Texture2D>("cell4bomb"));
            Textures.Cells.Add((CellType.Cell5, BonusType.Bomb), Content.Load<Texture2D>("cell5bomb"));

            Textures.Cells.Add((CellType.Cell1, BonusType.HorDestroyer), Content.Load<Texture2D>("cell1hdestroyer"));
            Textures.Cells.Add((CellType.Cell2, BonusType.HorDestroyer), Content.Load<Texture2D>("cell2hdestroyer"));
            Textures.Cells.Add((CellType.Cell3, BonusType.HorDestroyer), Content.Load<Texture2D>("cell3hdestroyer"));
            Textures.Cells.Add((CellType.Cell4, BonusType.HorDestroyer), Content.Load<Texture2D>("cell4hdestroyer"));
            Textures.Cells.Add((CellType.Cell5, BonusType.HorDestroyer), Content.Load<Texture2D>("cell5hdestroyer"));


            Textures.Cells.Add((CellType.Cell1, BonusType.VerDestroyer), Content.Load<Texture2D>("cell1vdestroyer"));
            Textures.Cells.Add((CellType.Cell2, BonusType.VerDestroyer), Content.Load<Texture2D>("cell2vdestroyer"));
            Textures.Cells.Add((CellType.Cell3, BonusType.VerDestroyer), Content.Load<Texture2D>("cell3vdestroyer"));
            Textures.Cells.Add((CellType.Cell4, BonusType.VerDestroyer), Content.Load<Texture2D>("cell4vdestroyer"));
            Textures.Cells.Add((CellType.Cell5, BonusType.VerDestroyer), Content.Load<Texture2D>("cell5vdestroyer"));
        }
    }
}
