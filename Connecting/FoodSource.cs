using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    public class FoodSource : GameObject
    {

        const float c_decayMultiplier = 5.0f;
        int _TicksEaten = 0;
        public bool Dead = false;

        private static Texture2D[] s_FoodTextures;

        private bool _BeingEaten = false;

        public bool BeingEaten { get { return _BeingEaten; } set { _BeingEaten = value; } }

        public override float Radius { get { return 30.0f; } }

        public override void Drop()
        {
            //throw new NotImplementedException("Can't drop food");
        }

        public override void Hold()
        {
            //throw new NotImplementedException("Can't hold food");
        }

        public static int s_Capacity = 6;
        public int _AmountLeft = s_Capacity;
        public int AmountLeft { get { return _AmountLeft; } }

        public FoodSource() {
            // Always start a food source at full capacity
            _AmountLeft = s_Capacity;
        }

        public FoodSource(Vector2 aStartLocation)
        {
            Location = aStartLocation;
        }

        public override void Update(GameTime aTime)
        {
            //if (this is FoodSource) {
            //    Console.WriteLine("hello " + ((FoodSource)this)._BeingEaten);
            //}

            _TicksEaten++;

            if (BeingEaten && _TicksEaten%(10*c_decayMultiplier) == 0)
            {
                if (_AmountLeft == 1)
                {
                    Dead = true;
                }
                else
                {
                    _AmountLeft--;
                }
            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_FoodTextures[0].Width / 2), Location.Y - (float)(s_FoodTextures[0].Height / 2));
            aBatch.Draw(s_FoodTextures[_AmountLeft - 1], draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_FoodTextures = new Texture2D[s_Capacity];
            for (int i = 0; i < s_Capacity; ++i)
            {
                s_FoodTextures[i] = aManager.Load<Texture2D>("food/food_grapes_" + i);
            }
        }

    }
}