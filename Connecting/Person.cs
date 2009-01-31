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

        public Person()
        {
            _Location = Vector2.Zero;
        }

        public void Update(GameTime aTime)
        {

        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(_Location.X - (float)(s_Texture.Width/2), _Location.Y - (float)(s_Texture.Height/2));
            aBatch.Draw(s_Texture, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_Texture = aManager.Load<Texture2D>("PersonSprite");
        }
    }
}
