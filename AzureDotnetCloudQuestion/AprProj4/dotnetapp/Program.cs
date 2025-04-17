using System;
using System.Data;
using System.Data.SqlClient;
using dotnetapp.Models;

namespace dotnetapp
{
    public static class ConnectionStringProvider
    {
        public static string ConnectionString { get; } =
            "User ID=sa;Password=examlyMssql@123;Server=localhost;Database=appdb;Trusted_Connection=False;Encrypt=False";
    }

    public class Program
    {
        static string connectionString = ConnectionStringProvider.ConnectionString;

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nMarketing Branding Recommendation Management Menu");
                Console.WriteLine("1. Add Branding Recommendation");
                Console.WriteLine("2. Delete Recommendation by Campaign Type");
                Console.WriteLine("3. Display All Recommendations");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBrandRecommendation();
                        break;

                    case "2":
                        Console.Write("Enter Campaign Type to delete: ");
                        string typeToDelete = Console.ReadLine();
                        DeleteRecommendationByCampaignType(typeToDelete);
                        break;

                    case "3":
                        DisplayAllRecommendations();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        public static void AddBrandRecommendation()
        {
            Console.Write("Client Name: ");
            string clientName = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Campaign Type: ");
            string campaignType = Console.ReadLine();

            Console.Write("Target Audience: ");
            string targetAudience = Console.ReadLine();

            Console.Write("Core Message: ");
            string coreMessage = Console.ReadLine();

            Console.Write("Suggested Channels: ");
            string suggestedChannels = Console.ReadLine();

            Console.Write("Creation Date (yyyy-MM-dd): ");
            string createdAt = Console.ReadLine();

            BrandRecommendation recommendation = new BrandRecommendation
            {
                ClientName = clientName,
                Email = email,
                CampaignType = campaignType,
                TargetAudience = targetAudience,
                CoreMessage = coreMessage,
                SuggestedChannels = suggestedChannels,
                CreatedAt = createdAt
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO BrandRecommendation 
                                (ClientName, Email, CampaignType, TargetAudience, CoreMessage, SuggestedChannels, CreatedAt) 
                                 VALUES 
                                (@ClientName, @Email, @CampaignType, @TargetAudience, @CoreMessage, @SuggestedChannels, @CreatedAt)";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ClientName", recommendation.ClientName);
                cmd.Parameters.AddWithValue("@Email", recommendation.Email);
                cmd.Parameters.AddWithValue("@CampaignType", recommendation.CampaignType);
                cmd.Parameters.AddWithValue("@TargetAudience", recommendation.TargetAudience);
                cmd.Parameters.AddWithValue("@CoreMessage", recommendation.CoreMessage);
                cmd.Parameters.AddWithValue("@SuggestedChannels", recommendation.SuggestedChannels);
                cmd.Parameters.AddWithValue("@CreatedAt", recommendation.CreatedAt);

                connection.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Recommendation added successfully.");
            }
        }

        public static void DeleteRecommendationByCampaignType(string campaignType)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM BrandRecommendation WHERE CampaignType = @CampaignType", connection);
                cmd.Parameters.AddWithValue("@CampaignType", campaignType);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine("Recommendation(s) deleted successfully.");
                else
                    Console.WriteLine($"No recommendations found with campaign type '{campaignType}'.");
            }
        }

        public static void DisplayAllRecommendations()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM BrandRecommendation", connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "BrandRecommendation");

                DataTable table = ds.Tables["BrandRecommendation"];

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Console.WriteLine($"\nClient ID: {row["ClientID"]}\nClient Name: {row["ClientName"]}\nEmail: {row["Email"]}");
                        Console.WriteLine($"Campaign Type: {row["CampaignType"]}\nTarget Audience: {row["TargetAudience"]}");
                        Console.WriteLine($"Core Message: {row["CoreMessage"]}\nSuggested Channels: {row["SuggestedChannels"]}\nCreated At: {row["CreatedAt"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No brand recommendations found.");
                }
            }
        }
    }
}
