using System;

namespace kurs_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();
            ///////////////////////////
            int health;
            health = 100;
            health = health - 10;
            //Console.WriteLine(health);
            /////////////////////////////////
            bool dead = health <= 0;  //koşul sağlanırsa true değerine sahip olacak
            bool inDanger = health <= 50;//karakter tehlikede mi?
            if(!dead)
            {
                if (inDanger)
                {
                    Console.WriteLine("Karakteriniz Tehlikede!");
                }
                else
                {
                    Console.WriteLine("Karakteriniz Güvende!");
                }
            }
            else
            {
                Console.WriteLine("Oyunu kaybettiniz!");
            }
            //if (health > 50)
            //{
            //    Console.WriteLine("Karakteriniz Güvende!");
            //}
            //else if (health > 0)
            //{
            //    Console.WriteLine("Karakteriniz Tehlikede!");
            //}
            //else
            //{
            //    Console.WriteLine("Oyunu kaybettiniz!");
            //}
            /////////////////////////////////
            float height = 1.60f;
            string name = "Halime";
            Console.WriteLine(name + " karakterinin boyu:" + height);
            Console.WriteLine(name + " karakterinin canı:" + health);
            Console.ReadKey();
            ///////////////////////////
            










        }
    }
}
