using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DAN_XXXVI_Kristina_Garcia_Francisco
{
    class Matrix
    {
        private int[] matrixValues = new int[10000];
        private int[,] matrixArray;
        private readonly object lockMatrix = new object();
        private Random rng = new Random();
        private readonly string matrixFile = "matrix.txt";
        private bool fileCreated = false;

        public void CreateMatrix()
        {
            lock (lockMatrix)
            {
                matrixArray = new int[100, 100];

                // Inform the thread that the object is available.
                Monitor.Pulse(lockMatrix);
                Monitor.Wait(lockMatrix);

                int matrixLength = matrixValues.Length;

                for (int i = 0; i < matrixArray.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixArray.GetLength(1); j++)
                    {
                        matrixArray[i, j] = matrixValues[(i + 1) * j];
                    }
                }
            }     
        }

        public void GenerateMatrixValues()
        {
            lock (lockMatrix)
            {
                // If the matrix is empty lock this thread.
                if (matrixArray == null)
                {
                    Monitor.Wait(lockMatrix);
                }

                int matrixLength = matrixValues.Length;
                int value = rng.Next(10, 100);
                for (int i = 0; i < matrixLength; i++)
                {
                    value = rng.Next(10, 100);
                    matrixValues[i] = value;
                }

                Monitor.Pulse(lockMatrix);
            }
        }

        public void OddNumberToFile()
        {
            lock (lockMatrix)
            {
                int countOdd = 0;
                List<int> allOddNumbers = new List<int>();

                foreach (var item in matrixArray)
                {
                    if (item % 2 == 1)
                    {
                        countOdd++;
                        allOddNumbers.Add(item);
                    }
                }

                int totalOddNumbers = allOddNumbers.Count;
                int[] matrixOddValues = new int[totalOddNumbers];

                for (int i = 0; i < totalOddNumbers; i++)
                {
                    matrixOddValues[i] = allOddNumbers[i];
                }

                using (StreamWriter streamWriter = new StreamWriter(matrixFile))
                {
                    for (int i = 0; i < totalOddNumbers; i++)
                    {
                        streamWriter.WriteLine(matrixOddValues[i] + " ");
                    }
                }

                fileCreated = true;

                // If the thread tried to run first but failed due to the lock, now it can run again
                Monitor.Pulse(lockMatrix);
            }
        }

        public void ReadFromFile()
        {
            lock (lockMatrix)
            {
                // Dont run this thread until the file is created
                if (fileCreated == false)
                {
                    Monitor.Wait(lockMatrix);
                }

                using (StreamReader streamReader = File.OpenText(matrixFile))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Console.Write(line);
                    }
                }
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

            Thread oddNumber = new Thread(OddNumberToFile);
            Thread readFile = new Thread(ReadFromFile);

            oddNumber.Start();
            readFile.Start();
        }
    }
}
