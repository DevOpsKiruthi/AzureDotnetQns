using System;
using System.Data;
using System.Data.SqlClient;
using dotnetapp.Models;

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
                Console.WriteLine("\nEMI Management System Menu");
                Console.WriteLine("1. Add EMI Record");
                Console.WriteLine("2. Delete EMI Record by Loan Type");
                Console.WriteLine("3. Display All EMI Records");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Borrower Name: ");
                        string borrower = Console.ReadLine();

                        Console.Write("Enter Loan Type (e.g., Home Loan, Car Loan): ");
                        string loanType = Console.ReadLine();

                        Console.Write("Enter Start Date (yyyy-MM-dd): ");
                        string startDate = Console.ReadLine();

                        Console.Write("Enter End Date (yyyy-MM-dd): ");
                        string endDate = Console.ReadLine();

                        Console.Write("Enter EMI Amount: ");
                        string amount = Console.ReadLine();

                        Console.Write("Enter Description (e.g., Active, Paid): ");
                        string desc = Console.ReadLine();

                        EMIDetails emi = new EMIDetails
                        {
                            BorrowerName = borrower,
                            LoanType = loanType,
                            StartDate = startDate,
                            EndDate = endDate,
                            EMIAmount = amount,
                            Description = desc
                        };

                        AddEMIDetails(emi);
                        break;

                    case "2":
                        Console.Write("Enter Loan Type to delete: ");
                        string deleteLoanType = Console.ReadLine();
                        DeleteEMIByLoanType(deleteLoanType);
                        break;

                    case "3":
                        DisplayAllEMIs();
                        break;

                    case "4":
                        Console.WriteLine("Exiting the application...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }

        public static void AddEMIDetails(EMIDetails emi)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM EMIDetails", connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "EMIDetails");

                DataTable table = ds.Tables["EMIDetails"];
                DataRow newRow = table.NewRow();

                newRow["BorrowerName"] = emi.BorrowerName;
                newRow["LoanType"] = emi.LoanType;
                newRow["StartDate"] = emi.StartDate;
                newRow["EndDate"] = emi.EndDate;
                newRow["EMIAmount"] = emi.EMIAmount;
                newRow["Description"] = emi.Description;

                table.Rows.Add(newRow);

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(ds, "EMIDetails");

                Console.WriteLine("EMI record added successfully.");
            }
        }

        public static void DeleteEMIByLoanType(string loanType)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM EMIDetails WHERE LoanType = @LoanType", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@LoanType", loanType);

                DataSet ds = new DataSet();
                adapter.Fill(ds, "EMIDetails");

                DataTable table = ds.Tables["EMIDetails"];

                if (table.Rows.Count == 0)
                {
                    Console.WriteLine($"No EMI records found with loan type {loanType}");
                    return;
                }

                foreach (DataRow row in table.Rows)
                {
                    row.Delete();
                }

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(ds, "EMIDetails");

                Console.WriteLine("EMI record(s) deleted successfully.");
            }
        }

        public static void DisplayAllEMIs()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM EMIDetails", connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "EMIDetails");

                DataTable table = ds.Tables["EMIDetails"];

                if (table.Rows.Count == 0)
                {
                    Console.WriteLine("No EMI records found.");
                    return;
                }

                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"EMIID: {row["EMIID"]}\tBorrower: {row["BorrowerName"]}\tLoan Type: {row["LoanType"]}\tStartDate: {row["StartDate"]}\tEndDate: {row["EndDate"]}\tEMI Amount: {row["EMIAmount"]}\tDescription: {row["Description"]}");
                }
            }
        }
    }
}
