using System;

namespace On_the_Line
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (OnTheLine game = new OnTheLine())
            {
                game.Run();
            }
        }
    }
#endif
}

