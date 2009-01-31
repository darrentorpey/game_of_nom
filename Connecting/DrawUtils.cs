using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    class DrawUtils
    {
        private static Texture2D _Pixel;
        
        public static void LoadContent(ContentManager aManager)
        {
            _Pixel = aManager.Load<Texture2D>("Pixel");
        }

        public static void DrawLine(SpriteBatch aBatch, Vector2 aFrom, Vector2 aTo, Color aColor)
        {
            int distance = (int)Vector2.Distance(aFrom, aTo);

            Vector2 connection = aFrom - aTo;
            Vector2 baseVector = new Vector2(1, 0);

            float angle = (float)Math.Atan2((double)(aTo.Y - aFrom.Y),
                    (double)(aTo.X - aFrom.X));

            aBatch.Draw(_Pixel, aFrom, null, aColor, angle, Vector2.Zero, new Vector2(distance, 1),
                    SpriteEffects.None, 0);
        }

        public static void DrawPoint(SpriteBatch aBatch, Vector2 aPoint, int aiSize, Color aColor)
        {
            aBatch.Draw(_Pixel, new Rectangle((int) aPoint.X - aiSize / 2, (int)aPoint.Y - aiSize / 2,
                aiSize, aiSize), aColor);
        }
    }
}
