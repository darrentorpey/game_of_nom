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
        static Texture2D s_Texture;

        public Vector2 Location;

        public Person(Vector2 aStartLocation)
        {
            Location = aStartLocation;
        }

        public void Update(GameTime aTime)
        {

        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            aBatch.Draw(s_Texture, Location, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_Texture = aManager.Load<Texture2D>("PersonSprite");
        }
    }
}
