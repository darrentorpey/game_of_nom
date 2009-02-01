using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connecting
{
    public class ExternalForce
    {
        public Vector2 Location;
        public float fForce;
        public float fFalloff;
        public int iLifeLeft;

        public ExternalForce(Vector2 aLocation, float afForce, float afFalloff, int aiLife)
        {
            Location = aLocation;
            fForce = afForce;
            fFalloff = afFalloff;
            iLifeLeft = aiLife;
        }
    }

    public class PersonFlock : GameObject
    {
        const float FOOD_PROXIMITY = 10.0f;

        public enum State
        {
            Held,
            Normal,
            Eating
        }

        private List<Person> _People = new List<Person>();
        private List<Person> _AddPeople = new List<Person>();
        private List<Person> _RemovePeople = new List<Person>();

        private Stack<FoodSource> _NearbyFoodSources = new Stack<FoodSource>();
        private List<ExternalForce> _ExternalForces = new List<ExternalForce>();

        private GameObject _CollidingObject = null;
        private float _fCurrentRadius = 0.0f;
        private Vector2 _CurrentCenterOfMass = Vector2.Zero;

        private State _eMyState = State.Normal;

        public override float Radius
        {
            get { return _fCurrentRadius; }
        }

        public Vector2 CurrentCoM
        {
            get { return _CurrentCenterOfMass; }
        }

        public Person this[int aiVal]
        {
            get { return _People[aiVal]; }
        }

        public int Count
        {
            get { return _People.Count; }
        }

        public State FlockState
        {
            get { return _eMyState; }
        }

        public PersonFlock()
        {

        }

        public void AddPerson(Person aPerson)
        {
            _AddPeople.Add(aPerson);
        }

        public void RemovePerson(Person aPerson)
        {
            _RemovePeople.Add(aPerson);
        }

        public override void Hold()
        {
            _eMyState = State.Held;
        }

        public override void Drop()
        {
            _eMyState = State.Normal;
            startEatingIfPossible();
        }

        public override void Update(GameTime aTime)
        {
            _fCurrentRadius = 0.0f;

            _CurrentCenterOfMass = CalculateCenterOfMass();
            for (int i = 0; i < _People.Count; ++i)
            {
                float fdistCoM = Vector2.Distance(_CurrentCenterOfMass, _People[i].Location) + _People[i].Radius;
                _fCurrentRadius = Math.Max(fdistCoM, _fCurrentRadius);
                _People[i].Update(aTime);

                float fdist;
                Vector2.Distance(ref _People[i].Location, ref _CurrentCenterOfMass, out fdist);
                if (fdist > 15.0f * _People.Count)
                    RemoveFromFlock(_People[i]);
            }

            for (int i = 0; i < _ExternalForces.Count; ++i)
            {
                _ExternalForces[i].iLifeLeft -= aTime.ElapsedGameTime.Milliseconds;
                if (_ExternalForces[i].iLifeLeft <= 0)
                    _ExternalForces.RemoveAt(i);
            }

            updateAllNearby();
            switch (_eMyState)
            {
                case State.Held:
                    break;
                case State.Eating:
                    if (EatingObject.Dead)
                    {
                        EatingObject = null;
                        _eMyState = State.Normal;
                    }
                    break;
                case State.Normal:
                    if (_CollidingObject is PersonFlock)
                    {
                        // Add all of us into them.
                        PersonFlock theFlock = ((PersonFlock)_CollidingObject);
                        for (int i = 0; i < _People.Count; ++i)
                        {
                            RemovePerson(_People[i]);
                            theFlock.AddPerson(_People[i]);
                        }
                    }
                    _CollidingObject = null;
                    startEatingIfPossible();
                    break;
            }

            for (int i = 0; i < _AddPeople.Count; ++i)
                AddToFlock(_AddPeople[i]);
            _AddPeople.Clear();
            for (int i = 0; i < _RemovePeople.Count; ++i)
                RemoveFromFlock(_RemovePeople[i]);
            _RemovePeople.Clear();

            if (_People.Count == 1)
                RemoveFromFlock(_People[0]);
            
            if (_People.Count == 0)
                GameObjectManager.Instance.RemoveObject(this);
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            for(int i = 0; i < _People.Count; ++i)
                _People[i].Draw(aBatch, aTime);

            DrawUtils.DrawPoint(aBatch, Location, 5, Color.Tomato);
        }

        public void AddExtenralForce(ExternalForce aForce)
        {
            _ExternalForces.Add(aForce);
        }

        public Vector2 GetExternalForces(Person aPerson)
        {
            Vector2 force = Vector2.Zero;
            for (int i = 0; i < _ExternalForces.Count; ++i)
            {
                float fdist = Vector2.Distance(aPerson.Location, _ExternalForces[i].Location);
                Vector2 direction = aPerson.Location - _ExternalForces[i].Location;
                direction.Normalize();

                float ffactor = _ExternalForces[i].fForce - (fdist * _ExternalForces[i].fFalloff);
                if(ffactor > 0)
                    force += direction * ffactor;
            }

            return force;
        }

        public Person.Mood GetMood()
        {
            Person.Mood retMood = Person.Mood.Happy;
            switch (_eMyState)
            {
                case State.Held:
                    if (_NearbyFoodSources.Count > 0)
                    {
                        retMood = Person.Mood.Excited;
                    }
                    break;
                case State.Eating:
                    retMood = Person.Mood.Eating;
                    break;
            }

            return retMood;
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

        private void startEatingIfPossible()
        {
            if (_NearbyFoodSources.Count > 0)
            {
                _NearbyFoodSources.First().BeingEaten = true;
                EatingObject = _NearbyFoodSources.First();
                _eMyState = State.Eating;
            }
        }

        private void AddToFlock(Person aPerson)
        {
            GameObjectManager.Instance.RemoveObject(aPerson);
            _People.Add(aPerson);
            aPerson.AddedToFlock(this);
        }

        private void RemoveFromFlock(Person aPerson)
        {
            _People.Remove(aPerson);
            GameObjectManager.Instance.AddObject(aPerson);
            aPerson.RemovedFromFlock();
        }

        private Vector2 CalculateCenterOfMass()
        {
            Vector2 runningTotal = Vector2.Zero;
            for (int i = 0; i < _People.Count; ++i)
                runningTotal += _People[i].Location;

            runningTotal /= _People.Count;

            return runningTotal;
        }
    }
}
