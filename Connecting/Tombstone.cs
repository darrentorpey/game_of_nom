using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    class Tombstone : GameObject
    {
        static Texture2D s_TombstoneTexture;

        public override float Radius { get { return 30.0f; } }

        public Tombstone(Vector2 aStartLocation)
        {
            Location = aStartLocation;
        }

        
        public override void Drop()
        {
            //throw new NotImplementedException("Can't drop food");
        }

        public override void Hold()
        {
            //throw new NotImplementedException("Can't hold food");
        }

        public override void Update(GameTime aTime)
        {
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_TombstoneTexture.Width / 2), Location.Y - (float)(s_TombstoneTexture.Height / 2));
            aBatch.Draw(s_TombstoneTexture, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_TombstoneTexture = aManager.Load<Texture2D>("score_tombstone_1");
        }


    }
}
