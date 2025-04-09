using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using dotnetapp;
using dotnetapp.Models;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class HighwayMaintenanceTests
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
                SqlCommand cmd = new SqlCommand("DELETE FROM HighwayMaintenance", conn);
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
                SqlCommand cmd = new SqlCommand("DELETE FROM HighwayMaintenance", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_MaintenanceTask_Class_Should_Exist()
        {
            Assert.IsNotNull(typeof(MaintenanceTask), "MaintenanceTask class does not exist.");
        }

        [Test, Order(2)]
        public void Test_MaintenanceTask_Properties_Should_Exist()
        {
            var type = typeof(MaintenanceTask);

            Assert.NotNull(type.GetProperty("TaskName"), "TaskName property should exist.");
            Assert.NotNull(type.GetProperty("HighwayName"), "HighwayName property should exist.");
            Assert.NotNull(type.GetProperty("MaintenanceType"), "MaintenanceType property should exist.");
            Assert.NotNull(type.GetProperty("ContactNumber"), "ContactNumber property should exist.");
            Assert.NotNull(type.GetProperty("ScheduledDate"), "ScheduledDate property should exist.");
            Assert.NotNull(type.GetProperty("AdditionalNotes"), "AdditionalNotes property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddTask_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddTask");
            Assert.IsNotNull(method, "AddTask method should exist.");
        }

        [Test, Order(4)]
        public void Test_AddTask_Should_Insert_Record()
        {
            MaintenanceTask task = new MaintenanceTask
            {
                TaskName = "Flower Bed Replanting",
                HighwayName = "NH-101",
                MaintenanceType = "Horticulture",
                ContactNumber = "9345678901",
                ScheduledDate = "2025-04-22",
                AdditionalNotes = "Replant seasonal flowers in central median"
            };

            Program.AddTask(task);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM HighwayMaintenance WHERE TaskName = @TaskName", conn);
                cmd.Parameters.AddWithValue("@TaskName", "Flower Bed Replanting");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "Task was not inserted correctly.");
            }
        }

        [Test, Order(5)]
        public void Test_UpdateTask_Method_Exists()
        {
            var method = typeof(Program).GetMethod("UpdateTask");
            Assert.IsNotNull(method, "UpdateTask method should exist.");
        }

        [Test, Order(6)]
        public void Test_UpdateTask_Should_Modify_Record()
        {
            MaintenanceTask task = new MaintenanceTask
            {
                TaskName = "Tree Pruning",
                HighwayName = "NH-204",
                MaintenanceType = "Arborist Service",
                ContactNumber = "9876543211",
                ScheduledDate = "2025-04-15",
                AdditionalNotes = "Trim overgrown branches along the divider"
            };

            Program.AddTask(task);

            int taskId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TaskID FROM HighwayMaintenance WHERE TaskName = @TaskName", conn);
                cmd.Parameters.AddWithValue("@TaskName", "Tree Pruning");
                taskId = (int)cmd.ExecuteScalar();
            }

            MaintenanceTask updated = new MaintenanceTask
            {
                TaskName = "Grass Cutting",
                HighwayName = "NH-208",
                MaintenanceType = "Lawn Maintenance",
                ContactNumber = "9123456780",
                ScheduledDate = "2025-04-18",
                AdditionalNotes = "Mow grass along both shoulders"
            };

            Program.UpdateTask(taskId, updated);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TaskName FROM HighwayMaintenance WHERE TaskID = @TaskID", conn);
                cmd.Parameters.AddWithValue("@TaskID", taskId);
                string updatedName = (string)cmd.ExecuteScalar();

                Assert.AreEqual("Grass Cutting", updatedName, "Task was not updated correctly.");
            }
        }

        [Test, Order(7)]
        public void Test_DisplayAllTasks_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllTasks");
            Assert.IsNotNull(method, "DisplayAllTasks method should exist.");
        }

        [Test, Order(8)]
        public void Test_DisplayAllTasks_Should_Show_Tasks()
        {
            MaintenanceTask task = new MaintenanceTask
            {
                TaskName = "Decorative Plant Installation",
                HighwayName = "NH-120",
                MaintenanceType = "Landscape Installation",
                ContactNumber = "9012345678",
                ScheduledDate = "2025-05-01",
                AdditionalNotes = "Install decorative shrubs and hedges"
            };

            Program.AddTask(task);

            consoleOutput.GetStringBuilder().Clear();
            Program.DisplayAllTasks();
            string output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("Decorative Plant Installation"), "DisplayAllTasks did not output the expected task.");
            Assert.IsTrue(output.Contains("Landscape Installation"), "DisplayAllTasks did not include the MaintenanceType.");
            Assert.IsTrue(output.Contains("NH-120"), "DisplayAllTasks did not include the HighwayName.");
        }
    }
}
