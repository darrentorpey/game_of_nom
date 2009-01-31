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
        private List<Person> _People = new List<Person>();
        private List<Person> _AddPeople = new List<Person>();
        private List<Person> _RemovePeople = new List<Person>();

        private List<ExternalForce> _ExternalForces = new List<ExternalForce>();
        private float _fCurrentRadius = 0.0f;
        private Vector2 _CurrentCenterOfMass = Vector2.Zero;

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
            
        }

        public override void Drop()
        {
            
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
