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
                Console.WriteLine("\nSocial Media Content Management Menu");
                Console.WriteLine("1. Add Social Media Post");
                Console.WriteLine("2. Delete Social Media Post by Platform");
                Console.WriteLine("3. Display All Social Media Posts");
                Console.WriteLine("4. Exit");

                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Collect social media post details from the user
                        Console.Write("Enter Title: ");
                        string title = Console.ReadLine();

                        Console.Write("Enter Content: ");
                        string content = Console.ReadLine();

                        Console.Write("Enter Publish Date (YYYY-MM-DD): ");
                        string publishDate = Console.ReadLine();

                        Console.Write("Enter Platform (e.g., Facebook, Twitter): ");
                        string platform = Console.ReadLine();

                        Console.Write("Enter Status (Draft, Scheduled, Published): ");
                        string status = Console.ReadLine();

                        SocialMediaPost newPost = new SocialMediaPost
                        {
                            Title = title,
                            Content = content,
                            PublishDate = publishDate,
                            Platform = platform,
                            Status = status
                        };

                        AddSocialMediaPost(newPost);
                        break;

                    case "2":
                        Console.Write("Enter Platform to delete posts: ");
                        string platformToDelete = Console.ReadLine();
                        DeleteSocialMediaPostByPlatform(platformToDelete);
                        break;

                    case "3":
                        DisplayAllSocialMediaPosts();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        // Add a new social media post to the database
        public static void AddSocialMediaPost(SocialMediaPost post)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO SocialMediaPosts (Title, Content, PublishDate, Platform, Status) OUTPUT INSERTED.PostID VALUES (@Title, @Content, @PublishDate, @Platform, @Status)", 
                    connection);

                cmd.Parameters.AddWithValue("@Title", post.Title);
                cmd.Parameters.AddWithValue("@Content", post.Content);
                cmd.Parameters.AddWithValue("@PublishDate", post.PublishDate);
                cmd.Parameters.AddWithValue("@Platform", post.Platform);
                cmd.Parameters.AddWithValue("@Status", post.Status);

                connection.Open();
                int postId = (int)cmd.ExecuteScalar();
                Console.WriteLine("Social Media Post added successfully");
            }
        }

        // Delete social media posts by the specified platform
        public static void DeleteSocialMediaPostByPlatform(string platform)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM SocialMediaPosts WHERE Platform = @Platform", connection);
                cmd.Parameters.AddWithValue("@Platform", platform);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Social Media Post(s) deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"No social media posts found with platform {platform}.");
                }
            }
        }

        // Display all social media posts in the database
        public static void DisplayAllSocialMediaPosts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SocialMediaPosts", connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "SocialMediaPosts");

                DataTable postTable = dataSet.Tables["SocialMediaPosts"];

                if (postTable.Rows.Count > 0)
                {
                    foreach (DataRow row in postTable.Rows)
                    {
                        Console.WriteLine($"PostID: {row["PostID"]}\tTitle: {row["Title"]}\tContent: {row["Content"]}\tPublishDate: {row["PublishDate"]}\tPlatform: {row["Platform"]}\tStatus: {row["Status"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No social media posts found.");
                }
            }
        }
    }
}
