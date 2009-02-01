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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static int GAME_HEIGHT = 600;
        public static int GAME_WIDTH = 900;
        Rectangle GameBoundaries = new Rectangle(0, 0, GAME_WIDTH, GAME_HEIGHT);

        Texture2D menuBarTexture;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObject inTransitByUser;

        float _fFruitSpawnRate = 20.0f;

        SpriteFont font;
        MouseState lastMouseState;
        KeyboardState lastKeyState;
        PersonFlock _Flock;
        bool _bSingleStep = false;

        public Game1()
        {
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GameBoundaries.Width;
            graphics.PreferredBackBufferHeight = GameBoundaries.Height + 100;
            Content.RootDirectory = "Content";
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
            SoundState.Instance.ToggleSound();
            font = Content.Load<SpriteFont>("Helvetica");

            menuBarTexture = Content.Load<Texture2D>("menu_bar");

            spawnStartingObjects();

            loadNavBar(spriteBatch);
        }

        private void loadNavBar(SpriteBatch spriteBatch)
        {
            GameObjectManager.Instance.AddObject(new Angel(new Vector2(100.0f, 650.0f), true));
        }

        private void spawnStartingObjects()
        {
            // Add lots of people around randomly
            _Flock = new PersonFlock();
            _Flock.Location = new Vector2(RandomInstance.Instance.Next(0, GameBoundaries.Width),
                RandomInstance.Instance.Next(0, GameBoundaries.Height));
            Rectangle bounds = new Rectangle(13, 13, GameBoundaries.Width, GameBoundaries.Height - 26);
            for (int i = 0; i < 10; ++i)
            {
                Person newPerson = new Person(_Flock.Location, bounds);
                _Flock.AddPerson(newPerson);
            }

            // Init people here so that we know the content (textures, etc.) are loaded
            GameObjectManager.Instance.AddObject(_Flock);

            for (int i = 0; i < 10; ++i) {
                GameObjectManager.Instance.AddObject(new Person(getRandomLocation(50), bounds));
            }

            for (int i = 0; i < 3; ++i)
            {
                spawnFruit(FoodSource.Fruit.Grapes, true);
            }

            for (int i = 0; i < 3; ++i)
            {
                spawnFruit(FoodSource.Fruit.Orange, true);
            }

            for (int i = 0; i < 3; ++i)
            {
                spawnFruit(FoodSource.Fruit.Banana, true);
            }

            //GameObjectManager.Instance.AddObject(new Angel(getRandomLocation(50)));
        }

        private Vector2 getRandomLocation(int borderPadding)
        {
            return new Vector2(
                RandomInstance.Instance.Next(borderPadding, GameBoundaries.Width - borderPadding),
                RandomInstance.Instance.Next(borderPadding, GameBoundaries.Height - borderPadding));
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            processMouseEvents(gameTime);

            processKeyboardEvents(gameTime);

            spawnMoreFood();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        private int ticksTillFruitSpawn;

        private void spawnMoreFood()
        {
            if (0 == ticksTillFruitSpawn)
            {
                spawnRandomFruit();
                ticksTillFruitSpawn = (int)(10 * _fFruitSpawnRate);
            }
            else
            {
                ticksTillFruitSpawn--;
            }
        }

        private void spawnRandomFruit()
        {
            spawnFruit((FoodSource.Fruit)(RandomInstance.Instance.Next(0, (int)FoodSource.Fruit.Count)));
        }

        private void spawnFruit(FoodSource.Fruit fruitType)
        {
            GameObjectManager.Instance.AddObject(new FoodSource(getRandomLocation(50), fruitType));
        }

        private void spawnFruit(FoodSource.Fruit fruitType, bool skipStartAnimation)
        {
            FoodSource foodSource = new FoodSource(getRandomLocation(50), fruitType);
            foodSource.NoStartAnimation = skipStartAnimation;
            GameObjectManager.Instance.AddObject(foodSource);
        }

        private void spawnPerson()
        {
            Rectangle bounds = new Rectangle(0, 0, GameBoundaries.Width, GameBoundaries.Height);
            GameObjectManager.Instance.AddObject(new Person(getRandomLocation(50), bounds));
        }

        private void processKeyboardEvents(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            if (lastKeyState == null)
                lastKeyState = keyState;

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
                            if (!(manager[i] is FoodSource) && !(manager[i] is Angel) && !(manager[i] is Tombstone) && !(manager[i] is Person && ((Person)manager[i]).Dead))
                            {
                                this.inTransitByUser = manager[i];
                                manager[i].Hold();
                                SoundState.Instance.PlayPickupSound(aTime);
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
                                manager[i]._Hunger += 50;
                            }
                        }
                    }
                }
                else
                {
                    _Flock.AddExtenralForce(new ExternalForce(mouseLoc, 600.0f, 3.0f, 120));
                }
            }

            if (this.inTransitByUser != null)
            {
                int x = (int)Math.Max(this.inTransitByUser.Radius, Math.Min(mouseLoc.X, GameBoundaries.Width - this.inTransitByUser.Radius));
                int y = (int)Math.Max(this.inTransitByUser.Radius, Math.Min(mouseLoc.Y, GameBoundaries.Height - this.inTransitByUser.Radius));
                this.inTransitByUser.Location = new Vector2(x, y);
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
            GraphicsDevice.Clear(Color.LightCyan);

            spriteBatch.Begin();

            drawNavBar();
            
            GameObjectManager.Instance.Draw(spriteBatch, gameTime);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawNavBar()
        {
            spriteBatch.Draw(menuBarTexture, new Rectangle(0, GameBoundaries.Height, GameBoundaries.Width, 100), Color.White);
            spriteBatch.DrawString(font, GameObjectManager.Instance.getDeadScore(), new Vector2(120.0f, 635.0f), Color.Black);
        }

        public void Restart()
        {
            GameObjectManager.Instance.Clear();
            spawnStartingObjects();
        }
    }
}
