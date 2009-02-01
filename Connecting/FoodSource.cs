using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Connecting
{
    public class FoodSource : GameObject
    {
        const int c_decayTime = 1500;
        const int c_poofAnim = 1500;
        const int c_nomAnim = 600;

        static Texture2D s_PoofTexture;
        static Texture2D[][] s_FruitTextureSets;
        static Texture2D[] s_NomTextures;

        public enum Fruit
        {
            Grapes = 0,
            Orange = 1,
            Banana = 2,

            Count = 3
        }

        private int _iEatDelay = 0;
        private int _iNextNomAnim = 0;
        private int _iNomAnimFrame = 0;
        private int _iSpawnDelay = c_poofAnim;

        private GameObject _Eater;
        private bool _Dead = false;

        public bool NoStartAnimation = false;

        public override bool CanBeHeld { get { return false; } }
        public Fruit FruitType { get; set; }

        public bool CanEat { get { return !_Dead && _Eater == null; } }
        public GameObject Eater { get { return _Eater; } }

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

        public void StartEating(GameObject aEater)
        {
            _Eater = aEater;
        }

        public void StopEating(GameObject aEater)
        {
            Debug.Assert(aEater == _Eater);
            _Eater = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool Eat(GameTime aTime)
        {
            if (_iEatDelay <= 0)
            {
                _iEatDelay = c_decayTime;
                if (_AmountLeft == 1)
                {
                    _Dead = true;
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

            return !_Dead;
        }

        public override void Update(GameTime aTime)
        {
            if (_iSpawnDelay > 0)
                _iSpawnDelay -= aTime.ElapsedGameTime.Milliseconds;
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_FruitTextureSets[(int)FruitType][0].Width / 2), Location.Y - (float)(s_FruitTextureSets[(int)FruitType][0].Height / 2));
            Texture2D textureToDraw;
            if (_iSpawnDelay > 0 && !NoStartAnimation)
            {
                textureToDraw = s_PoofTexture;
            }
            else
            {
                textureToDraw = s_FruitTextureSets[(int)FruitType][_AmountLeft - 1];
            }

            aBatch.Draw(textureToDraw, draw_loc, Color.White);

            if(_Eater != null)
            {
                if(_iNextNomAnim <=0)
                {
                    _iNextNomAnim = c_nomAnim;
                    _iNomAnimFrame++;
                    if(_iNomAnimFrame == s_NomTextures.Length)
                        _iNomAnimFrame = 0;
                }
                else
                    _iNextNomAnim -= aTime.ElapsedGameTime.Milliseconds;

                aBatch.Draw(s_NomTextures[_iNomAnimFrame], draw_loc, Color.White);
            }
        }

        public override string GetDebugInfo()
        {
            return String.Format("Location: {0}, CanEat: {1}, Eater: {2}", Location.ToString(), CanEat, _Eater);
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
                s_FruitTextureSets[(int)Fruit.Banana][i] = aManager.Load<Texture2D>("food/food_banana_" + i);
            }

            s_PoofTexture = aManager.Load<Texture2D>("food/food_poof_1");
            s_NomTextures = new Texture2D[] {
                aManager.Load<Texture2D>("food/eating_nom_1"),
                aManager.Load<Texture2D>("food/eating_nom_2")
            };
        }
    }
}