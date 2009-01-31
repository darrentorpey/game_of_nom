using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connecting
{
    public class PersonFlock : GameObject
    {
        private List<Person> _People = new List<Person>();
        private float _fCurrentRadius = 0.0f;
        private Vector2 _CurrentCenterOfMass = Vector2.Zero;

        private bool _bHeld = false;

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
            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            for(int i = 0; i < _People.Count; ++i)
                _People[i].Draw(aBatch, aTime);

            DrawUtils.DrawPoint(aBatch, Location, 5, Color.Tomato);
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
