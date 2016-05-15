using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Project_Airline_info
{
    interface IPriceable
    {
        void AddPrices();
        void EditPrices();
        void ShowPricesForFlight();
    }
    class PricesDatabase : IPriceable
    {
        SqlConnection sqlConnectionPrice;
        internal PricesDatabase()
        {
            sqlConnectionPrice = new SqlConnection(ConfigurationManager.ConnectionStrings["AirportConnection"].ConnectionString);

            ConsoleKeyInfo KeyReader;
            do
            {
                Console.WriteLine("1.Add prices for flight\n2.Edit prices\n3.Show prices for flight\n4.Buy Ticket");
                KeyReader = Console.ReadKey();
                Console.WriteLine();
                switch (KeyReader.KeyChar)
                {
                    case '1':
                        AddPrices();
                        break;
                    case '2':
                        EditPrices();
                        break;
                    case '3':
                        ShowPricesForFlight();
                        break;
                    case '4':
                        BuyTicket();
                        break;
                    default:
                        break;
                }
                Console.WriteLine("If you want to exit press escape otherwise press anykey");
                KeyReader = Console.ReadKey();
                Console.WriteLine();
            }
            while (KeyReader.Key != ConsoleKey.Escape);
        }
        public void AddPrices()
        {
            Console.Write("Enter Flight=");
            string tempNameOfFlight = Console.ReadLine();
            sqlConnectionPrice.Open();
            SqlCommand CheckFlightNumberInPrices = new SqlCommand(@"SELECT FlightNumber, ID, 
            City_of_arrival, City_of_departure FROM Prices WHERE ID LIKE @NameOfFLight 
            OR FlightNumber LIKE @NameOfFlight", sqlConnectionPrice);
            CheckFlightNumberInPrices.Parameters.AddWithValue("@NameOfFlight", tempNameOfFlight);

            SqlDataReader GetDataForFlightInPrices = CheckFlightNumberInPrices.ExecuteReader();
            bool isPriceHasFlight = GetDataForFlightInPrices.HasRows;
            GetDataForFlightInPrices.Close();
            SqlCommand CheckFlightNumberInFlights = new SqlCommand(@"SELECT Flight_Number,ID,
            City_of_arrival,City_of_departure,Arrival,Departure FROM Flights WHERE ID LIKE @NameOfFLight 
            OR Flight_Number LIKE @NameOfFlight", sqlConnectionPrice);
            CheckFlightNumberInFlights.Parameters.AddWithValue("@NameOfFlight", tempNameOfFlight);
            SqlDataReader GetDataForFlightInFlights = CheckFlightNumberInFlights.ExecuteReader();
            SqlCommand WritePriceData = new SqlCommand(@"INSERT INTO Prices(City_of_departure,
            City_of_arrival,FlightNumber,Price_For_Econom,Price_For_Business,Price_For_First,Arrival,Departure,ID) 
            VALUES (@City_of_departure,@City_of_arrival,@FlightNumber,@Price_For_Econom,@Price_For_Business,
            @Price_For_First,@Arrival,@Departure,@ID)", sqlConnectionPrice);
            #region Write data to prices table according to if statement
            if (GetDataForFlightInFlights.HasRows && !isPriceHasFlight)
            {

                while (GetDataForFlightInFlights.Read())
                {
                    WritePriceData.Parameters.AddWithValue("@FlightNumber",
                        GetDataForFlightInFlights.GetValue(0).ToString());
                    WritePriceData.Parameters.AddWithValue("@ID",
                        (int)GetDataForFlightInFlights.GetValue(1));
                    WritePriceData.Parameters.AddWithValue("@City_of_arrival",
                        GetDataForFlightInFlights.GetValue(2).ToString());
                    WritePriceData.Parameters.AddWithValue("@City_of_departure",
                        GetDataForFlightInFlights.GetValue(3).ToString());
                    WritePriceData.Parameters.AddWithValue("@Arrival",
                        GetDataForFlightInFlights.GetValue(4).ToString());
                    WritePriceData.Parameters.AddWithValue("@Departure",
                        GetDataForFlightInFlights.GetValue(5).ToString());

                }
                GetDataForFlightInFlights.Close();

                SqlCommand ReadColumnsName = new SqlCommand();
                ReadColumnsName.CommandText = "SELECT * FROM Prices";
                ReadColumnsName.Connection = sqlConnectionPrice;
                SqlDataReader GetName = ReadColumnsName.ExecuteReader();
                List<string> Columnnames = Enumerable.Range(0, GetName.FieldCount).
                    Select(GetName.GetName).ToList();
                for (int i = 3; i < Columnnames.Count - 3; i++)
                {
                    Console.Write(Columnnames[i] + "=");
                    WritePriceData.Parameters.AddWithValue(Columnnames[i], Console.ReadLine());
                }
                GetName.Close();
                WritePriceData.ExecuteNonQuery();
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist");
            }
            #endregion
            sqlConnectionPrice.Close();
        }
        public void EditPrices()
        {
            string selectedFlight;
            sqlConnectionPrice.Open();
            SqlCommand CheckFlightNumber = new SqlCommand(@"SELECT City_of_departure,City_of_arrival,
            FlightNumber,Price_For_Econom,Price_For_Business,Price_For_First,ID FROM Prices 
            WHERE ID LIKE @NameOfFLight OR FlightNumber LIKE @NameOfFlight", sqlConnectionPrice);
            Console.Write("Enter Flight=");
            selectedFlight = Console.ReadLine();
            CheckFlightNumber.Parameters.AddWithValue("@NameOfFlight", selectedFlight);
            SqlDataReader GetDataForFlight = CheckFlightNumber.ExecuteReader();
            SqlCommand WritePriceData = new SqlCommand(@"UPDATE Prices SET 
            Price_For_Econom=@Price_For_Econom,
            Price_For_Business= @Price_For_Business,
            Price_For_First=@Price_For_First 
            WHERE ID LIKE @SelectedID OR FlightNumber LIKE  @SelectedID", sqlConnectionPrice);
            if (GetDataForFlight.HasRows)
            {
                GetDataForFlight.Close();
                SqlCommand ReadColumnsName = new SqlCommand("SELECT * FROM Prices", sqlConnectionPrice);
                SqlDataReader GetName = ReadColumnsName.ExecuteReader();
                List<string> Columnnames = Enumerable.Range(0, GetName.FieldCount).Select(GetName.GetName).ToList();
                for (int i = 3; i < Columnnames.Count - 1; i++)
                {
                    Console.Write(Columnnames[i] + "=");
                    WritePriceData.Parameters.AddWithValue(Columnnames[i], Console.ReadLine());
                }
                GetName.Close();
                WritePriceData.Parameters.AddWithValue("@SelectedID", selectedFlight);
                try
                {
                    WritePriceData.ExecuteNonQuery();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist");
            }
            sqlConnectionPrice.Close();
        }
        public void ShowPricesForFlight()
        {
            sqlConnectionPrice.Open();
            SqlCommand ShowFlightByFilter = new SqlCommand(@"SELECT * FROM Prices WHERE ID LIKE @SelectedID OR FlightNumber LIKE @SelectedID", sqlConnectionPrice);
            Console.Write("Enter Flight=");
            ShowFlightByFilter.Parameters.AddWithValue("@SelectedID", Console.ReadLine());
            SqlDataReader ReadFlightFromDataBase = ShowFlightByFilter.ExecuteReader();
            StringBuilder writeIntoFile = new StringBuilder();
            if (ReadFlightFromDataBase.HasRows)
            {
                while (ReadFlightFromDataBase.Read())
                {
                    for (int i = 0; i < ReadFlightFromDataBase.FieldCount; i++)
                    {
                        writeIntoFile.AppendLine(ReadFlightFromDataBase.GetName(i) +
                            ":" + ReadFlightFromDataBase.GetValue(i));
                    }
                }
                Console.WriteLine(writeIntoFile);
                File.WriteAllText("PriceByFilter.txt", writeIntoFile.ToString());
                ReadFlightFromDataBase.Close();
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist");
            }
            sqlConnectionPrice.Close();
        }
        public void BuyTicket()
        {
            sqlConnectionPrice.Open();
            SqlCommand findPlane = new SqlCommand(@"SELECT * FROM Prices 
            WHERE City_of_departure LIKE @City_of_departure AND 
            City_of_arrival LIKE @City_of_arrival OR
            Arrival LIKE @Arrival OR
            Departure LIKE @Departure", sqlConnectionPrice);
            Console.WriteLine("Enter Departure Date");
            findPlane.Parameters.AddWithValue("@Departure", PassengerDataBase.AddDateOnly());
            Console.WriteLine("Enter Arrival Date=");
            findPlane.Parameters.AddWithValue("@Arrival", PassengerDataBase.AddDateOnly());
            Console.Write("Enter city of departure=");
            findPlane.Parameters.AddWithValue("@City_of_departure", Console.ReadLine());
            Console.Write("Enter city of arrival=");
            findPlane.Parameters.AddWithValue("@City_of_arrival", Console.ReadLine());
            SqlDataReader getplane = findPlane.ExecuteReader();
            Console.WriteLine();
            #region Get Buyed Plane
            if (getplane.HasRows)
            {
               
                while (getplane.Read())
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine(getplane.GetName(i) + ":" + getplane.GetValue(i));
                    }
                    for (int i = 3; i < 6; i++)
                    {
                        Console.WriteLine(getplane.GetName(i) + ":" + getplane.GetValue(i)+"$");
                    }
                    for (int i = 6; i < getplane.FieldCount-1; i++)
                    {
                        Console.WriteLine(getplane.GetName(i) + ":" + getplane.GetValue(i));
                    }
                    Console.WriteLine("_____________________");
                }
            }
            else
            {
                Console.WriteLine("There is no plane on your date,sorry");
            }
            #endregion
            sqlConnectionPrice.Close();
        }
    }
}
