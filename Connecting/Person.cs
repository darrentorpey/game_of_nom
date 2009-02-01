using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

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
            Confused = 3,
            Excited = 4,
            Eating = 5,
            Surprise = 6,
            Starving = 7,
            Hungry = 8,
            Dead = 9,

            Count = 10
        }

        public enum State
        {
            Alone,
            Flocking,
            Held,
            Eating,
            Dead
        }

        public enum HungerLevel
        {
            Starving = 400,
            Dead = 1000
        }

        public enum AloneState
        {
            StandingStill,
            Looking,
            Wandering
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
        
        private State _eMyState = State.Alone;
        private AloneState _eMyAloneState = AloneState.StandingStill;
        
        private GameObject _CollidingObject = null;
        private Stack<FoodSource> _NearbyFoodSources = new Stack<FoodSource>();
        private Vector2 _Velocity;
        private Rectangle _Bounds;
        private int _iNextThink = 0;
        private int _iNextLook = 0;
        private float _fSensitivity = 0.0f;
        private Vector2 _WalkVelocity = Vector2.Zero;

        private Vector2[] _Forces = new Vector2[5];      

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

        public void AddedToFlock(PersonFlock aFlock)
        {
            ParentFlock = aFlock;
            _eMyState = State.Flocking;
        }

        public void RemovedFromFlock()
        {
            ParentFlock = null;
            _eMyState = State.Alone;
        }

        public override void Hold()
        {
            _eMyState = State.Held;
        }

        public override void Drop()
        {
            _eMyState = State.Alone;

            if (_CollidingObject != null)
            {
                GameObjectManager manager = GameObjectManager.Instance;
                if (_CollidingObject is PersonFlock)
                {
                    manager.RemoveObject(this);
                    ((PersonFlock)_CollidingObject).AddPerson(this);
                }
                else if (_CollidingObject is Person)
                {
                    manager.RemoveObject(this);
                    manager.RemoveObject(_CollidingObject);

                    PersonFlock flock = new PersonFlock();
                    flock.AddPerson(this);
                    flock.AddPerson((Person)_CollidingObject);
                    flock.Location = this.Location;
                    manager.AddObject(flock);
                }

                _CollidingObject = null;
            }

            if (EatingObject != null && !EatingObject.InProximity(this, FOOD_PROXIMITY))
            {
                EatingObject.BeingEaten = false;
                EatingObject = null;
            }

            startEatingIfPossible();
        }

        private void startEatingIfPossible()
        {
            if (_NearbyFoodSources.Count > 0)
            {
                _NearbyFoodSources.First().BeingEaten = true;
                EatingObject = _NearbyFoodSources.First();
                _eMyState = State.Eating;
            }
        }

        public override void Update(GameTime aTime)
        {
            updateHunger();
            updateAllNearby();

            if (ParentFlock != null)
                _eMyState = State.Flocking;

            if (_eMyState == State.Eating && EatingObject.Dead)
            {
                EatingObject = null;
                _eMyState = State.Alone;
                _eMyAloneState = AloneState.StandingStill;
            }

            switch(_eMyState)
            {
                case State.Dead:
                    MyMood = getMood();
                    break;
                case State.Eating:
                    MyMood = getMood();
                    break;
                case State.Flocking:
                    AccumulateForces();
                    Location = Location + (_Velocity * (float)aTime.ElapsedGameTime.TotalSeconds);

                    Vector2 moving;
                    switch (ParentFlock.FlockState)
                    {
                        case PersonFlock.State.Held:
                            // Look in the direction we're being dragged.
                            moving = Location - ParentFlock.Location;
                            MyLook = moving.X < 0 ? LookDirection.Right : LookDirection.Left;
                            break;
                        case PersonFlock.State.Eating:
                            // Look at what we're eating
                            moving = Location - ParentFlock.EatingObject.Location;
                            MyLook = moving.X < 0 ? LookDirection.Right : LookDirection.Left;
                            break;
                    }

                    // When you're flocking, you're sensitive to other people.
                    // Use the avoidance force as a basis
                    _fSensitivity += _Forces[2].Length() * .5f;
                    
                    // Reduce sensitivity depending on the number of people in my flock    
                    float dec = (1.0f / (float)ParentFlock.Count) * 200.0f;
                    _fSensitivity -= dec;
                    if (_fSensitivity < 0)
                        _fSensitivity = 0.0f;

                    Mood myOldMood = MyMood;
                    MyMood = getMood();
                    
                    if(myOldMood != MyMood && MyMood == Mood.Angry)
                        SoundState.Instance.PlayAngrySound(this, aTime);

                    if (_fSensitivity > 400.0f)
                    {
                        // EXPLODE!
                        ParentFlock.AddExtenralForce(new ExternalForce(this.Location, 600.0f, 3.0f, 120));
                        ParentFlock.RemovePerson(this);
                        SoundState.Instance.PlayExplosionSound(aTime);

                        MyMood = Mood.Sad;
                        _eMyState = State.Alone;
                    }
                    break;
                case State.Alone:
                    _fSensitivity = 0.0f;
                    AccumulateForces();
                    if (_iNextThink <= 0)
                    {
                        _iNextThink = c_iThinkTime + RandomInstance.Instance.Next(-c_iRandDelay, c_iRandDelay);
                        _eMyAloneState = (AloneState)RandomInstance.Instance.Next(0, 3);
                    }
                    else
                        _iNextThink -= aTime.ElapsedGameTime.Milliseconds;

                    switch (_eMyAloneState)
                    {
                        case AloneState.StandingStill:
                            _WalkVelocity = Vector2.Zero;
                            MyMood = getMood();
                            break;
                        case AloneState.Looking:
                            if (_iNextLook <= 0)
                            {
                                _iNextLook = c_iLookTime + RandomInstance.Instance.Next(-c_iLookDelay, c_iLookDelay);
                                MyLook = MyLook == LookDirection.Left ? LookDirection.Right : LookDirection.Left;
                            }
                            else
                                _iNextLook -= aTime.ElapsedGameTime.Milliseconds;
                            _WalkVelocity = Vector2.Zero;
                            MyMood = getMood();
                            break;
                        case AloneState.Wandering:
                            if (_WalkVelocity == Vector2.Zero)
                            {
                                _WalkVelocity = new Vector2(RandomInstance.Instance.Next(-100, 100) / 100.0f,
                                    RandomInstance.Instance.Next(-100, 100) / 100.0f);
                                _WalkVelocity.Normalize();
                                if (_WalkVelocity.X < 0)
                                    MyLook = LookDirection.Left;
                                else
                                    MyLook = LookDirection.Right;

                                _WalkVelocity *= c_fSpeed;
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

                            startEatingIfPossible();

                            MyMood = getMood();
                            break;
                    }
                    break;
                case State.Held:
                    MyHoverMood = getMood();
                    if (_NearbyFoodSources.Count > 0)
                    {
                        // Look at what you want to eat.
                        Vector2 lookDirection = Location - _NearbyFoodSources.Peek().Location;
                        MyLook = lookDirection.X < 0 ? LookDirection.Right : LookDirection.Left;
                    }
                    break;
            }
        }

        private void updateAllNearby()
        {
            _NearbyFoodSources.Clear();
            _CollidingObject = null;
            
            GameObjectManager manager = GameObjectManager.Instance;
            for (int i = 0; i < manager.Count; ++i)
            {
                GameObject currObj = manager[i];
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
                }
            }
        }

        private void updateHunger()
        {
            _Hunger += 1;

            if (_eMyState == State.Eating ||
                (_eMyState == State.Flocking && ParentFlock.FlockState == PersonFlock.State.Eating))
            {
                _Hunger -= 2;
            }

            if (_Hunger > (int)HungerLevel.Dead)
            {
                // R.I.P.
                _eMyState = State.Dead;
            }
        }

        private Mood getMood()
        {
            Mood retMood = Mood.Sad;

            GameObjectManager manager = GameObjectManager.Instance;

            switch (_eMyState)
            {
                case State.Dead:
                    retMood = Mood.Dead;
                    break;
                case State.Alone:
                    if (_Hunger > (int)HungerLevel.Starving) {
                        // Hungry!
                        retMood = Mood.Starving;
                    }
                    else if(_eMyAloneState == AloneState.Looking || _eMyAloneState == AloneState.Wandering) {
                        retMood = Mood.Confused;
                    }
                    else
                        retMood = Mood.Sad;
                    break;
                case State.Flocking:
                    retMood = MyMood;
                    if (_Hunger > (int)HungerLevel.Starving)
                        retMood = Mood.Starving;
                    else
                    {
                        switch (MyMood)
                        {
                            // Mostly, I want the flock's mood
                            case Mood.Excited:
                            case Mood.Happy:
                            case Mood.Confused:
                            case Mood.Eating:
                                retMood = ParentFlock.GetMood();
                                if (_fSensitivity > 50.0f)
                                    retMood = Mood.Sad;
                                break;
                            // But when I'm sad, hungry or angry, I want my mood
                            case Mood.Hungry:
                            case Mood.Starving:
                            case Mood.Sad:
                                if (_fSensitivity < 20.0f)
                                    retMood = Mood.Happy;
                                else if (_fSensitivity > 120.0f)
                                    retMood = Mood.Angry;
                                break;
                            case Mood.Angry:
                                if (_fSensitivity < 40.0f)
                                    retMood = Mood.Sad;
                                break;
                        }
                    }
                    break;

                case State.Eating:
                    retMood = Mood.Eating;
                    break;

                case State.Held:
                    // Look to see if we need to indicate that droping this Person will change their mood
                    if (_CollidingObject != null && (_CollidingObject is Person || _CollidingObject is PersonFlock)) {
                        // If we're colliding with a person, we're happy (we're very social!)
                        retMood = Mood.Excited;
                    }
                    else if(_NearbyFoodSources.Count != 0) {
                        retMood = Mood.Excited;
                    }
                    else
                        goto case State.Alone;
                    break;
            }

            return retMood;
        }

        private void AccumulateForces()
        {
            Vector2 forces = Vector2.Zero;

            for (int i = 0; i < 5; ++i)
                _Forces[i] = Vector2.Zero;

            if (ParentFlock != null)
            {
                _Forces[4] = ParentFlock.GetExternalForces(this);

                // Always move toward CoM.  Pull harder if farther away.
                _Forces[0] = GetForceToward(ParentFlock.CurrentCoM, .2f, Radius * ParentFlock.Count);
                
                // Always move toward Target location
                _Forces[1] = GetForceToward(ParentFlock.Location, .3f, 2.5f);

                // Move away from all other boids
                Vector2 avoidanceForce = Vector2.Zero;
                Vector2 matchingForce = Vector2.Zero;
                for (int i = 0; i < ParentFlock.Count; ++i)
                {
                    if (ParentFlock[i] == this)
                        continue;

                    float dist;
                    Vector2.Distance(ref ParentFlock[i].Location, ref this.Location, out dist);
                    if (dist < 40.0f)
                    {
                        Vector2 force = (this.Location - ParentFlock[i].Location);
                        avoidanceForce += force;
                    }

                    // Match velocity with other boids
                    matchingForce += ParentFlock[i].Velocity;
                }
                _Forces[2] = avoidanceForce;

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

            if (_eMyState == State.Held)
            {
                mood_texture = s_MoodTextures[(int)MyHoverMood];
            }
            else
            {
                mood_texture = s_MoodTextures[(int)MyMood];
            }

            aBatch.Draw(mood_texture, draw_loc, null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                MyLook == LookDirection.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0f);

            if (_eMyState == State.Held)
            {
                aBatch.Draw(s_HeldTexture, draw_loc, Color.White);
            }

            //for (int i = 0; i < 5; ++i)
            //    DrawUtils.DrawLine(aBatch, Location, Location + _Forces[i], _ForceColors[i]);
        }

        public static void LoadContent(ContentManager aManager)
        {
            s_MoodTextures = new Texture2D[(int)Mood.Count];
            for (int i = 0; i < (int)Mood.Count; ++i)
                s_MoodTextures[i] = aManager.Load<Texture2D>("people_" + ((Mood)i).ToString() + "_1");

            s_HeldTexture = aManager.Load<Texture2D>("selection_halo_1");
        }
    }
}
