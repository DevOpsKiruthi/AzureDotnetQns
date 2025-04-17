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
    public class DigitalAdsManagementTests
    {
        private string connectionString = ConnectionStringProvider.ConnectionString;
        private StringWriter consoleOutput;
        private TextWriter originalConsoleOut;

        [SetUp]
        public void Setup()
        {
            // Clear the database before each test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM DigitalAds", conn);
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM DigitalAds", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_DigitalAds_Class_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.DigitalAds";
            Assembly assembly = Assembly.Load(assemblyName);
            Type digitalAdsType = assembly.GetType(typeName);

            // Assert
            Assert.IsNotNull(digitalAdsType, "DigitalAds class should exist.");
        }

        [Test, Order(2)]
        public void Test_DigitalAds_Properties_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.DigitalAds";
            Assembly assembly = Assembly.Load(assemblyName);
            Type digitalAdsType = assembly.GetType(typeName);

            // Act
            var digitalAdIdProperty = digitalAdsType.GetProperty("DigitalAdID");
            var adTitleProperty = digitalAdsType.GetProperty("AdTitle");
            var adContentProperty = digitalAdsType.GetProperty("AdContent");
            var startDateProperty = digitalAdsType.GetProperty("StartDate");
            var endDateProperty = digitalAdsType.GetProperty("EndDate");
            var displayLocationProperty = digitalAdsType.GetProperty("DisplayLocation");
            var adTypeProperty = digitalAdsType.GetProperty("AdType");

            // Assert
            Assert.IsNotNull(digitalAdIdProperty, "DigitalAdID property should exist.");
            Assert.IsNotNull(adTitleProperty, "AdTitle property should exist.");
            Assert.IsNotNull(adContentProperty, "AdContent property should exist.");
            Assert.IsNotNull(startDateProperty, "StartDate property should exist.");
            Assert.IsNotNull(endDateProperty, "EndDate property should exist.");
            Assert.IsNotNull(displayLocationProperty, "DisplayLocation property should exist.");
            Assert.IsNotNull(adTypeProperty, "AdType property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddDigitalAd_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddDigitalAd");
            Assert.IsNotNull(method, "AddDigitalAd method should exist.");
        }

        [Test, Order(4)]
        public void Test_DeleteDigitalAd_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DeleteDigitalAd");
            Assert.IsNotNull(method, "DeleteDigitalAd method should exist.");
        }

        [Test, Order(5)]
        public void Test_DisplayAllDigitalAds_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllDigitalAds");
            Assert.IsNotNull(method, "DisplayAllDigitalAds method should exist.");
        }

        [Test, Order(6)]
        public void Test_AddDigitalAd_Should_Insert_Ad_Into_Database()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.DigitalAds";
            Assembly assembly = Assembly.Load(assemblyName);
            Type digitalAdsType = assembly.GetType(typeName);
            object adInstance = Activator.CreateInstance(digitalAdsType);

            // Setting properties dynamically
            digitalAdsType.GetProperty("AdTitle").SetValue(adInstance, "Summer Sale");
            digitalAdsType.GetProperty("AdContent").SetValue(adInstance, "Up to 50% off on selected items");
            digitalAdsType.GetProperty("StartDate").SetValue(adInstance, "2024-06-01");
            digitalAdsType.GetProperty("EndDate").SetValue(adInstance, "2024-06-30");
            digitalAdsType.GetProperty("DisplayLocation").SetValue(adInstance, "Bus Interior");
            digitalAdsType.GetProperty("AdType").SetValue(adInstance, "Promotional");

            // Act - Call AddDigitalAd method
            MethodInfo addDigitalAdMethod = typeof(Program).GetMethod("AddDigitalAd");
            addDigitalAdMethod.Invoke(null, new[] { adInstance });

            // Assert - Check if the digital ad was added to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DigitalAds WHERE AdTitle = @AdTitle AND AdContent = @AdContent", conn);
                cmd.Parameters.AddWithValue("@AdTitle", "Summer Sale");
                cmd.Parameters.AddWithValue("@AdContent", "Up to 50% off on selected items");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "The digital ad should have been inserted into the database.");
            }
        }

        [Test, Order(7)]
        public void Test_DeleteDigitalAd_ValidId_ShouldDeleteAd()
        {
            int digitalAdId;

            var adObj = new Dictionary<string, object>
            {
                { "AdTitle", "Winter Clearance" },
                { "AdContent", "Clearance sale - everything must go!" },
                { "StartDate", "2024-12-01" },
                { "EndDate", "2024-12-31" },
                { "DisplayLocation", "Train Interior" },
                { "AdType", "Clearance" }
            };

            // Add a new digital ad entity to the context
            var ad = new DigitalAds();
            foreach (var kvp in adObj)
            {
                var propertyInfo = typeof(DigitalAds).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(ad, kvp.Value);
                }
            }
            Program.AddDigitalAd(ad);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 DigitalAdID FROM DigitalAds ORDER BY DigitalAdID DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    digitalAdId = (int)cmd.ExecuteScalar();
                }
            }

            // Act - Call DeleteDigitalAd method
            MethodInfo deleteDigitalAdMethod = typeof(Program).GetMethod("DeleteDigitalAd");
            deleteDigitalAdMethod.Invoke(null, new object[] { digitalAdId });

            // Assert - Check if the digital ad was deleted from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM DigitalAds WHERE DigitalAdID = @DigitalAdID", conn);
                cmd.Parameters.AddWithValue("@DigitalAdID", digitalAdId);
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(0, count, "The digital ad should have been deleted from the database.");
            }
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentDigitalAd()
        {
            // Act - Call DeleteDigitalAd method for a non-existent digital ad
            MethodInfo deleteDigitalAdMethod = typeof(Program).GetMethod("DeleteDigitalAd");
            deleteDigitalAdMethod.Invoke(null, new object[] { 999 });

            string output = consoleOutput.ToString().Trim();
            Assert.IsTrue(output.ToLower().Contains("no digital ad found with id 999"));
        }

        [Test, Order(9)]
        public void Test_DisplayAllDigitalAds_ShouldReturnData()
        {
            var adObj = new Dictionary<string, object>
            {
                { "AdTitle", "Flash Deal" },
                { "AdContent", "Limited time offer: 30% off!" },
                { "StartDate", "2024-05-01" },
                { "EndDate", "2024-05-02" },
                { "DisplayLocation", "Taxi Interior" },
                { "AdType", "Promotional" }
            };

            // Add a new digital ad entity to the context
            var ad = new DigitalAds();
            foreach (var kvp in adObj)
            {
                var propertyInfo = typeof(DigitalAds).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(ad, kvp.Value);
                }
            }
            Program.AddDigitalAd(ad);

            // Act - Call DisplayAllDigitalAds method
            MethodInfo displayAllDigitalAdsMethod = typeof(Program).GetMethod("DisplayAllDigitalAds");
            displayAllDigitalAdsMethod.Invoke(null, null);

            string output = consoleOutput.ToString();
            Assert.IsFalse(output.Contains("No digital ads found."), "Digital ads should be displayed.");
            StringAssert.Contains("Flash Deal", output);
            StringAssert.Contains("Limited time offer: 30% off!", output);
        }

        private string CaptureConsoleOutput(Action action)
        {
            consoleOutput.GetStringBuilder().Clear(); // Clear previous output
            action.Invoke();
            return consoleOutput.ToString();
        }
    }
}
