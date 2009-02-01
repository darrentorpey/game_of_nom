using System;

namespace Connecting
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameOfNom game = new GameOfNom())
            {
                game.Run();
            }
        }
    }
}

