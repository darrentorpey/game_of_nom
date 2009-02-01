using System;
using System.Collections.Generic;
using System.Text;

namespace Connecting
{
    class RandomInstance
    {
        private static Random s_Instance = new Random();

        public static Random Instance { get { return s_Instance; } }
    }
}
