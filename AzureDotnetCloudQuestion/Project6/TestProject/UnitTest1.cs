using System;
using System.Collections.Generic;
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
    public class SocialMediaPostManagementTests
    {
        private string connectionString = ConnectionStringProvider.ConnectionString;
        private StringWriter consoleOutput;
        private TextWriter originalConsoleOut;

        [SetUp]
        public void Setup()
        {
            // Clear the database before each test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SocialMediaPosts", conn);
                cmd.ExecuteNonQuery();
            }

            // Redirect console output to capture messages
            originalConsoleOut = Console.Out;
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [TearDown]
        public void TearDown()
        {
            // Reset console output
            Console.SetOut(originalConsoleOut);

            // Clear the database after each test
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SocialMediaPosts", conn);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Order(1)]
        public void Test_SocialMediaPost_Class_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SocialMediaPost";
            Assembly assembly = Assembly.Load(assemblyName);
            Type postType = assembly.GetType(typeName);

            // Assert
            Assert.IsNotNull(postType, "SocialMediaPost class should exist.");
        }

        [Test, Order(2)]
        public void Test_SocialMediaPost_Properties_Should_Exist()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SocialMediaPost";
            Assembly assembly = Assembly.Load(assemblyName);
            Type postType = assembly.GetType(typeName);

            // Act
            PropertyInfo postIdProperty = postType.GetProperty("PostID");
            PropertyInfo titleProperty = postType.GetProperty("Title");
            PropertyInfo contentProperty = postType.GetProperty("Content");
            PropertyInfo publishDateProperty = postType.GetProperty("PublishDate");
            PropertyInfo platformProperty = postType.GetProperty("Platform");
            PropertyInfo statusProperty = postType.GetProperty("Status");

            // Assert
            Assert.IsNotNull(postIdProperty, "PostID property should exist.");
            Assert.IsNotNull(titleProperty, "Title property should exist.");
            Assert.IsNotNull(contentProperty, "Content property should exist.");
            Assert.IsNotNull(publishDateProperty, "PublishDate property should exist.");
            Assert.IsNotNull(platformProperty, "Platform property should exist.");
            Assert.IsNotNull(statusProperty, "Status property should exist.");
        }

        [Test, Order(3)]
        public void Test_AddSocialMediaPost_Method_Exists()
        {
            var method = typeof(Program).GetMethod("AddSocialMediaPost");
            Assert.IsNotNull(method, "AddSocialMediaPost method should exist.");
        }

        [Test, Order(4)]
        public void Test_DeleteSocialMediaPostByPlatform_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DeleteSocialMediaPostByPlatform");
            Assert.IsNotNull(method, "DeleteSocialMediaPostByPlatform method should exist.");
        }

        [Test, Order(5)]
        public void Test_DisplayAllSocialMediaPosts_Method_Exists()
        {
            var method = typeof(Program).GetMethod("DisplayAllSocialMediaPosts");
            Assert.IsNotNull(method, "DisplayAllSocialMediaPosts method should exist.");
        }

        [Test, Order(6)]
        public void Test_AddSocialMediaPost_Should_Insert_Post_Into_Database()
        {
            // Arrange
            string assemblyName = "dotnetapp";
            string typeName = "dotnetapp.Models.SocialMediaPost";
            Assembly assembly = Assembly.Load(assemblyName);
            Type postType = assembly.GetType(typeName);
            object postInstance = Activator.CreateInstance(postType);

            // Setting properties dynamically
            postType.GetProperty("Title").SetValue(postInstance, "Test Post");
            postType.GetProperty("Content").SetValue(postInstance, "This is a test post.");
            postType.GetProperty("PublishDate").SetValue(postInstance, "2024-01-15");
            postType.GetProperty("Platform").SetValue(postInstance, "Facebook");
            postType.GetProperty("Status").SetValue(postInstance, "Scheduled");

            // Act - Call AddSocialMediaPost method
            MethodInfo addPostMethod = typeof(Program).GetMethod("AddSocialMediaPost");
            addPostMethod.Invoke(null, new[] { postInstance });

            // Assert - Check if the post was added to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SocialMediaPosts WHERE Title = @Title AND Platform = @Platform", conn);
                cmd.Parameters.AddWithValue("@Title", "Test Post");
                cmd.Parameters.AddWithValue("@Platform", "Facebook");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(1, count, "The social media post should have been inserted into the database.");
            }
        }

        [Test, Order(7)]
        public void Test_DeleteSocialMediaPostByPlatform_ValidPlatform_ShouldDeletePost()
        {
            // Arrange
            var postObj = new Dictionary<string, object>
            {
                { "Title", "Test Post" },
                { "Content", "This is a test post." },
                { "PublishDate", "2024-01-15" },
                { "Platform", "Twitter" },
                { "Status", "Draft" }
            };

            // Add a new social media post entity
            var post = new SocialMediaPost();
            foreach (var kvp in postObj)
            {
                var propertyInfo = typeof(SocialMediaPost).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(post, kvp.Value);
                }
            }
            MethodInfo addPostMethod = typeof(Program).GetMethod("AddSocialMediaPost");
            addPostMethod.Invoke(null, new[] { post });
            // Act - Call DeleteSocialMediaPostByPlatform method
            MethodInfo deletePostMethod = typeof(Program).GetMethod("DeleteSocialMediaPostByPlatform");
            deletePostMethod.Invoke(null, new object[] { "Twitter" });

            // Assert - Check if the post was deleted from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SocialMediaPosts WHERE Platform = @Platform", conn);
                cmd.Parameters.AddWithValue("@Platform", "Twitter");
                int count = (int)cmd.ExecuteScalar();

                Assert.AreEqual(0, count, "The social media post(s) with platform 'Twitter' should have been deleted from the database.");
            }
        }

        [Test, Order(8)]
        public void Test_DeleteNonExistentSocialMediaPostByPlatform()
        {
            // Act - Call DeleteSocialMediaPostByPlatform method for a non-existent platform
            MethodInfo deletePostMethod = typeof(Program).GetMethod("DeleteSocialMediaPostByPlatform");
            deletePostMethod.Invoke(null, new object[] { "NonExistentPlatform" });

            string output = consoleOutput.ToString().Trim();
            // Convert output to lowercase and check without any dots
            Assert.IsTrue(output.ToLower().Contains("no social media posts found with platform nonexistentplatform"),
                "The output should indicate that no posts were found for the specified platform.");
        }

        [Test, Order(9)]
        public void Test_DisplayAllSocialMediaPosts_ShouldReturnData()
        {
            var postObj = new Dictionary<string, object>
            {
                { "Title", "Test Post" },
                { "Content", "This is a test post." },
                { "PublishDate", "2024-01-15" },
                { "Platform", "Instagram" },
                { "Status", "Published" }
            };

            // Add a new social media post entity
            var post = new SocialMediaPost();
            foreach (var kvp in postObj)
            {
                var propertyInfo = typeof(SocialMediaPost).GetProperty(kvp.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(post, kvp.Value);
                }
            }
            MethodInfo addPostMethod = typeof(Program).GetMethod("AddSocialMediaPost");
            addPostMethod.Invoke(null, new[] { post });
            // Act - Call DisplayAllSocialMediaPosts method
            MethodInfo displayAllPostsMethod = typeof(Program).GetMethod("DisplayAllSocialMediaPosts");
            displayAllPostsMethod.Invoke(null, null);

            string output = consoleOutput.ToString().Trim().ToLower();
            // Check using lowercase conversion and without dots in the string literal
            Assert.IsFalse(output.Contains("no social media posts found"), "Posts should be displayed.");
            StringAssert.Contains("test post", output);
            StringAssert.Contains("instagram", output);
        }

        private string CaptureConsoleOutput(Action action)
        {
            consoleOutput.GetStringBuilder().Clear(); // Clear previous output
            action.Invoke();
            return consoleOutput.ToString();
        }
    }
}
