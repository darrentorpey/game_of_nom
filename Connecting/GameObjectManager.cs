using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Connecting
{
    public class GameObjectManager
    {
        private static GameObjectManager s_Instance = new GameObjectManager();

        public List<GameObject> _Objects;

        public static GameObjectManager Instance
        {
            get { return s_Instance; }
        }

        public void Update(GameTime aTime)
        {
            for (int i = 0; i < _Objects.Count; ++i)
                _Objects[i].Update(aTime);
        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            for (int i = 0; i < _Objects.Count; ++i)
                _Objects[i].Draw(aBatch, aTime);
        }
    }
}
