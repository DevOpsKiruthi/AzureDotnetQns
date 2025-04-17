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
                Console.WriteLine("\nSeasonal Crop Plantation Management Menu");
                Console.WriteLine("1. Add Seasonal Crop");
                Console.WriteLine("2. Delete Crop by Season");
                Console.WriteLine("3. Display All Crops");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Crop Name: ");
                        string name = Console.ReadLine();

                        Console.Write("Enter Season (e.g., Summer, Monsoon): ");
                        string season = Console.ReadLine();

                        Console.Write("Enter Start Date (yyyy-MM-dd): ");
                        string startDate = Console.ReadLine();

                        Console.Write("Enter End Date (yyyy-MM-dd): ");
                        string endDate = Console.ReadLine();

                        Console.Write("Enter Description: ");
                        string description = Console.ReadLine();

                        SeasonalCrop crop = new SeasonalCrop
                        {
                            CropName = name,
                            Season = season,
                            StartDate = startDate,
                            EndDate = endDate,
                            Description = description
                        };

                        AddSeasonalCrop(crop);
                        break;

                    case "2":
                        Console.Write("Enter Season to delete crops: ");
                        string seasonToDelete = Console.ReadLine();
                        DeleteSeasonalCropBySeason(seasonToDelete);
                        break;

                    case "3":
                        DisplayAllSeasonalCrops();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        public static void AddSeasonalCrop(SeasonalCrop crop)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO SeasonalCrop (CropName, Season, StartDate, EndDate, Description) VALUES (@CropName, @Season, @StartDate, @EndDate, @Description)";
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@CropName", crop.CropName);
                cmd.Parameters.AddWithValue("@Season", crop.Season);
                cmd.Parameters.AddWithValue("@StartDate", crop.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", crop.EndDate);
                cmd.Parameters.AddWithValue("@Description", crop.Description ?? (object)DBNull.Value);

                connection.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Crop added successfully.");
            }
        }

        public static void DeleteSeasonalCropBySeason(string season)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM SeasonalCrop WHERE Season = @Season", connection);
                cmd.Parameters.AddWithValue("@Season", season);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine("Crop(s) deleted successfully.");
                else
                    Console.WriteLine($"No seasonal crop found with season {season}");

            }
        }

        public static void DisplayAllSeasonalCrops()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SeasonalCrop", connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "SeasonalCrop");

                DataTable cropTable = dataSet.Tables["SeasonalCrop"];

                if (cropTable.Rows.Count > 0)
                {
                    foreach (DataRow row in cropTable.Rows)
                    {
                        Console.WriteLine($"CropID: {row["CropID"]}\tName: {row["CropName"]}\tSeason: {row["Season"]}\tStartDate: {row["StartDate"]}\tEndDate: {row["EndDate"]}\tDescription: {row["Description"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No seasonal crops found.");
                }
            }
        }
    }
}
