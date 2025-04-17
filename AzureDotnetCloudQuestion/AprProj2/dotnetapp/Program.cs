using System;
using System.Data;
using System.Data.SqlClient;
using dotnetapp.Models;

namespace dotnetapp
{
    public static class ConnectionStringProvider
    {
        public static string ConnectionString { get; } = "User ID=sa;password=examlyMssql@123;server=localhost;Database=appdb;trusted_connection=false;Persist Security Info=False;Encrypt=False";
    }

    public class Program
    {
        static string connectionString = ConnectionStringProvider.ConnectionString;

        public static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\nPublic Speakers Forum Slot Booking Menu");
                Console.WriteLine("1. Submit Talk Proposal");
                Console.WriteLine("2. View All Slot Requests");
                Console.WriteLine("3. Update Slot");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        SpeakerSlot newSlot = new SpeakerSlot
                        {
                            SpeakerName = ReadInput("Enter Speaker Name: "),
                            TalkTitle = ReadInput("Enter Talk Title: "),
                            TalkCategory = ReadInput("Enter Talk Category: "),
                            ContactEmail = ReadInput("Enter Contact Email: "),
                            SlotDate = ReadInput("Enter Slot Date (yyyy-mm-dd): "),
                            TalkSummary = ReadInput("Enter Talk Summary: ")
                        };
                        AddSlot(newSlot);
                        break;

                    case "2":
                        DisplayAllSlots();
                        break;

                    case "3":
                        int slotID = int.Parse(ReadInput("Enter Slot ID to update: "));
                        SpeakerSlot updatedSlot = new SpeakerSlot
                        {
                            SpeakerName = ReadInput("Enter Speaker Name: "),
                            TalkTitle = ReadInput("Enter Talk Title: "),
                            TalkCategory = ReadInput("Enter Talk Category: "),
                            ContactEmail = ReadInput("Enter Contact Email: "),
                            SlotDate = ReadInput("Enter Slot Date (yyyy-mm-dd): "),
                            TalkSummary = ReadInput("Enter Talk Summary: ")
                        };
                        UpdateSlot(slotID, updatedSlot);
                        break;

                    case "4":
                        Console.WriteLine("Exiting the application...");
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static string ReadInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public static void AddSlot(SpeakerSlot slot)
        {
            using var adapter = new SqlDataAdapter("SELECT * FROM SpeakerSlot", connectionString);
            using var builder = new SqlCommandBuilder(adapter);
            var dataset = new DataSet();
            adapter.Fill(dataset, "SpeakerSlot");
            var table = dataset.Tables["SpeakerSlot"];
            var newRow = table.NewRow();

            newRow["SpeakerName"] = slot.SpeakerName;
            newRow["TalkTitle"] = slot.TalkTitle;
            newRow["TalkCategory"] = slot.TalkCategory;
            newRow["ContactEmail"] = slot.ContactEmail;
            newRow["SlotDate"] = slot.SlotDate;
            newRow["TalkSummary"] = slot.TalkSummary;

            table.Rows.Add(newRow);
            adapter.Update(dataset, "SpeakerSlot");
            Console.WriteLine("Slot request submitted successfully.");
        }

        public static void DisplayAllSlots()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM SpeakerSlot", conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "SpeakerSlot");

                if (ds.Tables["SpeakerSlot"].Rows.Count > 0)
                {
                    Console.WriteLine("\nSlot Requests:");
                    foreach (DataRow row in ds.Tables["SpeakerSlot"].Rows)
                    {
                        Console.WriteLine($"SlotID: {row["SlotID"]}, SpeakerName: {row["SpeakerName"]}, TalkTitle: {row["TalkTitle"]}, TalkCategory: {row["TalkCategory"]}, ContactEmail: {row["ContactEmail"]}, SlotDate: {row["SlotDate"]}, TalkSummary: {row["TalkSummary"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No slots found.");
                }
            }
        }

        public static void UpdateSlot(int slotID, SpeakerSlot slot)
        {
            using var adapter = new SqlDataAdapter("SELECT * FROM SpeakerSlot", connectionString);
            using var builder = new SqlCommandBuilder(adapter);
            var dataset = new DataSet();
            adapter.Fill(dataset, "SpeakerSlot");
            var table = dataset.Tables["SpeakerSlot"];
            var rows = table.Select($"SlotID = {slotID}");

            if (rows.Length > 0)
            {
                var row = rows[0];
                row["SpeakerName"] = slot.SpeakerName;
                row["TalkTitle"] = slot.TalkTitle;
                row["TalkCategory"] = slot.TalkCategory;
                row["ContactEmail"] = slot.ContactEmail;
                row["SlotDate"] = slot.SlotDate;
                row["TalkSummary"] = slot.TalkSummary;

                adapter.Update(dataset, "SpeakerSlot");
                Console.WriteLine("Slot details updated successfully.");
            }
            else
            {
                Console.WriteLine($"No slot found with ID {slotID}.");
            }
        }
    }
}
