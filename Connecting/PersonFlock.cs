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

        public List<Person> People
        {
            get { return _People; }
        }

        public PersonFlock()
        {

        }

        public void AddPerson(Person aPerson)
        {
            _People.Add(aPerson);
            aPerson.ParentFlock = this;
        }

        public override void Hold()
        {
            
        }

        public override void Drop()
        {
            
        }

        public override void MoveTo(ref Vector2 aLocation)
        {
            Location = aLocation;
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
                {
                    _People[i].ParentFlock = null;
                    GameObjectManager.Instance._Objects.Add(_People[i]);
                    _People.RemoveAt(i);
                }
            }

            for (int i = 0; i < _ExternalForces.Count; ++i)
            {
                _ExternalForces[i].iLifeLeft -= aTime.ElapsedGameTime.Milliseconds;
                if (_ExternalForces[i].iLifeLeft <= 0)
                    _ExternalForces.RemoveAt(i);
            }

            if (_People.Count == 1)
            {
                GameObjectManager.Instance._Objects.Remove(this);

                People[0].ParentFlock = null;
                GameObjectManager.Instance._Objects.Add(_People[0]);
                _People.RemoveAt(0);

            }
            else if (_People.Count == 0)
                GameObjectManager.Instance._Objects.Remove(this);
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
                Console.WriteLine(ffactor);
                if(ffactor > 0)
                    force += direction * ffactor;
            }

            return force;
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
