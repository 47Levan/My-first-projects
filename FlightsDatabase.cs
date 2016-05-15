using System;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
namespace Project_Airline_info
{
    interface IFlightable
    {
        void Add();
        void Remove();
        void ShowAll();
        void ShowByFilter();
    }
    class FlightsDatabase : IFlightable
    {
        SqlConnection sqlcon;
        internal FlightsDatabase()
        {
            sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["AirportConnection"]
                .ConnectionString);
        }

        internal static DateTime EnterDataWithTime()
        {
            int year;
            int day;
            int month;
            int hour;
            int minute;
            do
            {
                try
                {
                    do
                    {
                        Console.Write("Enter Year=");
                        year = int.Parse(Console.ReadLine());
                        try
                        {
                            if (year < DateTime.Now.Year - 5)
                            {
                                throw new DateYearCheck();
                            }
                        }
                        catch (DateYearCheck)
                        {
                            Console.WriteLine("You entered too small year try again");
                        }
                    }
                    while (year < DateTime.Now.Year - 5);
                    Console.Write("Enter Month=");
                    month = int.Parse(Console.ReadLine());
                    Console.Write("Enter day=");
                    day = int.Parse(Console.ReadLine());
                    Console.Write("Enter Hour=");
                    hour = int.Parse(Console.ReadLine());
                    Console.Write("Enter Minute=");
                    minute = int.Parse(Console.ReadLine());
                    return new DateTime(year, month, day, hour, minute, 0);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You entered wrong type value try again");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("You entered more value that variable exprect");
                }
            } while (true);
        }

