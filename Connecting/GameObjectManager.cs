using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Connecting
{
    public class GameObjectManager
    {
        private static GameObjectManager s_Instance = new GameObjectManager();

        public Person[] _Persons;

        public static GameObjectManager Instance
        {
            get { return s_Instance; }
        }
    }
}
