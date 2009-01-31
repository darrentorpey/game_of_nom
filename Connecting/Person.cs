using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Connecting
{
    public class Person : GameObject
    {
        const float c_fSpeed = 3.0f;

        public enum Mood
        {
            Happy = 0,
            Sad = 1,
            Angry = 2
        }

        private static Texture2D[] s_MoodTextures;
        private static Texture2D s_HeldTexture;

        public float Instability = 0.0f;
        private Mood MyMood = Mood.Sad;
        private bool _bHeld = false;
        private GameObject _CollidingObject = null;
        private Stack<GameObject> _NearbyFoodSources = new Stack<GameObject>();

        public bool InFlock = false;

        public override float Radius { get { return 13.0f; } }

        public Person(Vector2 aStartLocation)
        {
            Location = aStartLocation;
        }

        public override void Hold()
        {
            _bHeld = true;
        }

        public override void Drop()
        {
            if (_CollidingObject != null)
            {
                GameObjectManager manager = GameObjectManager.Instance;
                if (_CollidingObject is PersonFlock)
                {
                    manager._Objects.Remove(this);
                    ((PersonFlock)_CollidingObject).AddPerson(this);
                }
                else if (_CollidingObject is Person)
                {
                    manager._Objects.Remove(this);
                    manager._Objects.Remove(_CollidingObject);
                    PersonFlock flock = new PersonFlock();
                    flock.AddPerson(this);
                    flock.AddPerson((Person)_CollidingObject);
                    flock.Location = this.Location;
                    manager._Objects.Add(flock);
                }

                _CollidingObject = null;
            }

            _bHeld = false;
        }

        public override void Update(GameTime aTime)
        {
            if (InFlock)
            {
                if (Instability < 20.0f)
                    MyMood = Mood.Happy;
                else if (Instability < 50.0f)
                    MyMood = Mood.Sad;
                else
                    MyMood = Mood.Angry;
            }

            if (_bHeld)
            {
                _NearbyFoodSources.Clear();

                GameObjectManager manager = GameObjectManager.Instance;

                // Look to see if we need to indicate that droping this Person will change their mood
                _CollidingObject = null;
                for (int i = 0; i < manager._Objects.Count; ++i)
                {
                    GameObject currObj = manager._Objects[i];
                    if (this != currObj)
                    {
                        if (currObj.CollidesWith(this))
                        {
                            _CollidingObject = currObj;
                            if (currObj is FoodSource)
                            {
                                _NearbyFoodSources.Push(currObj);
                            }
                        }
                        else if (currObj.InProximity(this, 10.0f) && currObj is FoodSource)
                        {
                            _NearbyFoodSources.Push(currObj);
                        }
                    }

                    if (_CollidingObject != null && _CollidingObject is Person)
                        // If we're colliding with a person, we're happy (we're very social!)
                        MyMood = Mood.Happy;
                    else if (_NearbyFoodSources.Count != 0) {
                        // If we're near available food, we're happy
                        MyMood = Mood.Happy;
                    } else {
                        // Otherwise, that makes us a SAD PANDA
                        MyMood = Mood.Sad;
                    }
                }

            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_MoodTextures[0].Width / 2), Location.Y - (float)(s_MoodTextures[0].Height / 2));
            aBatch.Draw(s_MoodTextures[(int)MyMood], draw_loc, Color.White);

            if (_bHeld)
                aBatch.Draw(s_HeldTexture, draw_loc, Color.White);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_MoodTextures = new Texture2D[] {
                aManager.Load<Texture2D>("happy"),
                aManager.Load<Texture2D>("sad"),
                aManager.Load<Texture2D>("angry")
            };

            s_HeldTexture = aManager.Load<Texture2D>("halo");
        }
    }
}
