using System;
using System.Collections.Generic;
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
                Console.WriteLine("\nSuit Recommendation Management Menu");
                Console.WriteLine("1. Add Suit Recommendation");
                Console.WriteLine("2. Delete Suit Recommendation by Email");
                Console.WriteLine("3. Display All Suit Recommendations");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Collect suit recommendation details from the user
                        Console.Write("Enter Full Name: ");
                        string fullName = Console.ReadLine();

                        Console.Write("Enter Email: ");
                        string email = Console.ReadLine();

                        Console.Write("Enter Occasion (e.g., Work, Casual, Formal, Party): ");
                        string occasion = Console.ReadLine();

                        Console.Write("Enter Preferred Style (e.g., Classic, Trendy, Minimalist, Bold): ");
                        string preferredStyle = Console.ReadLine();

                        Console.Write("Enter Color Preference (e.g., Black, Navy, Beige, Bright Colors): ");
                        string colorPreference = Console.ReadLine();

                        Console.Write("Enter Fabric Preference (e.g., Cotton, Wool, Linen, Velvet): ");
                        string fabricPreference = Console.ReadLine();

                        Console.Write("Enter Suggested Pairings: ");
                        string suggestedPairings = Console.ReadLine();

                        SuitRecommendationModel newRecommendation = new SuitRecommendationModel
                        {
                            FullName = fullName,
                            Email = email,
                            Occasion = occasion,
                            PreferredStyle = preferredStyle,
                            ColorPreference = colorPreference,
                            FabricPreference = fabricPreference,
                            SuggestedPairings = suggestedPairings,
                            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
                        };

                        AddSuitRecommendation(newRecommendation);
                        break;

                    case "2":
                        Console.Write("Enter Email of the recommendation to delete: ");
                        string emailToDelete = Console.ReadLine();
                        DeleteSuitRecommendation(emailToDelete);
                        break;

                    case "3":
                        DisplayAllSuitRecommendations();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        // Add a new suit recommendation to the database
        public static void AddSuitRecommendation(SuitRecommendationModel recommendation)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO SuitRecommendations 
                                 (FullName, Email, Occasion, PreferredStyle, ColorPreference, FabricPreference, SuggestedPairings, CreatedAt)
                                 VALUES (@FullName, @Email, @Occasion, @PreferredStyle, @ColorPreference, @FabricPreference, @SuggestedPairings, @CreatedAt)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@FullName", recommendation.FullName);
                    cmd.Parameters.AddWithValue("@Email", recommendation.Email);
                    cmd.Parameters.AddWithValue("@Occasion", recommendation.Occasion);
                    cmd.Parameters.AddWithValue("@PreferredStyle", recommendation.PreferredStyle);
                    cmd.Parameters.AddWithValue("@ColorPreference", recommendation.ColorPreference);
                    cmd.Parameters.AddWithValue("@FabricPreference", recommendation.FabricPreference);
                    cmd.Parameters.AddWithValue("@SuggestedPairings", recommendation.SuggestedPairings);
                    cmd.Parameters.AddWithValue("@CreatedAt", recommendation.CreatedAt);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Suit recommendation added successfully.");
                }
            }
        }

        // Delete a suit recommendation by Email
        public static void DeleteSuitRecommendation(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM SuitRecommendations WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Suit recommendation deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"No suit recommendation found with Email: {email}");
                    }
                }
            }
        }

        // Display all suit recommendations in the database
        public static void DisplayAllSuitRecommendations()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM SuitRecommendations";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "SuitRecommendations");

                DataTable recommendationTable = dataSet.Tables["SuitRecommendations"];

                if (recommendationTable.Rows.Count > 0)
                {
                    foreach (DataRow row in recommendationTable.Rows)
                    {
                        Console.WriteLine(
                            $"FullName: {row["FullName"]}\tEmail: {row["Email"]}\tOccasion: {row["Occasion"]}" +
                            $"\tPreferredStyle: {row["PreferredStyle"]}\tColorPreference: {row["ColorPreference"]}" +
                            $"\tFabricPreference: {row["FabricPreference"]}\tSuggestedPairings: {row["SuggestedPairings"]}" +
                            $"\tCreatedAt: {row["CreatedAt"]}"
                        );
                    }
                }
                else
                {
                    Console.WriteLine("No suit recommendations found.");
                }
            }
        }
    }
}
