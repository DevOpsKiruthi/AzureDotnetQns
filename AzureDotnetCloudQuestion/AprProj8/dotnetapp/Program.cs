using System;
using System.Data;
using System.Data.SqlClient;
using dotnetapp.Models;

namespace dotnetapp
{
    public static class ConnectionStringProvider
    {
        public static string ConnectionString { get; } =
            "User ID=sa;password=examlyMssql@123; server=localhost;Database=appdb;trusted_connection=false;Persist Security Info=False;Encrypt=False";
    }

    public class Program
    {
        static string connectionString = ConnectionStringProvider.ConnectionString;

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nPetrol Product Management Menu");
                Console.WriteLine("1. Add Petrol Product");
                Console.WriteLine("2. Update Petrol Product by Name");
                Console.WriteLine("3. Display All Petrol Products");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice (1-4): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        PetrolProduct newProduct = GetProductInput();
                        AddProduct(newProduct);
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

                        UpdateProductByName(updateName, updateQty, updatePrice, updateContact, updateRestocked, updateNotes);
                        break;

                    case "3":
                        DisplayAllProducts();
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
        {
            try
            {
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

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying products: {ex.Message}");
            }
        }
    }
}
