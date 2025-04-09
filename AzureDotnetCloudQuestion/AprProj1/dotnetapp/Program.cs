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
                Console.WriteLine("\nHighway HighwayMaintenance Task Menu");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. Display All Tasks");
                Console.WriteLine("3. Update Task by ID");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        MaintenanceTask task = new MaintenanceTask
                        {
                            TaskName = ReadInput("Enter Task Name: "),
                            HighwayName = ReadInput("Enter Highway Name: "),
                            MaintenanceType = ReadInput("Enter HighwayMaintenance Type: "),
                            ContactNumber = ReadInput("Enter Contact Number: "),
                            ScheduledDate = ReadInput("Enter Scheduled Date: "),
                            AdditionalNotes = ReadInput("Enter Additional Notes: ")
                        };
                        AddTask(task);
                        break;

                    case "2":
                        DisplayAllTasks();
                        break;

                    case "3":
                        int taskId = int.Parse(ReadInput("Enter Task ID to update: "));
                        MaintenanceTask updatedTask = new MaintenanceTask
                        {
                            TaskName = ReadInput("Enter Task Name: "),
                            HighwayName = ReadInput("Enter Highway Name: "),
                            MaintenanceType = ReadInput("Enter HighwayMaintenance Type: "),
                            ContactNumber = ReadInput("Enter Contact Number: "),
                            ScheduledDate = ReadInput("Enter Scheduled Date: "),
                            AdditionalNotes = ReadInput("Enter Additional Notes: ")
                        };
                        UpdateTask(taskId, updatedTask);
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

        public static void AddTask(MaintenanceTask task)
        {
            using var adapter = new SqlDataAdapter("SELECT * FROM HighwayMaintenance", connectionString);
            using var builder = new SqlCommandBuilder(adapter);
            var dataset = new DataSet();
            adapter.Fill(dataset, "HighwayMaintenance");
            var table = dataset.Tables["HighwayMaintenance"];
            var newRow = table.NewRow();

            newRow["TaskName"] = task.TaskName;
            newRow["HighwayName"] = task.HighwayName;
            newRow["MaintenanceType"] = task.MaintenanceType;
            newRow["ContactNumber"] = task.ContactNumber;
            newRow["ScheduledDate"] = task.ScheduledDate;
            newRow["AdditionalNotes"] = task.AdditionalNotes;

            table.Rows.Add(newRow);
            adapter.Update(dataset, "HighwayMaintenance");
            Console.WriteLine("Task added successfully.");
        }

        public static void DisplayAllTasks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM HighwayMaintenance", conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "HighwayMaintenance");

                if (ds.Tables["HighwayMaintenance"].Rows.Count > 0)
                {
                    Console.WriteLine("\nMaintenance Tasks:");
                    foreach (DataRow row in ds.Tables["HighwayMaintenance"].Rows)
                    {
                        Console.WriteLine($"TaskID: {row["TaskID"]}, TaskName: {row["TaskName"]}, HighwayName: {row["HighwayName"]}, MaintenanceType: {row["MaintenanceType"]}, ContactNumber: {row["ContactNumber"]}, ScheduledDate: {row["ScheduledDate"]}, AdditionalNotes: {row["AdditionalNotes"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No tasks found.");
                }
            }
        }

        public static void UpdateTask(int taskID, MaintenanceTask task)
        {
            using var adapter = new SqlDataAdapter("SELECT * FROM HighwayMaintenance", connectionString);
            using var builder = new SqlCommandBuilder(adapter);
            var dataset = new DataSet();
            adapter.Fill(dataset, "HighwayMaintenance");
            var table = dataset.Tables["HighwayMaintenance"];
            var rows = table.Select($"TaskID = {taskID}");

            if (rows.Length > 0)
            {
                var row = rows[0];
                row["TaskName"] = task.TaskName;
                row["HighwayName"] = task.HighwayName;
                row["MaintenanceType"] = task.MaintenanceType;
                row["ContactNumber"] = task.ContactNumber;
                row["ScheduledDate"] = task.ScheduledDate;
                row["AdditionalNotes"] = task.AdditionalNotes;

                adapter.Update(dataset, "HighwayMaintenance");
                Console.WriteLine("Task updated successfully.");
            }
            else
            {
                Console.WriteLine($"No task found with ID {taskID}.");
            }
        }
    }
}
