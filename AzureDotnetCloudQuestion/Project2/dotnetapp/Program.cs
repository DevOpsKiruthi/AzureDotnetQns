using dotnetapp.Models;
using System;
using System.Data;
using System.Data.SqlClient;

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
                Console.WriteLine("\nWorshipper Booking Menu ");
                Console.WriteLine("1. Add Booking");
                Console.WriteLine("2. Update Booking Information");
                Console.WriteLine("3. Display All Bookings");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        WorshipperBooking newBooking = new WorshipperBooking
                        {
                            WorshipperName = ReadInput("Enter Worshipper Name: "),
                            ReligiousInstitutionName = ReadInput("Enter Religious Institution Name (Temple/Church/Mosque): "),
                            ServiceType = ReadInput("Enter Service Type (Prayer/Spiritual Discourse/Festival Event): "),
                            ContactNumber = ReadInput("Enter Contact Number: "),
                            BookingDate = ReadInput("Enter Booking Date (yyyy-MM-dd): "),
                            SpecialRequest = ReadInput("Enter any Special Request (optional): ")
                        };
                        AddBooking(newBooking);
                        break;

                    case "2":
                        try
                        {
                            int updateBookingID = int.Parse(ReadInput("Enter the Booking ID to update: "));
                            WorshipperBooking updatedBooking = new WorshipperBooking
                            {
                                WorshipperName = ReadInput("Enter the new Worshipper Name: "),
                                ReligiousInstitutionName = ReadInput("Enter the new Religious Institution Name: "),
                                ServiceType = ReadInput("Enter the new Service Type: "),
                                ContactNumber = ReadInput("Enter the new Contact Number: "),
                                BookingDate = ReadInput("Enter the new Booking Date (yyyy-MM-dd): "),
                                SpecialRequest = ReadInput("Enter any Special Request (optional): ")
                            };

                            // Call IsUpdateSuccessful method instead of UpdateBooking directly
                            bool isSuccess = IsUpdateSuccessful(updateBookingID, updatedBooking);
                            Console.WriteLine(isSuccess ? $"Booking ID {updateBookingID} updated successfully." : $"No booking found with ID {updateBookingID}. Update failed.");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input. Booking ID must be a number.");
                        }
                        break;

                    case "3":
                        DisplayAllBookings();
                        break;

                    case "4":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        // Method to add a booking
        public static void AddBooking(WorshipperBooking bookingObj)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Worship (WorshipperName, ReligiousInstitutionName, ServiceType, ContactNumber, BookingDate, SpecialRequest) VALUES (@WorshipperName, @ReligiousInstitutionName, @ServiceType, @ContactNumber, @BookingDate, @SpecialRequest)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@WorshipperName", bookingObj.WorshipperName);
                    command.Parameters.AddWithValue("@ReligiousInstitutionName", bookingObj.ReligiousInstitutionName);
                    command.Parameters.AddWithValue("@ServiceType", bookingObj.ServiceType);
                    command.Parameters.AddWithValue("@ContactNumber", bookingObj.ContactNumber);
                    command.Parameters.AddWithValue("@BookingDate", bookingObj.BookingDate);
                    command.Parameters.AddWithValue("@SpecialRequest", bookingObj.SpecialRequest ?? "");

                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("Booking added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }

        // Method to update a booking
        public static string UpdateBooking(int bookingID, WorshipperBooking updatedBooking)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Worship SET WorshipperName = @WorshipperName, ReligiousInstitutionName = @ReligiousInstitutionName, ServiceType = @ServiceType, ContactNumber = @ContactNumber, BookingDate = @BookingDate, SpecialRequest = @SpecialRequest WHERE BookingID = @BookingID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookingID", bookingID);
                    command.Parameters.AddWithValue("@WorshipperName", updatedBooking.WorshipperName);
                    command.Parameters.AddWithValue("@ReligiousInstitutionName", updatedBooking.ReligiousInstitutionName);
                    command.Parameters.AddWithValue("@ServiceType", updatedBooking.ServiceType);
                    command.Parameters.AddWithValue("@ContactNumber", updatedBooking.ContactNumber);
                    command.Parameters.AddWithValue("@BookingDate", updatedBooking.BookingDate);
                    command.Parameters.AddWithValue("@SpecialRequest", updatedBooking.SpecialRequest ?? "");

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0
                        ? $"Booking ID {bookingID} updated successfully."
                        : $"No booking found with ID {bookingID}. Update failed.";
                }
            }
            catch (Exception ex)
            {
                return $"Error updating booking: {ex.Message}";
            }
        }

        // Method to check if the update was successful
        public static bool IsUpdateSuccessful(int bookingID, WorshipperBooking updatedBooking)
        {
            return UpdateBooking(bookingID, updatedBooking).Contains("updated successfully");
        }

        // Method to display all bookings
        public static void DisplayAllBookings()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM Worship", connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    Console.WriteLine("\n All Bookings ");
                    bool hasData = false;
                    while (reader.Read())
                    {
                        Console.WriteLine($"BookingID: {reader["BookingID"]}, Name: {reader["WorshipperName"]}, Institution: {reader["ReligiousInstitutionName"]}, Type: {reader["ServiceType"]}, Contact: {reader["ContactNumber"]}, Date: {reader["BookingDate"]}, Special Request: {reader["SpecialRequest"]}");
                        hasData = true;
                    }

                    if (!hasData)
                    {
                        Console.WriteLine("No bookings found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Method to read user input
        private static string ReadInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
