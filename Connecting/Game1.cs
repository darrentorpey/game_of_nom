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

        SpriteFont font;

        PersonFlock _Flock = new PersonFlock();

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
            font = Content.Load<SpriteFont>("Helvetica");

            Random rand = new Random();
            // Add lots of people around randomly
            for (int i = 0; i < 20; ++i)
            {
                Person newPerson = new Person(
                    new Vector2(rand.Next(0, Window.ClientBounds.Width), rand.Next(0, Window.ClientBounds.Height)));
                _Flock.AddPerson(newPerson);
            }

            FoodSource theFood = new FoodSource(new Vector2(rand.Next(0, Window.ClientBounds.Width), rand.Next(0, Window.ClientBounds.Height)));

            _Flock.Location = new Vector2(rand.Next(0, Window.ClientBounds.Width), rand.Next(0, Window.ClientBounds.Height));
            // Init people here so that we know the content (textures, etc.) are loaded
            GameObjectManager.Instance._Objects = new List<GameObject> {
                new Person(new Vector2(100, 100)), 
                new Person(new Vector2(200, 200)),
                _Flock,
                theFood
            };
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

            GameObjectManager.Instance.Update(gameTime);

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        GameObject inTransitByUser;

        private void processMouseEvents()
        {
            MouseState state = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(state.X, state.Y);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
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

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, _Flock.CalculateCenterOfMass().ToString(), Vector2.Zero, Color.Black); 
            
            GameObjectManager.Instance.Draw(spriteBatch, gameTime);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
