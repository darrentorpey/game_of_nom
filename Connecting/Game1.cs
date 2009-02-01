using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Connecting
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameOfNom : Microsoft.Xna.Framework.Game
    {
        public static int GAME_HEIGHT = 600;
        public static int GAME_WIDTH = 900;
        Rectangle GameBoundaries = new Rectangle(0, 0, GAME_WIDTH, GAME_HEIGHT);

        private static int MAX_NOMS_DEAD = 5;

        private static GameObjectManager s_GameManPause = new GameObjectManager();
        private static GameObjectManager s_GameManStart = new GameObjectManager();
        private static GameObjectManager s_GameManGameOver = new GameObjectManager();
        private static GameObjectManager s_GameManVictory = new GameObjectManager();

        private float _fGameMinutesElapsed = 0.0f;

        Texture2D menuBarTexture;

        public enum GameState
        {
            Running = 0,
            Start = 1,
            Paused = 2,
            Help = 3,
            Victory = 4,
            GameOver = 5
        }

        public GameState CurrentGameState { get; set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObject inTransitByUser;

        int _iFruitSpawnRate = 3000;
        int _iPersonSpawnRate = 12000;
        int _iTimeToFruit;
        int _iTimeToPerson;

        SpriteFont helveticaSmall;
        SpriteFont helveticaSmallItalic;
        SpriteFont helveticaMedium;
        SpriteFont helveticaLarge;
        SpriteFont helveticaHuge;

        MouseState lastMouseState;
        KeyboardState lastKeyState;

        bool _bSingleStep = false;
        bool _bPrintDebugInfo = true;

        public GameOfNom()
        {
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GameBoundaries.Width;
            graphics.PreferredBackBufferHeight = GameBoundaries.Height + 100;
            Content.RootDirectory = "Content";

            CurrentGameState = GameState.Start;

            _iTimeToFruit = _iFruitSpawnRate;
            _iTimeToPerson = _iPersonSpawnRate;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            Person.LoadContent(Content);
            FoodSource.LoadContent(Content);
            Angel.LoadContent(Content);
            Tombstone.LoadContent(Content);
            DrawUtils.LoadContent(Content);
            SoundState.LoadContent(Content);
            GameLogo.LoadContent(Content);
            GameLogoFull.LoadContent(Content);
            GameOverSplash.LoadContent(Content);
            VictorySplash.LoadContent(Content);

            helveticaSmall = Content.Load<SpriteFont>("fonts/HelveticaSmall");
            helveticaSmallItalic = Content.Load<SpriteFont>("fonts/HelveticaSmallItalic");
            helveticaMedium = Content.Load<SpriteFont>("fonts/HelveticaMedium");
            helveticaLarge = Content.Load<SpriteFont>("fonts/HelveticaLarge");
            helveticaHuge = Content.Load<SpriteFont>("fonts/HelveticaHuge");

            menuBarTexture = Content.Load<Texture2D>("menu_bar");

            spawnStartingObjects();

            loadNavBar();

            loadPauseScreen();
            loadStartScreen();
            loadGameOverScreen();
            loadVictoryScreen();
        }

        private void loadPauseScreen()
        {
            s_GameManPause.AddObject(new GameLogoFull(new Vector2(GameBoundaries.Width / 2, 20.0f)));
        }

        private void loadStartScreen()
        {
            s_GameManStart.AddObject(new GameLogoFull(new Vector2(GameBoundaries.Width / 2, 20.0f)));
        }

        private void loadGameOverScreen()
        {
            //s_GameManGameOver.AddObject(new GameLogoFull(new Vector2(GameBoundaries.Width / 2, 20.0f)));
            s_GameManGameOver.AddObject(new GameOverSplash(new Vector2(GameBoundaries.Width / 2, 20.0f)));
        }

        private void loadVictoryScreen()
        {
            s_GameManVictory.AddObject(new VictorySplash(new Vector2(GameBoundaries.Width / 2, 20.0f)));
        }

        private void loadNavBar() {
            GameObjectManager.Instance.AddObject(new Angel(new Vector2(100.0f, 650.0f), true));
            GameObjectManager.Instance.AddObject(new GameLogo(new Vector2(790.0f, 595.0f)));
        }

        private void spawnStartingObjects()
        {
            // Add lots of people around randomly
            //_Flock = new PersonFlock();
            //_Flock.Location = new Vector2(RandomInstance.Instance.Next(0, GameBoundaries.Width),
            //    RandomInstance.Instance.Next(0, GameBoundaries.Height));
            Rectangle bounds = new Rectangle(13, 13, GameBoundaries.Width, GameBoundaries.Height - 26);
            //for (int i = 0; i < 10; ++i)
            //{
            //    Person newPerson = new Person(_Flock.Location, bounds);
            //    _Flock.AddPerson(newPerson);
            //}

            //// Init people here so that we know the content (textures, etc.) are loaded
            //GameObjectManager.Instance.AddObject(_Flock);

            for (int i = 0; i < 20; ++i) {
                GameObjectManager.Instance.AddObject(new Person(getRandomLocation(50, bounds), bounds));
            }

            for (int i = 0; i < 6; ++i)
            {
                spawnFruit(FoodSource.Fruit.Grapes, true);
            }

            for (int i = 0; i < 6; ++i)
            {
                spawnFruit(FoodSource.Fruit.Orange, true);
            }

            for (int i = 0; i < 6; ++i)
            {
                spawnFruit(FoodSource.Fruit.Banana, true);
            }

            //GameObjectManager.Instance.AddObject(new Angel(getRandomLocation(50)));
        }

        private Vector2 getRandomLocation(int borderPadding, Rectangle aBounds)
        {
            return new Vector2(
                RandomInstance.Instance.Next(aBounds.Left + borderPadding, aBounds.Right - borderPadding),
                RandomInstance.Instance.Next(aBounds.Top + borderPadding, aBounds.Bottom - borderPadding)
            );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            if (_fGameMinutesElapsed >= 3.0f)
            //if (_fGameMinutesElapsed >= 0.03f)
            {
                // A winner is you!
                CurrentGameState = GameState.Victory;
            }
            else if (GameObjectManager.Instance.CountDead >= MAX_NOMS_DEAD)
            {
                // Game Over
                CurrentGameState = GameState.GameOver;
            }

            if (CurrentGameState == GameState.Running)
            {
                _fGameMinutesElapsed += (float)gameTime.ElapsedGameTime.TotalMinutes;

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                    || ((Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Q) || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.X)) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.LeftControl)))
                    this.Exit();

                processMouseEvents(gameTime);

                processKeyboardEvents(gameTime);

                spawnMoreFood(gameTime);
                //spawnMorePeople(gameTime);

                // TODO: Add your update logic here
                base.Update(gameTime);
                SoundState.Instance.ClearFinishedSounds(gameTime);
            }
            else
            {
                KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
                if (lastKeyState == null)
                    lastKeyState = keyState;
                Keys[] keysPressed = keyState.GetPressedKeys();

                MouseState mouseState = Mouse.GetState();
                Vector2 mouseLoc = new Vector2(mouseState.X, mouseState.Y);
                if (lastMouseState == null)
                    lastMouseState = mouseState;
                switch (CurrentGameState)
                {
                    case GameState.Paused:

                        if ((keyState.IsKeyDown(Keys.P) && !lastKeyState.IsKeyDown(Keys.P)) || (keyState.IsKeyDown(Keys.Escape) && !lastKeyState.IsKeyDown(Keys.Escape)))
                        {
                            Unpause();
                        }

                        s_GameManPause.Update(gameTime);

                        base.Update(gameTime);
                        SoundState.Instance.ClearFinishedSounds(gameTime);

                        break;
                    case GameState.Start:

                        // Keyboard
                        //if ((keyState.IsKeyDown(Keys.Space) && !lastKeyState.IsKeyDown(Keys.Space)) || (keyState.IsKeyDown(Keys.Enter) && !lastKeyState.IsKeyDown(Keys.Enter)))
                        //{
                        //    CurrentGameState = GameState.Running;
                        //}
                        if (keysPressed.Count() > 0)
                        {
                            CurrentGameState = GameState.Running;
                        }

                        // Mouse 
                        if (mouseState.LeftButton == ButtonState.Pressed && (lastMouseState.LeftButton != ButtonState.Pressed))
                        {
                            CurrentGameState = GameState.Running;
                        }

                        s_GameManStart.Update(gameTime);

                        base.Update(gameTime);
                        SoundState.Instance.ClearFinishedSounds(gameTime);

                        break;
                    case GameState.GameOver:

                        // Keyboard
                        //if ((keyState.IsKeyDown(Keys.Space) && !lastKeyState.IsKeyDown(Keys.Space)) || (keyState.IsKeyDown(Keys.Enter) && !lastKeyState.IsKeyDown(Keys.Enter)))
                        //{
                        //    // Restart the game
                        //    Restart();
                        //}
                        if (keysPressed.Count() > 0)
                        {
                            // Restart the game
                            Restart();
                        }

                        // Mouse 
                        if (mouseState.LeftButton == ButtonState.Pressed && (lastMouseState.LeftButton != ButtonState.Pressed))
                        {
                            Restart();
                        }

                        s_GameManGameOver.Update(gameTime);

                        base.Update(gameTime);
                        SoundState.Instance.ClearFinishedSounds(gameTime);

                        break;
                    case GameState.Victory:
                        if (keysPressed.Count() > 0)
                        {
                            // Restart the game
                            Restart();
                        }

                        // Mouse 
                        if (mouseState.LeftButton == ButtonState.Pressed && (lastMouseState.LeftButton != ButtonState.Pressed))
                        {
                            Restart();
                        }

                        s_GameManVictory.Update(gameTime);

                        base.Update(gameTime);
                        SoundState.Instance.ClearFinishedSounds(gameTime);

                        break;
                }
                lastKeyState = keyState;
                lastMouseState = mouseState;
            }
        }

        private void spawnMorePeople(GameTime aTime)
        {
            if (_iTimeToPerson <= 0)
            {
                spawnPerson(true);
                _iTimeToPerson = _iPersonSpawnRate;
            }
            else
                _iTimeToPerson -= aTime.ElapsedGameTime.Milliseconds;
        }

        private void spawnMoreFood(GameTime aTime)
        {
            if (_iTimeToFruit <= 0)
            {
                spawnRandomFruit();
                _iTimeToFruit = _iFruitSpawnRate;
            }
            else
                _iTimeToFruit -= aTime.ElapsedGameTime.Milliseconds;
        }

        private void spawnRandomFruit()
        {
            spawnFruit((FoodSource.Fruit)(RandomInstance.Instance.Next(0, (int)FoodSource.Fruit.Count)));
        }

        private void spawnFruit(FoodSource.Fruit fruitType)
        {
            GameObjectManager.Instance.AddObject(new FoodSource(getRandomLocation(50, GameBoundaries), fruitType));
        }

        private void spawnFruit(FoodSource.Fruit fruitType, bool skipStartAnimation)
        {
            FoodSource foodSource = new FoodSource(getRandomLocation(50, GameBoundaries), fruitType);
            foodSource.NoStartAnimation = skipStartAnimation;
            GameObjectManager.Instance.AddObject(foodSource);
        }

        private void spawnPerson(bool abOffScreen)
        {
            Rectangle personBounds = new Rectangle(13, 13, GameBoundaries.Width, GameBoundaries.Height - 26);

            if (abOffScreen)
            {
                int iside = RandomInstance.Instance.Next(0, 5);
                Rectangle spawnBounds = Rectangle.Empty;
                switch (iside)
                {
                    case 0: // Left
                        spawnBounds = new Rectangle(-100, 0, 100, GameBoundaries.Height);
                        break;
                    case 1: // Right
                        spawnBounds = new Rectangle(GameBoundaries.Width, 0, 100, GameBoundaries.Height);
                        break;
                    case 2: // Top
                        spawnBounds = new Rectangle(0, -100, GameBoundaries.Width, 100);
                        break;
                    case 3: //Bottom
                        spawnBounds = new Rectangle(0, GameBoundaries.Height, GameBoundaries.Width, 100);
                        break;
                }
                GameObjectManager.Instance.AddObject(new Person(getRandomLocation(0, spawnBounds), personBounds));
            }
            else
            {
                GameObjectManager.Instance.AddObject(new Person(getRandomLocation(50, personBounds), personBounds));
            }
        }

        private void processKeyboardEvents(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            if (lastKeyState == null)
                lastKeyState = keyState;

            if ((keyState.IsKeyDown(Keys.P) && !lastKeyState.IsKeyDown(Keys.P)) || (keyState.IsKeyDown(Keys.Escape) && !lastKeyState.IsKeyDown(Keys.Escape)))
            {
                Pause();
            }

            if (keyState.IsKeyDown(Keys.R) && !lastKeyState.IsKeyDown(Keys.R))
            {
                Restart();
            }
            if (keyState.IsKeyDown(Keys.S) && !lastKeyState.IsKeyDown(Keys.S))
            {
                if (keyState.IsKeyDown(Keys.D1))
                {
                    spawnFruit(FoodSource.Fruit.Grapes);
                }
                else if (keyState.IsKeyDown(Keys.D2))
                {
                    spawnFruit(FoodSource.Fruit.Orange);
                }
                else if (keyState.IsKeyDown(Keys.D3))
                {
                    spawnFruit(FoodSource.Fruit.Banana);
                }
                else
                {
                    spawnRandomFruit();
                }
            }
            if (keyState.IsKeyDown(Keys.Back) && !lastKeyState.IsKeyDown(Keys.Back))
                SoundState.Instance.ToggleSound();
            if (keyState.IsKeyDown(Keys.Tab) && !lastKeyState.IsKeyDown(Keys.Tab))
                _bSingleStep = !_bSingleStep;

            if (_bSingleStep)
            {
                if (keyState.IsKeyDown(Keys.OemPlus) && !lastKeyState.IsKeyDown(Keys.OemPlus))
                    GameObjectManager.Instance.Update(gameTime);
            }
            else
                GameObjectManager.Instance.Update(gameTime);

            lastKeyState = keyState;
        }

        private void Pause()
        {
            SoundState.Instance.SoundPause();
            CurrentGameState = GameState.Paused;
        }

        private void Unpause()
        {
            SoundState.Instance.SoundResume();
            CurrentGameState = GameState.Running;
        }

        private void processMouseEvents(GameTime aTime)
        {
            MouseState state = Mouse.GetState();

            if (lastMouseState == null)
                lastMouseState = state;

            Vector2 mouseLoc = new Vector2(state.X, state.Y);

            if (state.LeftButton == ButtonState.Pressed)
            {
                if (this.inTransitByUser == null)
                {
                    GameObjectManager manager = GameObjectManager.Instance;
                    for (int i = 0; i < manager.Count; ++i)
                    {
                        if(manager[i].RadiusCheck(ref mouseLoc, 0.0f))
                        {
                            if (manager[i].CanBeHeld)
                            {
                                this.inTransitByUser = manager[i];
                                manager[i].Hold();
                                SoundState.Instance.PlayPickupSound(aTime);
                            }
                            else if(_bPrintDebugInfo)
                            {
                                Console.WriteLine(manager[i].GetDebugInfo());
                            }
                        }
                    }
                }
            }

            if (state.RightButton == ButtonState.Pressed && 
                lastMouseState.RightButton != ButtonState.Pressed)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {

                    GameObjectManager manager = GameObjectManager.Instance;
                    for (int i = 0; i < manager.Count; ++i)
                    {
                        if (manager[i].RadiusCheck(ref mouseLoc, 0.0f))
                        {
                            if (manager[i] is Person)
                            {
                                manager[i]._Hunger += 100;
                            }
                        }
                    }
                }
            }

            if (this.inTransitByUser != null)
            {
                int x = (int)Math.Max(this.inTransitByUser.Radius, Math.Min(mouseLoc.X, GameBoundaries.Width - this.inTransitByUser.Radius));
                int y = (int)Math.Max(this.inTransitByUser.Radius, Math.Min(mouseLoc.Y, GameBoundaries.Height - this.inTransitByUser.Radius));
                Vector2 loc = new Vector2(x, y);
                this.inTransitByUser.MoveTo(ref loc);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (this.inTransitByUser != null)
                {
                    inTransitByUser.Drop();
                    this.inTransitByUser = null;
                }
            }

            lastMouseState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (CurrentGameState == GameState.Running)
            {
                GraphicsDevice.Clear(Color.LightCyan);

                spriteBatch.Begin();

                drawNavBar();

                GameObjectManager.Instance.Draw(spriteBatch, gameTime);

                spriteBatch.End();

                base.Draw(gameTime);
            }
            else
            {
                spriteBatch.Begin();
                //GraphicsDevice.Clear(Color.LightGray);
                //GraphicsDevice.Clear(Color.LightSkyBlue);
                Vector3 color = Color.LightCyan.ToVector3();
                GraphicsDevice.Clear(new Color(color.X - 0.05f, color.Y - 0.05f, color.Z - 0.05f));

                switch (CurrentGameState)
                {
                    case GameState.Paused:
                        s_GameManPause.Draw(spriteBatch, gameTime);
                        drawPauseScreen(spriteBatch);
                        break;
                    case GameState.Start: // Right
                        s_GameManStart.Draw(spriteBatch, gameTime);
                        drawStartScreen(spriteBatch);
                        break;
                    case GameState.GameOver: // Top
                        s_GameManGameOver.Draw(spriteBatch, gameTime);
                        drawGameOverScreen(spriteBatch);
                        break;
                    case GameState.Victory: //Bottom
                        s_GameManVictory.Draw(spriteBatch, gameTime);
                        drawVictoryScreen(spriteBatch);
                        break;
                }
                    

                spriteBatch.End();
                base.Draw(gameTime);
            }
        }

        private void drawPauseScreen(SpriteBatch spriteBatch)
        {
            float x = helveticaHuge.MeasureString("Paused").X / 2;
            spriteBatch.DrawString(helveticaHuge, "Paused", new Vector2(GameBoundaries.Width / 2 - x, 240.0f), Color.Black);
        }

        private void drawStartScreen(SpriteBatch spriteBatch)
        {
            drawStringHorizontallyCentered(spriteBatch, helveticaHuge, "Click or press any key to start", new Vector2(0.0f, 590.0f));
            //drawStringHorizontallyCentered(spriteBatch, helveticaSmallItalic, "As long as we have each other, we'll never run out of problems", new Vector2(0.0f, 600.0f));
        }

        private void drawGameOverScreen(SpriteBatch spriteBatch)
        {
            drawStringHorizontallyCentered(spriteBatch, helveticaMedium, "Sorry to say it, but you let five of your Noms die!", new Vector2(0.0f, 240.0f));
            drawStringHorizontallyCentered(spriteBatch, helveticaHuge, "Click or press any key to continue", new Vector2(0.0f, 340.0f));
        }

        private void drawVictoryScreen(SpriteBatch spriteBatch)
        {
            drawStringHorizontallyCentered(spriteBatch, helveticaMedium, "The Hom shall continue to thrive", new Vector2(0.0f, 240.0f));
            drawStringHorizontallyCentered(spriteBatch, helveticaHuge, "Click or press any key to play again", new Vector2(0.0f, 340.0f));
        }

        private void drawString(SpriteBatch spriteBatch, SpriteFont font, string message, Vector2 location)
        {
            spriteBatch.DrawString(font, message, location, Color.Black);
        }

        private void drawStringHorizontallyCentered(SpriteBatch spriteBatch, SpriteFont font, string message, Vector2 location)
        {
            float x = font.MeasureString(message).X / 2;
            location = new Vector2(GameBoundaries.Width/2 - x, location.Y);
            spriteBatch.DrawString(font, message, location, Color.Black);
        }

        private void drawNavBar()
        {
            spriteBatch.Draw(menuBarTexture, new Rectangle(0, GameBoundaries.Height, GameBoundaries.Width, 100), Color.White);
            spriteBatch.DrawString(helveticaSmall, GameObjectManager.Instance.getDeadScore(), new Vector2(120.0f, 635.0f), Color.Black);
        }

        public void Restart()
        {
            _fGameMinutesElapsed = 0.0f;

            CurrentGameState = GameState.Start;
            GameObjectManager.Instance.Clear();
            GameObjectManager.Instance.resetCountDead();
            spawnStartingObjects();
            loadNavBar();
        }
    }
}
