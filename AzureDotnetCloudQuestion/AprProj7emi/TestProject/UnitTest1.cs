using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using dotnetapp.Models;
using NUnit.Framework;

namespace dotnetapp.Tests
{
    [TestFixture]
    public class EMITests
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
                string query = "DELETE FROM EMIDetails";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddTestEMI(string borrowerName, string loanType)
        {
            EMIDetails emi = new EMIDetails
            {
                BorrowerName = borrowerName,
                LoanType = loanType,
                StartDate = "2024-05-01",
                EndDate = "2025-05-01",
                EMIAmount = "12000",
                Description = "Test EMI"
            };
            Program.AddEMIDetails(emi);
        }

        [Test, Order(1)]
        public void Test_EMIDetails_Class_Should_Exist()
        {
            Type type = typeof(EMIDetails);
            Assert.IsNotNull(type, "EMIDetails class should exist.");
        }

        [Test, Order(2)]
        public void Test_AddEMIDetails_Should_Insert_Into_Database()
        {
            AddTestEMI("John", "Home Loan");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM EMIDetails WHERE BorrowerName = 'John'";
                SqlCommand cmd = new SqlCommand(query, connection);
                int count = (int)cmd.ExecuteScalar();
                Assert.AreEqual(1, count);
            }
        }

        [Test, Order(3)]
        public void Test_DisplayAllEMIs_Should_Show_Data()
        {
            AddTestEMI("Alice", "Education Loan");

            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllEMIs");
            displayMethod.Invoke(null, null);

            string output = consoleOutput.ToString();
            Assert.IsTrue(output.Contains("Alice"), "DisplayAllEMIs should print the EMI details.");
        }

        [Test, Order(4)]
        public void Test_EMIDetails_Properties_Should_Exist()
        {
            Type type = typeof(EMIDetails);
            Assert.IsNotNull(type.GetProperty("BorrowerName"));
            Assert.IsNotNull(type.GetProperty("LoanType"));
            Assert.IsNotNull(type.GetProperty("StartDate"));
            Assert.IsNotNull(type.GetProperty("EndDate"));
            Assert.IsNotNull(type.GetProperty("EMIAmount"));
            Assert.IsNotNull(type.GetProperty("Description"));
        }

        [Test, Order(5)]
        public void Test_AddEMIDetails_Method_Should_Exist()
        {
            var method = typeof(Program).GetMethod("AddEMIDetails");
            Assert.IsNotNull(method, "AddEMIDetails method should exist in Program class.");
        }

        [Test, Order(6)]
        public void Test_DisplayAllEMIs_Method_Should_Exist()
        {
            var method = typeof(Program).GetMethod("DisplayAllEMIs");
            Assert.IsNotNull(method, "DisplayAllEMIs method should exist in Program class.");
        }

        [Test, Order(7)]
        public void Test_DeleteEMIByLoanType_Method_Should_Exist()
        {
            var method = typeof(Program).GetMethod("DeleteEMIByLoanType");
            Assert.IsNotNull(method, "DeleteEMIByLoanType method should exist in Program class.");
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentEMIByLoanType()
        {
            Program.DeleteEMIByLoanType("InvalidLoanType");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT COUNT(*) FROM EMIDetails", connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                int count = Convert.ToInt32(table.Rows[0][0]);
                Assert.AreEqual(0, count, "No records should be deleted for a non-existent loan type.");
            }
        }

        [Test, Order(9)]
        public void Test_DisplayAllByproducts_Should_Show_Data()
        {
            AddTestEMI("Sam", "Agri Loan");
            AddTestEMI("Lily", "Education Loan");

            MethodInfo displayMethod = typeof(Program).GetMethod("DisplayAllEMIs");
            displayMethod.Invoke(null, null);

            string output = consoleOutput.ToString();
            Assert.IsTrue(output.Contains("Sam") && output.Contains("Lily"), "All EMIs should be displayed properly.");
        }
    }
}
