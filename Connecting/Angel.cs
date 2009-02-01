using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    class Angel : GameObject
    {
        static Texture2D s_AngelTexture;

        public override float Radius { get { return 30.0f; } }

        public static float s_SpeedFactor = 300.0f;

        private Person _MortalRemains;

        public Angel(Vector2 aStartLocation, Person mortalRemains)
        {
            Location = aStartLocation;
            _MortalRemains = mortalRemains;
        }

        public Angel(Vector2 aStartLocation, bool justForLooks)
        {
            Location = aStartLocation;
            _JustForLooks = justForLooks;
        }

        private bool _JustForLooks;

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
            if (!_JustForLooks)
            {
                Vector2 destination = new Vector2(100, 650);
                Vector2 mainVec = Vector2.Subtract(destination, Location);
                mainVec.Normalize();
                Vector2 vec = new Vector2(mainVec.X * s_SpeedFactor, mainVec.Y * s_SpeedFactor);
                Location = Location + (vec * (float)aTime.ElapsedGameTime.TotalSeconds);

                if (Vector2.Distance(Location, destination) < 10.0)
                {
                    // Our journey has completed
                    GameObjectManager.Instance.RemoveObject(this);
                    GameObjectManager.Instance.registerDead(_MortalRemains);
                }
            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_AngelTexture.Width / 2), Location.Y - (float)(s_AngelTexture.Height / 2));
            aBatch.Draw(s_AngelTexture, draw_loc, Color.White);
        }


        public static void LoadContent(ContentManager aManager)
        {
            s_AngelTexture = aManager.Load<Texture2D>("score_angel_1");
        }


    }
}
