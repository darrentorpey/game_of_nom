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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObject inTransitByUser;

        SpriteFont font;
        MouseState lastMouseState;
        KeyboardState lastKeyState;
        PersonFlock _Flock;
        bool _bSingleStep = false;

        public Game1()
        {
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
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
            DrawUtils.LoadContent(Content);
            font = Content.Load<SpriteFont>("Helvetica");

            spawnStartingObjects();
        }

        private void spawnStartingObjects()
        {
            // Add lots of people around randomly
            _Flock = new PersonFlock();
            _Flock.Location = new Vector2(RandomInstance.Instance.Next(0, Window.ClientBounds.Width),
                RandomInstance.Instance.Next(0, Window.ClientBounds.Height));
            Rectangle bounds = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            for (int i = 0; i < 10; ++i)
            {
                Person newPerson = new Person(_Flock.Location, bounds);
                _Flock.AddPerson(newPerson);
            }

            // Init people here so that we know the content (textures, etc.) are loaded
            GameObjectManager.Instance._Objects = new List<GameObject> {
                _Flock
            };

            for (int i = 0; i < 5; ++i) {
                GameObjectManager.Instance._Objects.Add(new Person(getRandomLocation(100), bounds));
            }

            for (int i = 0; i < 5; ++i)
            {
                GameObjectManager.Instance._Objects.Add(new FoodSource(getRandomLocation(100), FoodSource.Fruit.Grapes));
            }

            for (int i = 0; i < 5; ++i)
            {
                GameObjectManager.Instance._Objects.Add(new FoodSource(getRandomLocation(100), FoodSource.Fruit.Orange));
            }
        }

        private Vector2 getRandomLocation(int borderPadding)
        {
            return new Vector2(
                RandomInstance.Instance.Next(0, Window.ClientBounds.Width - borderPadding),
                RandomInstance.Instance.Next(0, Window.ClientBounds.Height - borderPadding));
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

            processMouseEvents();

            KeyboardState keyState = Keyboard.GetState(PlayerIndex.One);
            if(lastKeyState == null)
                lastKeyState = keyState;

            if (_bSingleStep)
            {
                if (keyState.IsKeyDown(Keys.OemPlus) && !lastKeyState.IsKeyDown(Keys.OemPlus))
                    GameObjectManager.Instance.Update(gameTime);
            }
            else
                GameObjectManager.Instance.Update(gameTime);

            lastKeyState = keyState;

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        private void processMouseEvents()
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
                    for (int i = 0; i < manager._Objects.Count; ++i)
                    {
                        if(manager._Objects[i].RadiusCheck(ref mouseLoc, 0.0f))
                        {
                           this.inTransitByUser = manager._Objects[i];
                           manager._Objects[i].Hold();
                        }
                    }
                }
            }

            if (state.RightButton == ButtonState.Pressed && 
                lastMouseState.RightButton != ButtonState.Pressed)
            {
                _Flock.AddExtenralForce(new ExternalForce(mouseLoc, 600.0f, 3.0f, 120));
            }

            if (this.inTransitByUser != null)
            {
                this.inTransitByUser.Location = mouseLoc;
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
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            
            GameObjectManager.Instance.Draw(spriteBatch, gameTime);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
