﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    class Tombstone : UIObject
    {
        static Texture2D s_MainTexture;

        public Tombstone(Vector2 startLocation): base(startLocation)
        {
            Location = startLocation;
        }

        protected override Texture2D getMainTexture()
        {
            return s_MainTexture;
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_MainTexture = aManager.Load<Texture2D>("score_tombstone_1");
        }
    }
}