        public void Add()
        {
            sqlcon.Open();
            Flight flight = new Flight();
            SqlCommand addflight = new SqlCommand();
            addflight.Connection = sqlcon;
            Console.WriteLine("Enter Departure Date");
            addflight.Parameters.AddWithValue("@Departure", EnterDataWithTime());
            Console.WriteLine("Enter Arrival Date");
            addflight.Parameters.AddWithValue("@Arrival", EnterDataWithTime());
            Console.Write("Enter Flight number=");
            addflight.Parameters.AddWithValue("@Flight_Number", Console.ReadLine());
            Console.Write("Enter City of arrival=");
            addflight.Parameters.AddWithValue("@City_of_arrival", Console.ReadLine());
            Console.Write("Enter City of departure=");
            addflight.Parameters.AddWithValue("@City_of_departure", Console.ReadLine());
            Console.Write("Enter Terminal=");
            addflight.Parameters.AddWithValue("@Terminal", Console.ReadLine());
            Console.Write("Enter Flight status=");
            addflight.Parameters.AddWithValue("@Flight_Status", Console.ReadLine());
            Console.Write("Enter Gate=");
            addflight.Parameters.AddWithValue("@Gate", Console.ReadLine());
            addflight.CommandText = "SELECT MAX(ID) FROM Flights";
            try
            {
                flight.ID = (Int32)addflight.ExecuteScalar();
            }
            catch (InvalidCastException)
            {
                flight.ID = 0;
            }
            addflight.Parameters.AddWithValue("@ID", flight.ID + 1);
            addflight.CommandText = @"INSERT INTO Flights(Arrival,Departure,Flight_Number,City_of_arrival,
            City_of_departure,Terminal,Flight_Status,Gate,ID)";
            addflight.CommandText += @"VALUES(@Arrival, @Departure, @Flight_Number, @City_of_arrival, 
            @City_of_departure, @Terminal, @Flight_Status, @Gate, @ID)";
            addflight.ExecuteNonQuery();
            sqlcon.Close();
        }
        public void Remove()
        {
            sqlcon.Open();
            Console.Write("Enter Flight that you want to remove=");
            string userchoice = Convert.ToString(Console.ReadLine());
            SqlCommand removeflight = new SqlCommand();
            removeflight.Connection = sqlcon;
            removeflight.CommandText = @"DELETE FROM Flights WHERE Flight_number LIKE @Name_of_flight
            OR ID LIKE @Name_of_flight";
            removeflight.Parameters.AddWithValue("@Name_of_flight", userchoice);
            removeflight.ExecuteNonQuery();
            sqlcon.Close();
        }
        public void ShowAll()
        {
            sqlcon.Open();
            SqlCommand showflights = new SqlCommand("SELECT * FROM Flights", sqlcon);
            SqlDataReader reader = showflights.ExecuteReader();
            StringBuilder writeIntoFile = new StringBuilder();
            while (reader.Read())
            {
                for (int i = 0; i < 9; i++)
                {
                    writeIntoFile.Append(reader.GetName(i) + ": ");
                    writeIntoFile.AppendLine(Convert.ToString(reader.GetValue(i)));
                }
                writeIntoFile.AppendLine("__________________________");
            }
            Console.WriteLine(writeIntoFile);
            File.WriteAllText("AllFlights.txt", writeIntoFile.ToString());
            sqlcon.Close();
        }
        public void ShowByFilter()
        {
            sqlcon.Open();
            Console.Write("Enter Flight that you want to find=");
            string userchoice = Convert.ToString(Console.ReadLine());
            SqlCommand searchflight = new SqlCommand();
            searchflight.Connection = sqlcon;
            searchflight.CommandText = @"SELECT * FROM Flights WHERE Flight_number LIKE @Name_of_flight 
            OR City_of_arrival LIKE @Name_of_flight OR City_of_departure LIKE @Name_of_flight 
            OR Terminal LIKE @Name_of_flight OR Flight_Status LIKE @Name_of_flight 
            OR Gate LIKE @Name_of_flight OR ID LIKE @Name_of_flight";
            searchflight.Parameters.AddWithValue("@Name_of_flight", userchoice);
            SqlDataReader showflight = searchflight.ExecuteReader();
            StringBuilder writeintofile = new StringBuilder();
            if (showflight.HasRows)
            {
                while (showflight.Read())
                {
                    for (int i = 0; i < 9; i++)
                    {
                        writeintofile.Append(showflight.GetName(i) + ": ");
                        writeintofile.AppendLine(Convert.ToString(showflight.GetValue(i)));
                    }
                    writeintofile.AppendLine();
                }
                Console.WriteLine(writeintofile);
                File.WriteAllText("Finded_Flight.txt", writeintofile.ToString());
            }
            else
            {
                Console.WriteLine("Searched flight didn't find");
            }
            sqlcon.Close();
        }
        public void ActionPassengers()
        {
            PassengerDataBase passengers = new PassengerDataBase();
            sqlcon.Open();
            Console.WriteLine("Enter Flight_Number where you want to view passengers");
            ConsoleKeyInfo Key_Reader;
            do
            {
                Console.WriteLine(@"1.Add passenger\n2.Remove passanger\n3.Show all passengers\n
                4.Show passanger by filter");
                Key_Reader = Console.ReadKey();
                Console.WriteLine();
                switch (Key_Reader.KeyChar)
                {
                    case '1':
                        passengers.Add();
                        break;
                    case '2':

                        passengers.Remove();
                        break;
                    case '3':

                        passengers.ShowAll();
                        break;
                    case '4':

                        passengers.ShowByFilter();
                        break;
                }
                Console.WriteLine("If you want to exit press escape otherwise press anykey");
                Key_Reader = Console.ReadKey();
                Console.WriteLine();
            }
            while (Key_Reader.Key != ConsoleKey.Escape);
            sqlcon.Close();
        }

    }
    struct Flight
    {
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        private int id;

    }
    class PassengerDataBase : IFlightable
    {
        internal static DateTime AddDateOnly()
        {
            int year;
            int day;
            int month;
            do
            {
                try
                {
                    do
                    {
                        Console.Write("Enter Year=");
                        year = int.Parse(Console.ReadLine());
                        try
                        {
                            if (year < 1900)
                            {
                                throw new DateYearCheck();
                            }
                        }
                        catch (DateYearCheck)
                        {
                            Console.WriteLine("You entered wrong year try again");
                        }
                    }
                    while (year < 1900);
                    Console.Write("Enter Month=");
                    month = int.Parse(Console.ReadLine());
                    Console.Write("Enter day=");
                    day = int.Parse(Console.ReadLine());
                    return new DateTime(year, month, day);
                }
                catch (FormatException)
                {
                    Console.WriteLine("You entered wrong value try again");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("You entered more value that variable exprect");
                }
            } while (true);

        }
        SqlConnection sqlcon;
        internal PassengerDataBase()
        {
            sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["AirportConnection"]
                .ConnectionString);
        }
        public void Add()
        {
            Console.Write("Enter Flight Number or id of flight=");
            string filter = Convert.ToString(Console.ReadLine());
            Console.WriteLine();
            ConsoleKeyInfo KeyReader;
            sqlcon.Open();
            SqlCommand searchflight = new SqlCommand(@"SELECT Flight_Number,ID FROM Flights WHERE 
            Flight_number LIKE @Name_of_flight OR ID LIKE @Name_of_flight",sqlcon);
            searchflight.Parameters.AddWithValue("@Name_of_flight", filter);
            SqlDataReader read_flight = searchflight.ExecuteReader();
            if (read_flight.HasRows)
            {

                string tempflightnumber = " ";
                int temp_id = 0;
                while (read_flight.Read())
                {
                    tempflightnumber = (string)read_flight.GetValue(0);
                    temp_id = (int)read_flight.GetValue(1);
                }
                do
                {
                    read_flight.Close();
                    SqlCommand addpassenger = new SqlCommand();
                    addpassenger.Connection = sqlcon;
                    Console.Write("Enter Name=");
                    addpassenger.Parameters.AddWithValue("@Name", Console.ReadLine());
                    Console.Write("Enter Second Name=");
                    addpassenger.Parameters.AddWithValue("@Second_name", Console.ReadLine());
                    Console.Write("Enter Nationality=");
                    addpassenger.Parameters.AddWithValue("@Nationality", Console.ReadLine());
                    Console.WriteLine("Enter Date of birthday");
                    addpassenger.Parameters.AddWithValue("@Birthday", AddDateOnly());
                    Console.Write("Enter Sex=");
                    addpassenger.Parameters.AddWithValue("@SEX", Console.ReadLine());
                    Console.Write("Enter Passport=");
                    addpassenger.Parameters.AddWithValue("@Passport", Console.ReadLine());
                    Console.Write("Enter Klass=");
                    addpassenger.Parameters.AddWithValue("@Class", Console.ReadLine());
                    addpassenger.CommandText = @"INSERT INTO Passengers(Name,Second_name,Nationality,SEX,
                    Birthday,Passport,Flight_Number,Class,ID) VALUES(@Name,@Second_name,@Nationality,
                    @SEX,@Birthday,@Passport,@Flight_Number,@Class,@ID)";
                    addpassenger.Parameters.AddWithValue("@ID", temp_id);
                    addpassenger.Parameters.AddWithValue("@Flight_Number", tempflightnumber);
                    addpassenger.ExecuteNonQuery();
                    Console.WriteLine("If you want to exit press escape,otherwise press anykey");
                    KeyReader = Console.ReadKey();
                }
                while (KeyReader.Key != ConsoleKey.Escape);
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist in Database");
            }
            sqlcon.Close();
        }
        public void Remove()
        {
            Console.Write("Enter Flight Number or id of flight=");
            string filter = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Enter first name of passenger that you want to remove= ");
            string temp_name = Console.ReadLine();
            Console.Write("Enter second name of passenger that you want to remove= ");
            string tempsecondname = Console.ReadLine();
            sqlcon.Open();
            SqlCommand searchflight = new SqlCommand(@"DELETE FROM Passengers WHERE (ID LIKE @filter 
            OR Flight_Number LIKE @filter) AND (Name LIKE @Name AND Second_name LIKE @Second_Name)",sqlcon);
            searchflight.Parameters.AddWithValue("@filter", filter);
            searchflight.Parameters.AddWithValue("@Name", temp_name);
            searchflight.Parameters.AddWithValue("@Second_Name", tempsecondname);
            searchflight.ExecuteNonQuery();
            sqlcon.Close();
        }
        public void ShowAll()
        {
            Console.Write("Enter Flight Number or id of flight=");
            string filter = Console.ReadLine();
            Console.WriteLine();
            sqlcon.Open();
            SqlCommand searchflight = new SqlCommand(@"SELECT * FROM Passengers WHERE Flight_number 
            LIKE @Name_of_flight OR ID LIKE @Name_of_flight",sqlcon);
            searchflight.Parameters.AddWithValue("@Name_of_flight", filter);
            SqlDataReader readflight = searchflight.ExecuteReader();
            StringBuilder writeintofile = new StringBuilder();
            if (readflight.HasRows)
            {
                while (readflight.Read())
                {
                    for (int i = 0; i < 8; i++)
                    {
                        writeintofile.AppendLine((readflight.GetName(i) + " :" + readflight.GetValue(i)));
                    }
                    writeintofile.AppendLine("________________________");
                }
                Console.WriteLine(writeintofile);
                File.WriteAllText("All_Passangers.txt", writeintofile.ToString());
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist in Database");
            }
            sqlcon.Close();
        }
        public void ShowByFilter()
        {
            Console.Write("Enter Flight Number or id of flight=");
            string filter = Console.ReadLine();
            Console.WriteLine();
            sqlcon.Open();
            SqlCommand searchflight = new SqlCommand(@"SELECT * FROM Passengers WHERE (Flight_number LIKE 
            @Name_of_flight OR ID LIKE @Name_of_flight) AND (Name LIKE @Name 
            AND Second_name LIKE @Second_Name)",sqlcon);
            string tempname;
            string tempsecondname;
            Console.Write("Enter name of passenger=");
            tempname = Console.ReadLine();
            Console.Write("Enter second name of passenger=");
            tempsecondname = Convert.ToString(Console.ReadLine());
            Console.WriteLine();
            searchflight.Parameters.AddWithValue("@Name_of_flight", filter);
            searchflight.Parameters.AddWithValue("@Name", tempname);
            searchflight.Parameters.AddWithValue("@Second_Name", tempsecondname);
            SqlDataReader read_flight = searchflight.ExecuteReader();
            StringBuilder writeintofile = new StringBuilder();
            if (read_flight.HasRows)
            {
                while (read_flight.Read())
                {
                    for (int i = 0; i < 8; i++)
                    {
                        writeintofile.AppendLine((read_flight.GetName(i) + " :" + read_flight.GetValue(i)));
                    }
                    writeintofile.AppendLine("________________________");
                }
                Console.WriteLine(writeintofile);
                File.WriteAllText("Finded_Passanger.txt", writeintofile.ToString());
            }
            else
            {
                Console.WriteLine("Your flight doesn't exist in Database");
            }
            sqlcon.Close();
        }
    }
}
