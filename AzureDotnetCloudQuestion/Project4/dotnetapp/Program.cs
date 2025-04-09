using System;
using System.Data;
using System.Data.SqlClient;
using dotnetapp.Models;

namespace dotnetapp
{
    public static class ConnectionStringProvider
    {
        public static string ConnectionString { get; } = "User ID=sa;password=examlyMssql@123; server=localhost;Database=appdb;trusted_connection=false;Persist Security Info=False;Encrypt=False";
    }

    public class Program
    {
        static string connectionString = ConnectionStringProvider.ConnectionString;

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nDigital Ads Management Menu");
                Console.WriteLine("1. Add Digital Ad");
                Console.WriteLine("2. Delete Digital Ad by ID");
                Console.WriteLine("3. Display All Digital Ads");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Collect digital ad details from the user
                        Console.Write("Enter Ad Title: ");
                        string adTitle = Console.ReadLine();

                        Console.Write("Enter Ad Content: ");
                        string adContent = Console.ReadLine();

                        Console.Write("Enter Start Date (YYYY-MM-DD): ");
                        string startDate = Console.ReadLine();

                        Console.Write("Enter End Date (YYYY-MM-DD): ");
                        string endDate = Console.ReadLine();

                        Console.Write("Enter Display Location: ");
                        string displayLocation = Console.ReadLine();

                        Console.Write("Enter Ad Type: ");
                        string adType = Console.ReadLine();

                        DigitalAds newAd = new DigitalAds
                        {
                            AdTitle = adTitle,
                            AdContent = adContent,
                            StartDate = startDate,
                            EndDate = endDate,
                            DisplayLocation = displayLocation,
                            AdType = adType
                        };

                        AddDigitalAd(newAd);
                        break;

                    case "2":
                        Console.Write("Enter Digital Ad ID to delete: ");
                        int adIdToDelete = int.Parse(Console.ReadLine());
                        DeleteDigitalAd(adIdToDelete);
                        break;

                    case "3":
                        DisplayAllDigitalAds();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        // Add a new digital ad to the database
        public static void AddDigitalAd(DigitalAds ad)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO DigitalAds (AdTitle, AdContent, StartDate, EndDate, DisplayLocation, AdType) OUTPUT INSERTED.DigitalAdID VALUES (@AdTitle, @AdContent, @StartDate, @EndDate, @DisplayLocation, @AdType)", connection);
                cmd.Parameters.AddWithValue("@AdTitle", ad.AdTitle);
                cmd.Parameters.AddWithValue("@AdContent", ad.AdContent);
                cmd.Parameters.AddWithValue("@StartDate", ad.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", ad.EndDate);
                cmd.Parameters.AddWithValue("@DisplayLocation", ad.DisplayLocation);
                cmd.Parameters.AddWithValue("@AdType", ad.AdType);

                connection.Open();
                int digitalAdId = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Digital Ad added successfully with ID: {digitalAdId}");
            }
        }

        // Delete a digital ad by its ID
        public static void DeleteDigitalAd(int digitalAdID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM DigitalAds WHERE DigitalAdID = @DigitalAdID", connection);
                cmd.Parameters.AddWithValue("@DigitalAdID", digitalAdID);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Digital Ad deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"No digital ad found with ID {digitalAdID}.");
                }
            }
        }

        // Display all digital ads in the database
        public static void DisplayAllDigitalAds()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM DigitalAds", connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "DigitalAds");

                DataTable adsTable = dataSet.Tables["DigitalAds"];

                if (adsTable.Rows.Count > 0)
                {
                    foreach (DataRow row in adsTable.Rows)
                    {
                        Console.WriteLine($"DigitalAdID: {row["DigitalAdID"]}\tAdTitle: {row["AdTitle"]}\tAdContent: {row["AdContent"]}\tStartDate: {row["StartDate"]}\tEndDate: {row["EndDate"]}\tDisplayLocation: {row["DisplayLocation"]}\tAdType: {row["AdType"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No digital ads found.");
                }
            }
        }
    }
}
