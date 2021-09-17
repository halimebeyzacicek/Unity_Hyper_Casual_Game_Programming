using System;

namespace kurs_1
{
    class Program
    {
        static void Main(string[] args)
        {
            int p1Score = 25;
            int p2Score = 50;
            int p3Score = 75;
            int[] pScore = { 25, 50, 75 };
            //               0    1   2

            int[] pScore2 = new int[3];
            pScore2[0] = 25;
            //pScore2[1] = 50;
            pScore2[2] = 75;

            Console.WriteLine("Birinci oyuncunun skoru : " + pScore2[0]);
            Console.WriteLine("İkinci oyuncunun skoru : " + pScore2[1]); //değeri değiştirmedik. yani sıfır görüyoruz.

            Console.ReadKey();
                
        }
    }
}
