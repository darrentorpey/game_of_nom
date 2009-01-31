using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    public class Person
    {
        const float c_fSpeed = 3.0f;

        public static Texture2D s_BallRed;
        public static Texture2D s_BallGreen;
        public static Texture2D s_BallYellow;

        public Vector2 Location;
        public Vector2 FlockLocation;

        public Texture2D _TheTexture;


        public Person(Vector2 aStartLocation)
        {
            Location = aStartLocation;
            this._TheTexture = Person.s_BallRed;
        }

        public void Update(GameTime aTime)
        {
            
        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_BallRed.Width / 2), Location.Y - (float)(s_BallRed.Height / 2));
            aBatch.Draw(this._TheTexture, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_BallRed = aManager.Load<Texture2D>("PersonSprite");
            s_BallGreen = aManager.Load<Texture2D>("PersonSprite2");
            s_BallYellow = aManager.Load<Texture2D>("PersonSprite3");

            // Set the default texture
            //this._TheTexture = s_BallRed;
        }
    }
}
