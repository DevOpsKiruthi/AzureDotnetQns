using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using dotnetapp;
using System.Reflection;
using dotnetapp.Models;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class SuitRecommendationTests
    {
        private string connectionString = ConnectionStringProvider.ConnectionString;
        private StringWriter consoleOutput;
        private TextWriter originalConsoleOut;

        [SetUp]
        public void Setup()
        {
            // Clear the SuitRecommendations table before each test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SuitRecommendations", conn);
                cmd.ExecuteNonQuery();
            }

            // Redirect console output to capture messages
            originalConsoleOut = Console.Out;
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            // Reset console output
            Console.SetOut(originalConsoleOut);

            // Clear the table after each test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SuitRecommendations", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_SuitRecommendationModel_Class_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SuitRecommendationModel";
            Assembly assembly = Assembly.Load(assemblyName);
            Type recommendationType = assembly.GetType(typeName);

            // Assert
            Assert.IsNotNull(recommendationType, "SuitRecommendationModel class should exist.");
        }

        [Test, Order(2)]
        public void Test_SuitRecommendationModel_Properties_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SuitRecommendationModel";
            Assembly assembly = Assembly.Load(assemblyName);
            Type recommendationType = assembly.GetType(typeName);

            // Act & Assert: Check for all required properties
            Assert.IsNotNull(recommendationType.GetProperty("FullName"), "FullName property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("Email"), "Email property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("Occasion"), "Occasion property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("PreferredStyle"), "PreferredStyle property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("ColorPreference"), "ColorPreference property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("FabricPreference"), "FabricPreference property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("SuggestedPairings"), "SuggestedPairings property should exist.");
            Assert.IsNotNull(recommendationType.GetProperty("CreatedAt"), "CreatedAt property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddSuitRecommendation_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddSuitRecommendation");
            Assert.IsNotNull(method, "AddSuitRecommendation method should exist.");
        }

        [Test, Order(4)]
        public void Test_DeleteSuitRecommendation_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DeleteSuitRecommendation");
            Assert.IsNotNull(method, "DeleteSuitRecommendation method should exist.");
        }

        [Test, Order(5)]
        public void Test_DisplayAllSuitRecommendations_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllSuitRecommendations");
            Assert.IsNotNull(method, "DisplayAllSuitRecommendations method should exist.");
        }

        [Test, Order(6)]
        public void Test_AddSuitRecommendation_Should_Insert_Recommendation_Into_Database()
        {
            // Arrange: Dynamically create a SuitRecommendationModel instance
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SuitRecommendationModel";
            Assembly assembly = Assembly.Load(assemblyName);
            Type recommendationType = assembly.GetType(typeName);
            object recommendationInstance = Activator.CreateInstance(recommendationType);

            // Setting properties dynamically
            recommendationType.GetProperty("FullName").SetValue(recommendationInstance, "John Smith");
            recommendationType.GetProperty("Email").SetValue(recommendationInstance, "john.smith@example.com");
            recommendationType.GetProperty("Occasion").SetValue(recommendationInstance, "Formal");
            recommendationType.GetProperty("PreferredStyle").SetValue(recommendationInstance, "Classic");
            recommendationType.GetProperty("ColorPreference").SetValue(recommendationInstance, "Black");
            recommendationType.GetProperty("FabricPreference").SetValue(recommendationInstance, "Wool");
            recommendationType.GetProperty("SuggestedPairings").SetValue(recommendationInstance, "Pair with a crisp white shirt, Add a slim tie");
            recommendationType.GetProperty("CreatedAt").SetValue(recommendationInstance, DateTime.UtcNow.ToString("yyyy-MM-dd"));

            // Act: Call AddSuitRecommendation method via reflection
            MethodInfo addMethod = typeof(Program).GetMethod("AddSuitRecommendation");
            addMethod.Invoke(null, new[] { recommendationInstance });

            // Assert: Verify the recommendation was inserted into the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM SuitRecommendations WHERE Email = @Email AND FullName = @FullName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", "john.smith@example.com");
                cmd.Parameters.AddWithValue("@FullName", "John Smith");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "The recommendation should have been inserted into the database.");
            }
        }

        [Test, Order(7)]
        public void Test_DeleteSuitRecommendation_ValidEmail_ShouldDeleteRecommendation()
        {
            // Arrange: Create and add a recommendation using the model class directly
            var recommendation = new SuitRecommendationModel
            {
                FullName = "John Smith",
                Email = "john.smith@example.com",
                Occasion = "Formal",
                PreferredStyle = "Classic",
                ColorPreference = "Black",
                FabricPreference = "Wool",
                SuggestedPairings = "Pair with a crisp white shirt, Add a slim tie",
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            // Use reflection to add the recommendation
            MethodInfo addMethod = typeof(Program).GetMethod("AddSuitRecommendation");
            addMethod.Invoke(null, new object[] { recommendation });

            // Act: Call DeleteSuitRecommendation method by providing the email via reflection
            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteSuitRecommendation");
            deleteMethod.Invoke(null, new object[] { "john.smith@example.com" });

            // Assert: Verify the recommendation was deleted from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM SuitRecommendations WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", "john.smith@example.com");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(0, count, "The recommendation should have been deleted from the database.");
            }
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentSuitRecommendation()
        {
            // Act: Attempt to delete a recommendation with a non-existent email via reflection
            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteSuitRecommendation");
            deleteMethod.Invoke(null, new object[] { "nonexistent@example.com" });

            string output = consoleOutput.ToString().Trim();
            Assert.IsTrue(output.ToLower().Contains("no suit recommendation found with email"),
                "Appropriate message should be displayed for non-existent recommendation.");
        }

        [Test, Order(9)]
        public void Test_DisplayAllSuitRecommendations_ShouldReturnData()
        {
            // Arrange: Create and add a recommendation using the model class directly
            var recommendation = new SuitRecommendationModel
            {
                FullName = "John Smith",
                Email = "john.smith@example.com",
                Occasion = "Formal",
                PreferredStyle = "Classic",
                ColorPreference = "Black",
                FabricPreference = "Wool",
                SuggestedPairings = "Pair with a crisp white shirt, Add a slim tie",
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            // Use reflection to add the recommendation
            MethodInfo addMethod = typeof(Program).GetMethod("AddSuitRecommendation");
            addMethod.Invoke(null, new object[] { recommendation });

            // Act: Call DisplayAllSuitRecommendations method via reflection
            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllSuitRecommendations");
            displayMethod.Invoke(null, null);

            // Assert: Verify that the recommendation details are displayed in the console output
            string output = consoleOutput.ToString();
            Assert.IsFalse(output.Contains("No suit recommendations found."), "Suit recommendations should be displayed.");
            StringAssert.Contains("John Smith", output);
            StringAssert.Contains("Formal", output);
        }
    }
}
