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
        const float c_fSpeed = 10.0f;
        const int c_iThinkTime = 12000;
        const int c_iRandDelay = 420;
        const int c_iLookTime = 2000;
        const int c_iLookDelay = 400;
        const float FOOD_PROXIMITY = 10.0f;

        public enum Mood
        {
            Happy = 0,
            Sad = 1,
            Angry = 2,
            Confused = 3
        }

        public enum LookDirection
        {
            Left,
            Right
        }

        private static Texture2D[] s_MoodTextures;
        private static Texture2D s_HeldTexture;
        private static Color[] _ForceColors = new Color[] {
            Color.Red, Color.Black, Color.Green, Color.Blue, Color.Pink
        };
                
        private Mood MyMood = Mood.Sad;
        private Mood? MyHoverMood = null;
        private LookDirection MyLook = LookDirection.Left;
        
        private bool _bHeld = false;
        private GameObject _CollidingObject = null;
        private Stack<FoodSource> _NearbyFoodSources = new Stack<FoodSource>();
        private Vector2 _Velocity;
        private Rectangle _Bounds;
        private int _iNextThink = 0;
        private int _iNextLook = 0;
        private bool _bLooking = false;
        private Vector2 _WalkVelocity = Vector2.Zero;

        private Vector2[] _Forces = new Vector2[5];      

        public float Instability { get; set; }
        public PersonFlock ParentFlock { get; set; }
        public Vector2 Velocity { get { return _Velocity; } }
        public override float Radius { get { return 13.0f; } }
        
        public Person(Vector2 aStartLocation, Rectangle aBounds)
        {
            Location = aStartLocation;
            _Bounds = aBounds;

            _Velocity = new Vector2((float)RandomInstance.Instance.NextDouble(), 
                (float)RandomInstance.Instance.NextDouble());
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

            if (EatingObject != null && !EatingObject.InProximity(this, FOOD_PROXIMITY))
            {
                EatingObject.BeingEaten = false;
                EatingObject = null;
            }

            if (_NearbyFoodSources.Count > 0)
            {
                _NearbyFoodSources.First().BeingEaten = true;
                EatingObject = _NearbyFoodSources.First();
            }

            _bHeld = false;
        }

        public override void Update(GameTime aTime)
        {

            if (ParentFlock != null)
            {
                AccumulateForces();
                Location = Location + (_Velocity * (float)aTime.ElapsedGameTime.TotalSeconds);

                if (Instability < 20.0f)
                    MyMood = Mood.Happy;
                else if (Instability < 50.0f)
                    MyMood = Mood.Sad;
                else
                    MyMood = Mood.Angry;
            }
            else
            {    
                if (_bHeld)
                {
                    alterMyMood(false);
                }
                else
                {
                    AccumulateForces();
                    if (_iNextThink <= 0)
                    {
                        _iNextThink = c_iThinkTime + RandomInstance.Instance.Next(-c_iRandDelay, c_iRandDelay);
                        switch (RandomInstance.Instance.Next(0, 3))
                        {
                            case 0: // Stay Still
                                _bLooking = false;
                                _WalkVelocity = Vector2.Zero;
                                break;
                            case 1: // Wander in a random direction
                                _bLooking = false;
                                _WalkVelocity = new Vector2(RandomInstance.Instance.Next(-100, 100) / 100.0f,
                                    RandomInstance.Instance.Next(-100, 100) / 100.0f);
                                _WalkVelocity.Normalize();
                                if (_WalkVelocity.X < 0)
                                    MyLook = LookDirection.Left;
                                else
                                    MyLook = LookDirection.Right;

                                _WalkVelocity *= c_fSpeed;
                                break;
                            case 2:  // Look around?
                                _bLooking = true;
                                _iNextLook = c_iLookTime + RandomInstance.Instance.Next(-c_iLookDelay, c_iLookDelay);
                                _WalkVelocity = Vector2.Zero;
                                break;
                        }

                        switch (RandomInstance.Instance.Next(0, 2))
                        {
                            case 0: MyMood = Mood.Sad; break;
                            case 1: MyMood = Mood.Confused; break;
                        }
                    }
                    else
                        _iNextThink -= aTime.ElapsedGameTime.Milliseconds;

                    if (_bLooking)
                    {
                        if (_iNextLook <= 0)
                        {
                            _iNextLook = c_iLookTime + RandomInstance.Instance.Next(-c_iLookDelay, c_iLookDelay);
                            MyLook = MyLook == LookDirection.Left ? LookDirection.Right : LookDirection.Left;
                        }
                        else
                            _iNextLook -= aTime.ElapsedGameTime.Milliseconds;
                    }

                    _Velocity += _WalkVelocity;
                    Location = Location + (_Velocity * (float)aTime.ElapsedGameTime.TotalSeconds);

                    if (Location.X < _Bounds.X || Location.X > _Bounds.X + _Bounds.Width)
                    {
                        _WalkVelocity.X = -_WalkVelocity.X;
                        MyLook = MyLook == LookDirection.Left ? LookDirection.Right : LookDirection.Left;
                    }

                    if (Location.Y < _Bounds.Y || Location.Y > _Bounds.Y + _Bounds.Height)
                        _WalkVelocity.Y = -_WalkVelocity.Y;
                }
            }
        }

        private void alterMyMood(bool realMood)
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
                        if (currObj is FoodSource && (!((FoodSource)(currObj)).BeingEaten || this.EatingObject == currObj))
                        {
                            _NearbyFoodSources.Push((FoodSource)currObj);
                        }
                    }
                    else if (currObj is FoodSource && (!((FoodSource)(currObj)).BeingEaten || this.EatingObject == currObj) && currObj.InProximity(this, FOOD_PROXIMITY))
                    {
                        _NearbyFoodSources.Push((FoodSource)currObj);
                    }
                    else
                    {
                        Console.WriteLine("Here: " + currObj.InProximity(this, FOOD_PROXIMITY) + ", " + (this.EatingObject == currObj));
                    }
                }
            }

            if (_CollidingObject != null && _CollidingObject is Person)
                // If we're colliding with a person, we're happy (we're very social!)
                MyMood = Mood.Happy;
            else if (_NearbyFoodSources.Count != 0)
            {
                // If we're near available food, we're happy
                MyMood = Mood.Happy;
            }
            else
            {
                // Otherwise, that makes us a SAD PANDA
                MyMood = Mood.Sad;
            }

            //if (EatingObject != null && !EatingObject.InProximity(this, FOOD_PROXIMITY))
            //{
            //    EatingObject.BeingEaten = false;
            //    EatingObject = null;
            //}

            //if (_NearbyFoodSources.Count > 0)
            //{
            //    _NearbyFoodSources.First().BeingEaten = true;
            //    EatingObject = _NearbyFoodSources.First();
            //}
        }

        public void AccumulateForces()
        {
            Vector2 forces = Vector2.Zero;

            for (int i = 0; i < 5; ++i)
                _Forces[i] = Vector2.Zero;

            if (ParentFlock != null)
            {
                _Forces[4] = ParentFlock.GetExternalForces(this);

                // Always move toward CoM.  Pull harder if farther away.
                _Forces[0] = GetForceToward(ParentFlock.CurrentCoM, .2f, Radius * ParentFlock.People.Count);
                
                // Always move toward Target location
                _Forces[1] = GetForceToward(ParentFlock.Location, .3f, 2.5f);
                //forces += GetForceToward(ParentFlock.Location, 3.0f, Radius * ParentFlock.People.Count);

                // Move away from all other boids
                Vector2 avoidanceForce = Vector2.Zero;
                Vector2 matchingForce = Vector2.Zero;
                for (int i = 0; i < ParentFlock.People.Count; ++i)
                {
                    if (ParentFlock.People[i] == this)
                        continue;

                    float dist;
                    Vector2.Distance(ref ParentFlock.People[i].Location, ref this.Location, out dist);
                    if (dist < 40.0f)
                    {
                        Vector2 force = (this.Location - ParentFlock.People[i].Location);
                        avoidanceForce += force;
                    }

                    // Match velocity with other boids
                    matchingForce += ParentFlock.People[i].Velocity;
                }
                _Forces[2] = avoidanceForce;
                //_Forces[3] = matchingForce / (ParentFlock.People.Count - 1) * .5f;

                for (int i = 0; i < 5; ++i)
                    forces += _Forces[i];
            }

            if (Location.X < _Bounds.X)
                forces.X += (_Bounds.X - Location.X) * 100.0f;
            else if (Location.X > _Bounds.X + _Bounds.Width)
                forces.X += ((_Bounds.X + _Bounds.Width) - Location.X) * 100.0f;

            if (Location.Y < _Bounds.Y)
                forces.Y += (_Bounds.Y - Location.Y) * 100.0f;
            else if (Location.Y > _Bounds.Y + _Bounds.Height)
                forces.Y += ((_Bounds.Y + _Bounds.Height) - Location.Y) * 100.0f;
                
            // Always involve friction
            forces += (-_Velocity) * .1f;

            // Assume 1.0 mass, force is acceleration
            _Velocity += forces;
        }

        private Vector2 GetForceToward(Vector2 aPoint, float afPull, float afFalloff)
        {
            float fcomDist = Vector2.Distance(Location, aPoint);
            if (fcomDist == 0.0f)
                return Vector2.Zero;

            Vector2 force = (Location - aPoint);
            force.Normalize();

            return force * (afPull - fcomDist / afFalloff);
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            Vector2 draw_loc = new Vector2(Location.X - (float)(s_MoodTextures[0].Width / 2), Location.Y - (float)(s_MoodTextures[0].Height / 2));
            Texture2D mood_texture;
            if (MyHoverMood != null)
                mood_texture = s_MoodTextures[(int)MyHoverMood];
            else
                mood_texture = s_MoodTextures[(int)MyMood];
           
            aBatch.Draw(mood_texture, draw_loc, null, Color.White, 0.0f, Vector2.Zero, 1.0f, 
                MyLook == LookDirection.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            if (_bHeld)
                aBatch.Draw(s_HeldTexture, draw_loc, Color.White);

            //for (int i = 0; i < 5; ++i)
            //    DrawUtils.DrawLine(aBatch, Location, Location + _Forces[i], _ForceColors[i]);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_MoodTextures = new Texture2D[] {
                aManager.Load<Texture2D>("happy"),
                aManager.Load<Texture2D>("sad"),
                aManager.Load<Texture2D>("angry"),
                aManager.Load<Texture2D>("people_confused_1")
            };

            s_HeldTexture = aManager.Load<Texture2D>("halo");
        }
    }
}
