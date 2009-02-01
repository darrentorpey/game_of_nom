using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    abstract class UIObject : GameObject
    {
        //protected Texture2D s_MainTexture;

        public override float Radius { get { return getMainTexture().Width; } }

        public UIObject(Vector2 startLocation)
        {
            Location = startLocation;
        }

        public override void Update(GameTime aTime) {}

        protected abstract Texture2D getMainTexture();

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(getMainTexture().Width / 2), Location.Y - (float)(getMainTexture().Height / 2));
            aBatch.Draw(getMainTexture(), draw_loc, Color.White);
        }
    }
}
