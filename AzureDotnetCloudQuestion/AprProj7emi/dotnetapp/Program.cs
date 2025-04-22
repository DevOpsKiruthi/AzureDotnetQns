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

                switch (Console.ReadLine())
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
                        Console.Write("Enter Product Name to update: ");
                        string updateName = Console.ReadLine();

                        Console.Write("Enter new Quantity Available: ");
                        string updateQty = Console.ReadLine();

                        Console.Write("Enter new Unit Price: ");
                        string updatePrice = Console.ReadLine();

                        Console.Write("Enter new Supplier Contact: ");
                        string updateContact = Console.ReadLine();

                        Console.Write("Enter new Last Restocked Date (yyyy-MM-dd): ");
                        string updateRestocked = Console.ReadLine();

                        Console.Write("Enter new Additional Notes: ");
                        string updateNotes = Console.ReadLine();

<<<<<<< HEAD
                    case "2":
                        Console.Write("Enter Loan Type to delete: ");
                        string deleteLoanType = Console.ReadLine();
                        DeleteEMIByLoanType(deleteLoanType);
                        break;

                    case "3":
                        DisplayAllEMIs();
=======
                        UpdateProductByName(updateName, updateQty, updatePrice, updateContact, updateRestocked, updateNotes);
                        break;

                    case "3":
                        DisplayAllProducts();
>>>>>>> aec3332de891ec0d4e208581f071a751bdc3131f
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

<<<<<<< HEAD
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
=======
        private static PetrolProduct GetProductInput()
        {
            Console.Write("Enter Product Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Quantity Available: ");
            string qty = Console.ReadLine();

            Console.Write("Enter Unit Price: ");
            string price = Console.ReadLine();

            Console.Write("Enter Supplier Contact: ");
            string contact = Console.ReadLine();

            Console.Write("Enter Last Restocked Date (yyyy-MM-dd): ");
            string restocked = Console.ReadLine();

            Console.Write("Enter Additional Notes: ");
            string notes = Console.ReadLine();

            return new PetrolProduct
            {
                ProductName = name,
                QuantityAvailable = qty,
                UnitPrice = price,
                SupplierContact = contact,
                LastRestocked = restocked,
                AdditionalNotes = notes
            };
        }

        public static void AddProduct(PetrolProduct product)
>>>>>>> aec3332de891ec0d4e208581f071a751bdc3131f
        {
            try
            {
<<<<<<< HEAD
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
=======
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PetrolProducts", connection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "PetrolProducts");

                    DataTable table = ds.Tables["PetrolProducts"];
                    DataRow newRow = table.NewRow();

                    newRow["ProductName"] = product.ProductName;
                    newRow["QuantityAvailable"] = product.QuantityAvailable;
                    newRow["UnitPrice"] = product.UnitPrice;
                    newRow["SupplierContact"] = product.SupplierContact;
                    newRow["LastRestocked"] = product.LastRestocked;
                    newRow["AdditionalNotes"] = product.AdditionalNotes;

                    table.Rows.Add(newRow);

                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.Update(ds, "PetrolProducts");

                    Console.WriteLine("Petrol product added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
            }
        }

            public static void UpdateProductByName(string productName, string newQuantity, string newPrice, string newContact, string newRestocked, string newNotes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PetrolProducts WHERE ProductName = @ProductName", connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ProductName", productName);

                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "PetrolProducts");

                    DataTable table = ds.Tables["PetrolProducts"];

                    if (table.Rows.Count == 0)
                    {
                        Console.WriteLine($"No petrol product found with name {productName}");
                        return;
                    }

                    DataRow row = table.Rows[0];
                    row["QuantityAvailable"] = newQuantity;
                    row["UnitPrice"] = newPrice;
                    row["SupplierContact"] = newContact;
                    row["LastRestocked"] = newRestocked;
                    row["AdditionalNotes"] = newNotes;

                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.Update(ds, "PetrolProducts");

                    Console.WriteLine("Petrol product updated successfully.");
                }
            }


        public static void DisplayAllProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM PetrolProducts", connection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "PetrolProducts");

                    DataTable table = ds.Tables["PetrolProducts"];

                    if (table.Rows.Count == 0)
                    {
                        Console.WriteLine("No petrol products found.");
                        return;
                    }

            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine($"ProductID: {row["ProductID"]} | Name: {row["ProductName"]} | Quantity: {row["QuantityAvailable"]} | Unit Price: {row["UnitPrice"]} | Supplier: {row["SupplierContact"]} | Restocked: {row["LastRestocked"]} | Notes: {row["AdditionalNotes"]}");
            }

>>>>>>> aec3332de891ec0d4e208581f071a751bdc3131f
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying products: {ex.Message}");
            }
        }
    }
}
