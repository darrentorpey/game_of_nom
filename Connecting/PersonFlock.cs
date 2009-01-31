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

        public override float Radius
        {
            get { return _fCurrentRadius; }
        }

        public PersonFlock()
        {

        }

        public void AddPerson(Person aPerson)
        {
            _People.Add(aPerson);
            aPerson.InFlock = true;
        }

        public override void Hold()
        {
            // Nothing to do
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

            Vector2 center = CalculateCenterOfMass();
            for (int i = 0; i < _People.Count; ++i)
            {
                Vector2 v0 = (Location - _People[i].Location) / 100.0f;
                Vector2 v1 = (center - _People[i].Location) / 100.0f;
                Vector2 v2 = Vector2.Zero;
                for (int z = 0; z < _People.Count; ++z)
                { 
                    if(i == z)
                        continue;
                    float dist;
                    Vector2.Distance(ref _People[i].Location, ref _People[z].Location, out dist);
                    if (dist < 25.0f)
                    {
                        v2 += (_People[i].Location - _People[z].Location);
                        _People[i].Instability += 1.0f;
                    }
                }
                Vector2 speed = v0 + v1 + v2;
                speed.Normalize();
                speed *= 3.0f;
                _People[i].Location += speed;
                float fdistCoM = Vector2.Distance(center, _People[i].Location) + _People[i].Radius;
                _fCurrentRadius = Math.Max(fdistCoM, _fCurrentRadius);
                _People[i].Update(aTime);
            }
        }

        public override void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            for(int i = 0; i < _People.Count; ++i)
                _People[i].Draw(aBatch, aTime);
        }

        public Vector2 CalculateCenterOfMass()
        {
            Vector2 runningTotal = Vector2.Zero;
            for (int i = 0; i < _People.Count; ++i)
                runningTotal += _People[i].Location;

            runningTotal /= _People.Count;

            return runningTotal;
        }
    }
}
