using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Connecting
{
    public class PersonFlock
    {
        private List<Person> _People = new List<Person>();

        public Vector2 TargetLocation { get; set; }

        public PersonFlock()
        {

        }

        public void AddPerson(Person aPerson)
        {
            _People.Add(aPerson);
        }

        public void Update(GameTime aTime)
        {
            Vector2 center = CalculateCenterOfMass();
            for (int i = 0; i < _People.Count; ++i)
            {
                Vector2 v0 = (TargetLocation - _People[i].Location) / 100.0f;
                Vector2 v1 = (center - _People[i].Location) / 100.0f;
                Vector2 v2 = Vector2.Zero;
                for (int z = 0; z < _People.Count; ++z)
                { 
                    if(i == z)
                        continue;
                    float dist;
                    Vector2.Distance(ref _People[i].Location, ref _People[z].Location, out dist);
                    if (dist < 20.0f)
                    {
                        v2 += (_People[i].Location - _People[z].Location);
                    }
                }
                _People[i].Location += v0 + v1 + v2;
            }
        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
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
