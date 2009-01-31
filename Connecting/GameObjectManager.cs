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

        private List<GameObject> _Objects = new List<GameObject>();
        private List<GameObject> _AddObject = new List<GameObject>();
        private List<GameObject> _RemoveObject = new List<GameObject>();

        public GameObject this[int aiIndex]
        {
            get { return _Objects[aiIndex]; }
        }
        public int Count
        {
            get { return _Objects.Count; }
        }

        public void AddObject(GameObject aObject)
        {
            _AddObject.Add(aObject);
        }

        public void RemoveObject(GameObject aObject)
        {
            _RemoveObject.Add(aObject);
        }

        public void Clear()
        {
            _Objects.Clear();
            _AddObject.Clear();
            _RemoveObject.Clear();
        }

        public static GameObjectManager Instance
        {
            get { return s_Instance; }
        }

        public void Update(GameTime aTime)
        {
            for (int i = 0; i < _Objects.Count; ++i)
                _Objects[i].Update(aTime);

            for (int i = 0; i < _AddObject.Count; ++i)
                _Objects.Add(_AddObject[i]);
            _AddObject.Clear();
            for (int i = 0; i < _RemoveObject.Count; ++i)
                _Objects.Remove(_RemoveObject[i]);
            _RemoveObject.Clear();
        }

        public void Draw(SpriteBatch aBatch, GameTime aTime)
        {
            for (int i = 0; i < _Objects.Count; ++i)
                _Objects[i].Draw(aBatch, aTime);
        }
    }
}
