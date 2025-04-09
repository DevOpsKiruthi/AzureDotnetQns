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
    public class SeasonalCropManagementTests
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
                SqlCommand cmd = new SqlCommand("DELETE FROM SeasonalCrop", conn);
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
                SqlCommand cmd = new SqlCommand("DELETE FROM SeasonalCrop", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_SeasonalCrop_Class_Should_Exist()
        {
            Type CropType = typeof(SeasonalCrop);
            Assert.IsNotNull(CropType, "SeasonalCrop class should exist.");
        }

        [Test, Order(2)]
        public void Test_SeasonalCrop_Properties_Should_Exist()
        {
            Type CropType = typeof(SeasonalCrop);
            Assert.IsNotNull(CropType.GetProperty("CropID"));
            Assert.IsNotNull(CropType.GetProperty("CropName"));
            Assert.IsNotNull(CropType.GetProperty("Season"));
            Assert.IsNotNull(CropType.GetProperty("StartDate"));
            Assert.IsNotNull(CropType.GetProperty("EndDate"));
            Assert.IsNotNull(CropType.GetProperty("Description"));
        }

        [Test, Order(3)]
        public void Test_AddSeasonalCrop_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddSeasonalCrop");
            Assert.IsNotNull(method, "AddSeasonalCrop method should exist.");
        }

        [Test, Order(4)]
        public void Test_DeleteSeasonalCropBySeason_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DeleteSeasonalCropBySeason");
            Assert.IsNotNull(method, "DeleteSeasonalCropBySeason method should exist.");
        }

        [Test, Order(5)]
        public void Test_DisplayAllSeasonalCrops_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllSeasonalCrops");
            Assert.IsNotNull(method, "DisplayAllSeasonalCrops method should exist.");
        }

        [Test, Order(6)]
        public void Test_AddSeasonalCrop_Should_Insert_Into_Database()
        {
            var Crop = new SeasonalCrop
            {
                CropName = "Sunshine Farms",
                Season = "Spring",
                StartDate = "2025-03-01",
                EndDate = "2025-05-30",
                Description = "Production for spring vegetables."
            };

            MethodInfo addMethod = typeof(Program).GetMethod("AddSeasonalCrop");
            addMethod.Invoke(null, new object[] { Crop });

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM SeasonalCrop WHERE CropName = @CropName AND Season = @Season", conn);
                cmd.Parameters.AddWithValue("@CropName", "Sunshine Farms");
                cmd.Parameters.AddWithValue("@Season", "Spring");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "Crop should be inserted into the database.");
            }
        }

        [Test, Order(7)]
        public void Test_DeleteSeasonalCropBySeason_Should_Delete_Crops()
        {
            var Crop = new SeasonalCrop
            {
                CropName = "Green Fields Ltd",
                Season = "Winter",
                StartDate = "2024-11-01",
                EndDate = "2025-02-28",
                Description = "Winter produce management."
            };

            MethodInfo addMethod = typeof(Program).GetMethod("AddSeasonalCrop");
            addMethod.Invoke(null, new object[] { Crop });

            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteSeasonalCropBySeason");
            deleteMethod.Invoke(null, new object[] { "Winter" });

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SeasonalCrop WHERE Season = @Season", conn);
                cmd.Parameters.AddWithValue("@Season", "Winter");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(0, count, "Crop(s) should be deleted.");
            }
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentSeasonalCropBySeason()
        {
            MethodInfo deleteMethod = typeof(Program).GetMethod("DeleteSeasonalCropBySeason");
            deleteMethod.Invoke(null, new object[] { "Autumn" });

           string output = consoleOutput.ToString().ToLower();
           Assert.IsTrue(output.Contains("no seasonal crop found with season autumn"), "Should show not found message.");

        }

        [Test, Order(9)]
        public void Test_DisplayAllSeasonalCrops_Should_Show_Data()
        {
            var Crop = new SeasonalCrop
            {
                CropName = "Tropical Produce Inc.",
                Season = "Rainy",
                StartDate = "2025-07-01",
                EndDate = "2025-09-15",
                Description = "Rainy season production management."
            };

            MethodInfo addMethod = typeof(Program).GetMethod("AddSeasonalCrop");
            addMethod.Invoke(null, new object[] { Crop });

            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllSeasonalCrops");
            displayMethod.Invoke(null, null);

            var output = consoleOutput.ToString().ToLower();
            Assert.IsFalse(output.Contains("no seasonal crop found"), "Should show Crop data.");
            StringAssert.Contains("tropical produce inc.", output);
            StringAssert.Contains("rainy", output);
            StringAssert.Contains("2025-07-01", output);
            StringAssert.Contains("2025-09-15", output);
        }
    }
}
