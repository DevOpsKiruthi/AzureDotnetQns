using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using dotnetapp;
using dotnetapp.Models;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class SpeakerSlotTests
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
                SqlCommand cmd = new SqlCommand("DELETE FROM SpeakerSlot", conn);
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
                SqlCommand cmd = new SqlCommand("DELETE FROM SpeakerSlot", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_SpeakerSlot_Class_Should_Exist()
        {
            Assert.IsNotNull(typeof(SpeakerSlot), "SpeakerSlot class does not exist.");
        }

        [Test, Order(2)]
        public void Test_SpeakerSlot_Properties_Should_Exist()
        {
            var type = typeof(SpeakerSlot);

            Assert.NotNull(type.GetProperty("SpeakerName"), "SpeakerName property should exist.");
            Assert.NotNull(type.GetProperty("TalkTitle"), "TalkTitle property should exist.");
            Assert.NotNull(type.GetProperty("TalkCategory"), "TalkCategory property should exist.");
            Assert.NotNull(type.GetProperty("ContactEmail"), "ContactEmail property should exist.");
            Assert.NotNull(type.GetProperty("SlotDate"), "SlotDate property should exist.");
            Assert.NotNull(type.GetProperty("TalkSummary"), "TalkSummary property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddSlot_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddSlot");
            Assert.IsNotNull(method, "AddSlot method should exist.");
        }

        [Test, Order(4)]
        public void Test_AddSlot_Should_Insert_Record()
        {
            SpeakerSlot slot = new SpeakerSlot
            {
                SpeakerName = "Dr. Maya Rao",
                TalkTitle = "AI and Ethics",
                TalkCategory = "Technology",
                ContactEmail = "maya.rao@example.com",
                SlotDate = "2025-05-01",
                TalkSummary = "A discussion on responsible AI practices"
            };

            Program.AddSlot(slot);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SpeakerSlot WHERE TalkTitle = @TalkTitle", conn);
                cmd.Parameters.AddWithValue("@TalkTitle", "AI and Ethics");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "Slot was not inserted correctly.");
            }
        }

        [Test, Order(5)]
        public void Test_UpdateSlot_Method_Exists()
        {
            var method = typeof(Program).GetMethod("UpdateSlot");
            Assert.IsNotNull(method, "UpdateSlot method should exist.");
        }

        [Test, Order(6)]
        public void Test_UpdateSlot_Should_Modify_Record()
        {
            SpeakerSlot slot = new SpeakerSlot
            {
                SpeakerName = "John Doe",
                TalkTitle = "Green Cities",
                TalkCategory = "Environment",
                ContactEmail = "john.doe@example.com",
                SlotDate = "2025-05-10",
                TalkSummary = "Urban sustainability challenges"
            };

            Program.AddSlot(slot);

            int slotId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT SlotID FROM SpeakerSlot WHERE TalkTitle = @TalkTitle", conn);
                cmd.Parameters.AddWithValue("@TalkTitle", "Green Cities");
                slotId = (int)cmd.ExecuteScalar();
            }

            SpeakerSlot updated = new SpeakerSlot
            {
                SpeakerName = "John Doe",
                TalkTitle = "Smart Cities",
                TalkCategory = "Technology",
                ContactEmail = "john.doe@example.com",
                SlotDate = "2025-05-12",
                TalkSummary = "Innovations in city infrastructure"
            };

            Program.UpdateSlot(slotId, updated);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TalkTitle FROM SpeakerSlot WHERE SlotID = @SlotID", conn);
                cmd.Parameters.AddWithValue("@SlotID", slotId);
                string updatedTitle = (string)cmd.ExecuteScalar();

                Assert.AreEqual("Smart Cities", updatedTitle, "Slot was not updated correctly.");
            }
        }

        [Test, Order(7)]
        public void Test_DisplayAllSlots_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllSlots");
            Assert.IsNotNull(method, "DisplayAllSlots method should exist.");
        }

        [Test, Order(8)]
        public void Test_DisplayAllSlots_Should_Show_Slots()
        {
            SpeakerSlot slot = new SpeakerSlot
            {
                SpeakerName = "Anita Sharma",
                TalkTitle = "Empowering Youth",
                TalkCategory = "Motivation",
                ContactEmail = "anita.sharma@example.com",
                SlotDate = "2025-06-05",
                TalkSummary = "Helping young minds build confidence"
            };

            Program.AddSlot(slot);

            consoleOutput.GetStringBuilder().Clear();
            Program.DisplayAllSlots();
            string output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("Empowering Youth"), "DisplayAllSlots did not output the expected slot.");
            Assert.IsTrue(output.Contains("Motivation"), "DisplayAllSlots did not include the TalkCategory.");
            Assert.IsTrue(output.Contains("Anita Sharma"), "DisplayAllSlots did not include the SpeakerName.");
        }
    }
}
