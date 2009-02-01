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
        const int c_decayTime = 1500;

        static Texture2D s_PoofTexture;
        static Texture2D[][] s_FruitTextureSets;

        public enum Fruit
        {
            Grapes = 0,
            Orange = 1,

            Count = 2
        }

        private int _ticksSinceSpawn = 0;
        private int _iEatDelay = 0;
        private bool _BeingEaten = false;

        public bool Dead = false;
        public Fruit FruitType { get; set; }    
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

        public FoodSource(Vector2 aStartLocation, Fruit fruit)
        {
            Location = aStartLocation;
            FruitType = fruit;
        }

        public override void Update(GameTime aTime)
        {
            _ticksSinceSpawn++;

            if (BeingEaten)
            {
                if (_iEatDelay <= 0)
                {
                    _iEatDelay = c_decayTime;
                    if (_AmountLeft == 1)
                    {
                        Dead = true;
                        BeingEaten = false; // This location may be a bad idea
                        SoundState.Instance.PlayNomSound(this, aTime);
                        // Remove self from the game object manager
                        GameObjectManager.Instance.RemoveObject(this);
                    }
                    else
                    {
                        SoundState.Instance.PlayNomSound(this, aTime);
                        _AmountLeft--;
                    }
                }
                else
                    _iEatDelay -= aTime.ElapsedGameTime.Milliseconds;
            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_FruitTextureSets[(int)FruitType][0].Width / 2), Location.Y - (float)(s_FruitTextureSets[(int)FruitType][0].Height / 2));
            Texture2D textureToDraw;
            if (_ticksSinceSpawn < 35)
            {
                textureToDraw = s_PoofTexture;
            }
            else
            {
                textureToDraw = s_FruitTextureSets[(int)FruitType][_AmountLeft - 1];
            }
            aBatch.Draw(textureToDraw, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_FruitTextureSets = new Texture2D[(int)Fruit.Count][];
            for (int i = 0; i < s_FruitTextureSets.Count(); ++i)
            {
                s_FruitTextureSets[i] = new Texture2D[s_Capacity];
            }

            s_FruitTextureSets[(int)Fruit.Grapes] = new Texture2D[s_Capacity];
            for (int i = 0; i < s_Capacity; ++i)
            {
                s_FruitTextureSets[(int)Fruit.Grapes][i] = aManager.Load<Texture2D>("food/food_grapes_" + i);
                s_FruitTextureSets[(int)Fruit.Orange][i] = aManager.Load<Texture2D>("food/food_orange_" + i);
            }

            s_PoofTexture = aManager.Load<Texture2D>("food/food_poof_1");
        }

    }
}