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

        public static Texture2D s_FaceSad;
        public static Texture2D s_FaceHappy;
        public static Texture2D s_FaceYellow;

        public Vector2 Location;
        public Vector2 FlockLocation;

        public Texture2D _TheTexture;


        public Person(Vector2 aStartLocation)
        {
            Location = aStartLocation;
            this._TheTexture = Person.s_FaceSad;
        }

        public void Update(GameTime aTime)
        {
            
        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_FaceSad.Width / 2), Location.Y - (float)(s_FaceSad.Height / 2));
            aBatch.Draw(this._TheTexture, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_FaceSad = aManager.Load<Texture2D>("sad");
            s_FaceHappy = aManager.Load<Texture2D>("angry");
            s_FaceYellow = aManager.Load<Texture2D>("yellow");
        }
    }
}
