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

        Vector2 _Location;

        public Vector2 Location
        {
            get { return _Location; }
        }

        public Person(Vector2 aStartLocation)
        {
            _Location = aStartLocation;
        }

        public void Update(GameTime aTime)
        {

        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            aBatch.Draw(s_Texture, _Location, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_Texture = aManager.Load<Texture2D>("PersonSprite");
        }
    }
}
