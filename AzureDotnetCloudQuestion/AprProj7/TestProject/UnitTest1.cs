using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using dotnetapp.Models;
using NUnit.Framework;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class PetrolProductTests
    {
        private string connectionString = "User ID=sa;password=examlyMssql@123; server=localhost;Database=appdb;trusted_connection=false;Persist Security Info=False;Encrypt=False";
        private StringWriter consoleOutput;

        [SetUp]
        public void SetUp()
        {
            ClearDatabase();
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            ClearDatabase();
            consoleOutput.Dispose();
        }

        private void ClearDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PetrolProducts";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_PetrolProduct_Class_Should_Exist()
        {
            Type type = typeof(PetrolProduct);
            Assert.IsNotNull(type, "PetrolProduct class should exist.");
        }

        [Test, Order(2)]
        public void Test_PetrolProduct_Properties_Should_Exist()
        {
            Type type = typeof(PetrolProduct);

            Assert.IsNotNull(type.GetProperty("ProductName"));
            Assert.IsNotNull(type.GetProperty("QuantityAvailable"));
            Assert.IsNotNull(type.GetProperty("UnitPrice"));
            Assert.IsNotNull(type.GetProperty("SupplierContact"));
            Assert.IsNotNull(type.GetProperty("LastRestocked"));
            Assert.IsNotNull(type.GetProperty("AdditionalNotes"));
        }

        [Test, Order(3)]
        public void Test_AddProduct_Method_Exists()
        {
            MethodInfo method = typeof(dotnetapp.Program).GetMethod("AddProduct");
            Assert.IsNotNull(method, "AddProduct method should exist in Program class.");
        }

        [Test, Order(4)]
        public void Test_DisplayAllProducts_Method_Exists()
        {
            MethodInfo method = typeof(dotnetapp.Program).GetMethod("DisplayAllProducts");
            Assert.IsNotNull(method, "DisplayAllProducts method should exist in Program class.");
        }

        [Test, Order(5)]
        public void Test_AddProduct_Should_Insert_Into_Database()
        {
            var product = new PetrolProduct
            {
                ProductName = "Speed Petrol",
                QuantityAvailable = "1000L",
                UnitPrice = "120.50",
                SupplierContact = "9876543210",
                LastRestocked = DateTime.Now.ToString("yyyy-MM-dd"),
                AdditionalNotes = "High Octane"
            };

            MethodInfo method = typeof(dotnetapp.Program).GetMethod("AddProduct");
            method.Invoke(null, new object[] { product });

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM PetrolProducts WHERE ProductName = 'Speed Petrol'";
                SqlCommand cmd = new SqlCommand(query, connection);
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "Product should be inserted into the database.");
            }
        }

        [Test, Order(6)]
        public void Test_DisplayAllProducts_Should_Show_Data()
        {
            var product = new PetrolProduct
            {
                ProductName = "Premium Petrol",
                QuantityAvailable = "500L",
                UnitPrice = "140.00",
                SupplierContact = "9876543210",
                LastRestocked = DateTime.Now.ToString("yyyy-MM-dd"),
                AdditionalNotes = "Premium Quality"
            };

            MethodInfo addMethod = typeof(dotnetapp.Program).GetMethod("AddProduct");
            addMethod.Invoke(null, new object[] { product });

            MethodInfo displayMethod = typeof(dotnetapp.Program).GetMethod("DisplayAllProducts");
            displayMethod.Invoke(null, null);

            string output = consoleOutput.ToString();
            Assert.IsTrue(output.Contains("Premium Petrol"), "DisplayAllProducts should print the product details.");
        }

        [Test, Order(7)]
        public void Test_UpdateProduct_Method_Exists()
        {
            // UPDATED: If method is in Program class
            MethodInfo method = typeof(dotnetapp.Program).GetMethod("UpdateProduct");
            Assert.IsNotNull(method, "UpdateProduct method should exist in Program class.");
        }

        [Test, Order(8)]
        public void Test_UpdateProduct_Should_Modify_Record()
        {
            var product = new PetrolProduct
            {
                ProductName = "Regular Petrol",
                QuantityAvailable = "800L",
                UnitPrice = "100.00",
                SupplierContact = "9876543210",
                LastRestocked = DateTime.Now.ToString("yyyy-MM-dd"),
                AdditionalNotes = "Regular Quality"
            };

            MethodInfo addMethod = typeof(dotnetapp.Program).GetMethod("AddProduct");
            addMethod.Invoke(null, new object[] { product });

            // Invoke UpdateProduct(ProductName, QuantityAvailable, UnitPrice, AdditionalNotes)
            MethodInfo updateMethod = typeof(dotnetapp.Program).GetMethod("UpdateProduct");
            Assert.IsNotNull(updateMethod, "UpdateProduct method should exist in Program class.");
            updateMethod.Invoke(null, new object[] { "Regular Petrol", "600L", "95.00", "Updated Quality" });

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT QuantityAvailable, UnitPrice, AdditionalNotes FROM PetrolProducts WHERE ProductName = 'Regular Petrol'";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                Assert.IsTrue(reader.Read(), "Product should exist.");
                Assert.AreEqual("600L", reader.GetString(0));
                Assert.AreEqual("95.00", reader.GetString(1));
                Assert.AreEqual("Updated Quality", reader.GetString(2));
            }
        }
    }
}
