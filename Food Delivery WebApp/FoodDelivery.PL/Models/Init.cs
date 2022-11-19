namespace FoodDelivery.PL.Models
{
    using System;

    public class Init
    {
        private static string Path { get; } = AppDomain.CurrentDomain.BaseDirectory + "/StoragePass.txt";

        public static string WriteEmailPass(string email, string password)
        {
            using (var sw = new System.IO.StreamWriter(Path, true))
            {
                sw.WriteLine($"{email} - {password}");
                return "Successfuly!";
            }
        }
    }
}