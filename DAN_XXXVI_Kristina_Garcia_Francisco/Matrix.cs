using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVI_Kristina_Garcia_Francisco
{
    class Matrix
    {
        private int[] matrixValues = new int[10000];
        private int[,] matrixArray;
        private readonly object lockMatrix = new object();
        private Random rng = new Random();

        public void CreateMatrix()
        {
            lock (lockMatrix)
            {
                matrixArray = new int[100, 100];
                Monitor.Wait(lockMatrix);

                int matrixLength = matrixValues.Length;

                for (int i = 0; i < matrixArray.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixArray.GetLength(1); j++)
                    {
                        matrixArray[i, j] = matrixValues[(i+1)*j];
                    }
                }
            }     
        }

        public void GenerateMatrixValues()
        {
            // Making sure the getValues thread only starts after the 
            // first part of createM thread is done
            Thread.Sleep(10);
            lock (lockMatrix)
            {
                int matrixLength = matrixValues.Length;
                int value = rng.Next(10, 100);
                for (int i = 0; i < matrixLength; i++)
                {
                    value = rng.Next(10, 100);
                    matrixValues[i] = value;
                }

                Monitor.Pulse(lockMatrix);
                Monitor.Wait(lockMatrix);
            }
        }

        public void CreateWorkers()
        {
            Thread createM = new Thread(CreateMatrix);
            Thread getValues = new Thread(GenerateMatrixValues);

            createM.Start();
            getValues.Start();

            createM.Join();
            getValues.Join();
        }
    }
}
