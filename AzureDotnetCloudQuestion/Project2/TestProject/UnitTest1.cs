using System;
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
    public class WorshipperBookingTests
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
                SqlCommand cmd = new SqlCommand("DELETE FROM Worship", conn);
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
                SqlCommand cmd = new SqlCommand("DELETE FROM Worship", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_WorshipperBooking_Class_Should_Exist()
        {
            Type worshipperType = typeof(WorshipperBooking);
            Assert.NotNull(worshipperType, "WorshipperBooking class does not exist.");
        }

        [Test, Order(2)]
        public void Test_WorshipperBooking_Properties_Should_Exist()
        {
            Type worshipperType = typeof(WorshipperBooking);

            Assert.NotNull(worshipperType.GetProperty("WorshipperName"), "WorshipperName property should exist.");
            Assert.NotNull(worshipperType.GetProperty("ReligiousInstitutionName"), "ReligiousInstitutionName property should exist.");
            Assert.NotNull(worshipperType.GetProperty("ServiceType"), "ServiceType property should exist.");
            Assert.NotNull(worshipperType.GetProperty("ContactNumber"), "ContactNumber property should exist.");
            Assert.NotNull(worshipperType.GetProperty("BookingDate"), "BookingDate property should exist.");
            Assert.NotNull(worshipperType.GetProperty("SpecialRequest"), "SpecialRequest property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddBooking_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddBooking");
            Assert.IsNotNull(method, "AddBooking method should exist.");
        }

        [Test, Order(4)]
        public void Test_AddBooking_Should_Insert_Record()
        {
            WorshipperBooking booking = new WorshipperBooking
            {
                WorshipperName = "John Doe",
                ReligiousInstitutionName = "First Temple",
                ServiceType = "Prayer",
                ContactNumber = "1234567890",
                BookingDate = "2025-02-14",
                SpecialRequest = "None"
            };

            Program.AddBooking(booking);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Worship WHERE WorshipperName = @WorshipperName", conn);
                cmd.Parameters.AddWithValue("@WorshipperName", "John Doe");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "Booking was not inserted correctly.");
            }
        }

        [Test, Order(5)]
        public void Test_UpdateBooking_Method_Exists()
        {
            var method = typeof(Program).GetMethod("UpdateBooking");
            Assert.IsNotNull(method, "UpdateBooking method should exist.");
        }

        [Test, Order(6)]
        public void Test_UpdateBooking_Should_Modify_Record()
        {
            WorshipperBooking booking = new WorshipperBooking
            {
                WorshipperName = "John Doe",
                ReligiousInstitutionName = "First Temple",
                ServiceType = "Prayer",
                ContactNumber = "1234567890",
                BookingDate = "2025-02-14",
                SpecialRequest = "None"
            };

            Program.AddBooking(booking);

            int bookingID;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT BookingID FROM Worship WHERE WorshipperName = @WorshipperName", conn);
                cmd.Parameters.AddWithValue("@WorshipperName", "John Doe");
                bookingID = (int)cmd.ExecuteScalar();
            }

            WorshipperBooking updatedBooking = new WorshipperBooking
            {
                WorshipperName = "Jane Doe",
                ReligiousInstitutionName = "Second Church",
                ServiceType = "Festival Event",
                ContactNumber = "0987654321",
                BookingDate = "2025-03-01",
                SpecialRequest = "Wheelchair assistance"
            };

            Program.UpdateBooking(bookingID, updatedBooking);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT WorshipperName, ReligiousInstitutionName, ServiceType FROM Worship WHERE BookingID = @BookingID", conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Assert.AreEqual("Jane Doe", reader.GetString(0), "WorshipperName was not updated correctly.");
                        Assert.AreEqual("Second Church", reader.GetString(1), "ReligiousInstitutionName was not updated correctly.");
                        Assert.AreEqual("Festival Event", reader.GetString(2), "ServiceType was not updated correctly.");
                    }
                    else
                    {
                        Assert.Fail("Updated booking not found.");
                    }
                }
            }
        }

        [Test, Order(7)]
        public void Test_DisplayAllBookings_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllBookings");
            Assert.IsNotNull(method, "DisplayAllBookings method should exist.");
        }

        [Test, Order(8)]
        public void Test_DisplayAllBookings_Should_Show_Bookings()
        {
            WorshipperBooking booking = new WorshipperBooking
            {
                WorshipperName = "John Doe",
                ReligiousInstitutionName = "First Temple",
                ServiceType = "Prayer",
                ContactNumber = "1234567890",
                BookingDate = "2025-02-14",
                SpecialRequest = "None"
            };

            Program.AddBooking(booking);

            consoleOutput.GetStringBuilder().Clear();
            Program.DisplayAllBookings();
            string output = consoleOutput.ToString();

            Assert.IsTrue(output.Contains("John Doe"), "DisplayAllBookings did not output the expected booking.");
            Assert.IsTrue(output.Contains("Prayer"), "DisplayAllBookings did not include the ServiceType.");
            Assert.IsTrue(output.Contains("First Temple"), "DisplayAllBookings did not include the ReligiousInstitutionName.");
        }

        [Test, Order(9)]
        public void Test_UpdateNonExistentWorshipper()
        {
            WorshipperBooking updatedBooking = new WorshipperBooking
            {
                WorshipperName = "Non Existent",
                ReligiousInstitutionName = "Non Existing Institution",
                ServiceType = "Special Worship",
                ContactNumber = "0000000000",
                BookingDate = "2025-12-25",
                SpecialRequest = "None"
            };

            int nonExistentBookingID = 99999; // Assuming this ID does not exist

            bool result = Program.IsUpdateSuccessful(nonExistentBookingID, updatedBooking);

            Assert.IsFalse(result, "Updating a non-existent booking should return false.");
        }
    }
}
