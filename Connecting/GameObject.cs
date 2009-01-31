using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Connecting
{
    public abstract class GameObject
    {
        public Vector2 Location = Vector2.Zero;

        public abstract float Radius { get; }

        public bool RadiusCheck(ref Vector2 aPoint, float afRadius)
        {
            float dist;
            Vector2.Distance(ref Location, ref aPoint, out dist);
            if (dist < (Radius + afRadius))
                return true;

            return false;
        }

        public bool CollidesWith(GameObject aOtherObj)
        {
            float dist;
            Vector2.Distance(ref Location, ref aOtherObj.Location, out dist);
            if (dist < (Radius + aOtherObj.Radius))
                return true;

            return false;
        }

        public bool InProximity(GameObject aOtherObj, float proximity)
        {
            float dist;
            Vector2.Distance(ref Location, ref aOtherObj.Location, out dist);
            if (dist < (Radius + aOtherObj.Radius + proximity))
                return true;
            return false;
        }

        public FoodSource EatingObject;

        public abstract void Hold();
        public abstract void Drop();
       
        public virtual void MoveTo(ref Vector2 aLocation)
        {
            Location = aLocation;
        }

        public abstract void Draw(SpriteBatch aBatch, GameTime aTime);
        public abstract void Update(GameTime aTime);
    }
}
