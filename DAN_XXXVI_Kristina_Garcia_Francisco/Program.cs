using System;

namespace DAN_XXXVI_Kristina_Garcia_Francisco
{
    /// <summary>
    /// The main program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">main arguments</param>
        static void Main(string[] args)
        {
            Matrix matrix = new Matrix();

            matrix.CreateWorkers();

            Console.ReadKey();
        }
    }
}
