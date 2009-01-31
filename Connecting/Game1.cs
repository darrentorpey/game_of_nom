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
        Person[] _Persons;

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
            font = Content.Load<SpriteFont>("Helvetica");

            Random rand = new Random();
            // Add lots of people around randomly
            for (int i = 0; i < 20; ++i)
            {
                _Flock.AddPerson(new Person(
                    new Vector2(rand.Next(0, Window.ClientBounds.Width), rand.Next(0, Window.ClientBounds.Height))
                ));
            }

            _Flock.TargetLocation = new Vector2(rand.Next(0, Window.ClientBounds.Width), rand.Next(0, Window.ClientBounds.Height));
            // Init people here so that we know the content (textures, etc.) are loaded
            _Persons = new Person[] {
                new Person(new Vector2(100, 100)), 
                new Person(new Vector2(200, 200))
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

            _Flock.Update(gameTime);
            processMouseEvents();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        Person inTransitByUser;

        private void processMouseEvents()
        {
            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (this.inTransitByUser == null)
                {
                    //Console.Out.WriteLine("helloooooo nurse!");
                    for (int i = 0; i < _Persons.Length; ++i)
                    {
                        float dist = Vector2.Distance(_Persons[i].Location, new Vector2(mouseX, mouseY));
                        //Console.Out.WriteLine("dist: " + dist);
                        if (dist < 13.0)
                        {
                            //Console.Out.WriteLine("on the dot");
                            this.inTransitByUser = _Persons[i];
                            //_Persons[i]._TheTexture = Content.Load<Texture2D>("PersonSprite2");
                            Console.Out.WriteLine("YELLOW");
                            this.inTransitByUser._TheTexture = Person.s_BallYellow;
                        }
                    }
                }
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Console.Out.WriteLine("bye nurse!");
            }

            if (this.inTransitByUser != null)
            {
                this.inTransitByUser.Location = new Vector2(mouseX, mouseY);
                for (int i = 0; i < _Persons.Length; ++i)
                {
                    bool touching = false;
                    if (this.inTransitByUser != _Persons[i] && Vector2.Distance(this.inTransitByUser.Location, _Persons[i].Location) < 26.0) {
                        touching = true;
                    }
                    if (touching)
                    {
                        Console.Out.WriteLine("GREEN");
                        this.inTransitByUser._TheTexture = Person.s_BallGreen;
                    }
                    else
                    {
                        Console.Out.WriteLine("RED");
                        this.inTransitByUser._TheTexture = Person.s_BallRed;
                    }
                    
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                if (this.inTransitByUser != null)
                {
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
            _Flock.Draw(spriteBatch, gameTime);
            for (int i = 0; i < _Persons.Length; ++i)
                _Persons[i].Draw(spriteBatch, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
