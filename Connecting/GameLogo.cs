using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    class GameLogo : UIObject
    {
        static Texture2D s_MainTexture;

        public GameLogo(Vector2 startLocation) : base(startLocation)
        {
            Location = startLocation;
        }

        protected override Texture2D getMainTexture()
        {
            return s_MainTexture;
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_MainTexture = aManager.Load<Texture2D>("gui/ui_logo_1");
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            aBatch.Draw(getMainTexture(), new Rectangle((int)(Location.X - (float)(s_MainTexture.Width / 2)), (int)Location.Y, getMainTexture().Width, getMainTexture().Height), Color.White);
        }
    }
}
