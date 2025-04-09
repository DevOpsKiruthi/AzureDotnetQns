using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using dotnetapp.Models;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class BrandRecommendationTests
    {
        private string connectionString = ConnectionStringProvider.ConnectionString;
        private StringWriter consoleOutput;
        private TextWriter originalConsoleOut;

        [SetUp]
        public void Setup()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM BrandRecommendation", conn);
                cmd.ExecuteNonQuery();
            }

            originalConsoleOut = Console.Out;
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalConsoleOut);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM BrandRecommendation", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_BrandRecommendation_Class_Should_Exist()
        {
            Type type = typeof(BrandRecommendation);
            Assert.IsNotNull(type, "BrandRecommendation class should exist.");
        }

        [Test, Order(2)]
        public void Test_BrandRecommendation_Properties_Should_Exist()
        {
            var type = typeof(BrandRecommendation);
            Assert.IsNotNull(type.GetProperty("ClientID"));
            Assert.IsNotNull(type.GetProperty("ClientName"));
            Assert.IsNotNull(type.GetProperty("Email"));
            Assert.IsNotNull(type.GetProperty("CampaignType"));
            Assert.IsNotNull(type.GetProperty("TargetAudience"));
            Assert.IsNotNull(type.GetProperty("CoreMessage"));
            Assert.IsNotNull(type.GetProperty("SuggestedChannels"));
            Assert.IsNotNull(type.GetProperty("CreatedAt"));
        }

        [Test, Order(3)]
        public void Test_AddBrandRecommendation_Method_Exists()
        {
            MethodInfo addMethod = typeof(Program).GetMethod("AddBrandRecommendation", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(addMethod, "AddBrandRecommendation method should exist in Program class.");
        }

        [Test, Order(4)]
        public void Test_DeleteRecommendationByCampaignType_Method_Exists()
        {
            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteRecommendationByCampaignType", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(deleteMethod, "DeleteRecommendationByCampaignType method should exist in Program class.");
        }

        [Test, Order(5)]
        public void Test_DisplayAllRecommendations_Method_Exists()
        {
            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllRecommendations", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(displayMethod, "DisplayAllRecommendations method should exist in Program class.");
        }

        [Test, Order(6)]
        public void Test_AddBrandRecommendation_Should_Insert_Into_Database()
        {
            var input = string.Join(Environment.NewLine, new[]
            {
                "Nova Inc.",
                "contact@novainc.com",
                "Social Media",
                "Young Adults",
                "Empowering Tomorrow's Leaders",
                "Instagram, TikTok",
                "2025-09-08"
            });

            Console.SetIn(new StringReader(input));

            MethodInfo addMethod = typeof(Program).GetMethod("AddBrandRecommendation", BindingFlags.Static | BindingFlags.Public);
            addMethod.Invoke(null, null);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM BrandRecommendation WHERE ClientName = @ClientName AND CampaignType = @CampaignType", conn);
                cmd.Parameters.AddWithValue("@ClientName", "Nova Inc.");
                cmd.Parameters.AddWithValue("@CampaignType", "Social Media");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "BrandRecommendation should be inserted into the database.");
            }
        }

        [Test, Order(7)]
        public void Test_DeleteRecommendationByCampaignType_Should_Remove_Records()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO BrandRecommendation 
                    (ClientName, Email, CampaignType, TargetAudience, CoreMessage, SuggestedChannels, CreatedAt)
                    VALUES (@ClientName, @Email, @CampaignType, @TargetAudience, @CoreMessage, @SuggestedChannels, @CreatedAt)", conn);

                cmd.Parameters.AddWithValue("@ClientName", "Future Bright Co");
                cmd.Parameters.AddWithValue("@Email", "bright@future.com");
                cmd.Parameters.AddWithValue("@CampaignType", "Email Marketing");
                cmd.Parameters.AddWithValue("@TargetAudience", "Professionals");
                cmd.Parameters.AddWithValue("@CoreMessage", "Brighten Your Brand");
                cmd.Parameters.AddWithValue("@SuggestedChannels", "Mailchimp, CRM");
                cmd.Parameters.AddWithValue("@CreatedAt", "2025-09-08");
                cmd.ExecuteNonQuery();
            }

            Console.SetIn(new StringReader("Email Marketing"));

            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteRecommendationByCampaignType", BindingFlags.Static | BindingFlags.Public);
            deleteMethod.Invoke(null, new object[] { "Email Marketing" });

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM BrandRecommendation WHERE CampaignType = @CampaignType", conn);
                cmd.Parameters.AddWithValue("@CampaignType", "Email Marketing");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(0, count, "Recommendations with the specified campaign type should be deleted.");
            }
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentRecommendation_Should_Show_Message()
        {
            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteRecommendationByCampaignType", BindingFlags.Static | BindingFlags.Public);
            deleteMethod.Invoke(null, new object[] { "Billboard" });

            var output = consoleOutput.ToString().ToLower();
            Assert.IsTrue(output.Contains("no recommendations found with campaign type"), "Should show not found message.");
        }

        [Test, Order(9)]
        public void Test_DisplayAllRecommendations_Should_Show_Data()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO BrandRecommendation 
                    (ClientName, Email, CampaignType, TargetAudience, CoreMessage, SuggestedChannels, CreatedAt)
                    VALUES (@ClientName, @Email, @CampaignType, @TargetAudience, @CoreMessage, @SuggestedChannels, @CreatedAt)", conn);

                cmd.Parameters.AddWithValue("@ClientName", "Echo Marketing");
                cmd.Parameters.AddWithValue("@Email", "hello@echo.com");
                cmd.Parameters.AddWithValue("@CampaignType", "Influencer");
                cmd.Parameters.AddWithValue("@TargetAudience", "Gen Z");
                cmd.Parameters.AddWithValue("@CoreMessage", "Let Your Voice Be Heard");
                cmd.Parameters.AddWithValue("@SuggestedChannels", "YouTube, Instagram");
                cmd.Parameters.AddWithValue("@CreatedAt", "2025-09-08");
                cmd.ExecuteNonQuery();
            }

            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllRecommendations", BindingFlags.Static | BindingFlags.Public);
            displayMethod.Invoke(null, null);

            var output = consoleOutput.ToString().ToLower();
            StringAssert.Contains("echo marketing", output);
            StringAssert.Contains("influencer", output);
        }
    }
}
