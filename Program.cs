using System;
namespace Project_Airline_info
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Menu menu = new Menu();
        }
    }
    class Menu
    {
        internal Menu()
        {
            FlightsDatabase flights = new FlightsDatabase();
            ConsoleKeyInfo Key_Reader;
            do
            {
                Console.WriteLine("1.Add Flight\n2.Remove Flight\n3.Show all flights\n4.Show Flight by filter\n5.Show and edit passangers list\n6.Edit Prices for flights");
                Key_Reader = Console.ReadKey();
                Console.WriteLine();
                switch (Key_Reader.KeyChar)
                {
                    case '1':
                        flights.Add();
                        break;
                    case '2':
                        flights.Remove();
                        break;
                    case '3':
                        flights.ShowAll();
                        break;
                    case '4':
                        flights.ShowByFilter();
                        break;
                    case '5':
                        flights.ActionPassengers();
                        break;
                    case '6':
                        PricesDatabase price_database=new PricesDatabase();
                        break;
                    default:
                        Console.WriteLine("You entered wrong number");
                        break;
                }
                Console.WriteLine("If you want to exit press escape otherwise press anykey");
                Key_Reader = Console.ReadKey();
                Console.WriteLine();
            }
            while (Key_Reader.Key != ConsoleKey.Escape);
          
        }
    }
    
}
