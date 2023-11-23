using Xunit.Abstractions;
using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;
using System;

namespace BowlingDataTest
{
    public class TestCustomerDataAccess
    {

        private readonly ITestOutputHelper _extraOutput;

        readonly private ICustomerAccess _CustomerAccess;
        readonly string _connectionString = "Server=localhost; Integrated Security=true; Database=BowlingDB";

        public TestCustomerDataAccess(ITestOutputHelper output)
        {
            _extraOutput = output;
            _CustomerAccess = new CustomerDatabaseAccess(_connectionString);
        }
      

        [Fact]
        public void TestGetAllCusomers()
        {
            // Arrange

            // Act
            List<Customer> readCustomers = _CustomerAccess.GetAllCustomers();
            bool customersWereRead = (readCustomers.Count > 0);
            // Print additional output
            _extraOutput.WriteLine("Number of Customers: " + readCustomers.Count);

            // Assert
            Assert.True(customersWereRead);
        }

        [Fact]
        public void TestCreateCustomer()
        {
            // Arrange
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "12345678"); // Create a new Lane object

            // Act
            int insertedId = _CustomerAccess.CreateCustomer(cus);
            
            // Assert
            Assert.True(insertedId > 0);
            _CustomerAccess.DeleteCustomerById(insertedId);
        }

        [Fact]
        public void TestGetCustomerById()
        {
            // Arrange
            string actualyFN = "Karl";
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "12345678"); // Create a new Lane object
            int insertedId = _CustomerAccess.CreateCustomer(cus); // Insert the Lane into the database
            // Act
            Customer retrievedCustomer = _CustomerAccess.GetCustomerById(insertedId);

            // Assert
            Assert.NotNull(retrievedCustomer);
            Assert.Equal(insertedId, retrievedCustomer.Id);
            Assert.Equal(actualyFN, retrievedCustomer.FirstName);
            _CustomerAccess.DeleteCustomerById(insertedId);
        }
        [Fact]
        public void TestGetCustomerByPhone()
        {
            // Arrange
            string actualyFN = "Karl";
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "12345678"); // Create a new Lane object
            int insertedId = _CustomerAccess.CreateCustomer(cus); // Insert the Lane into the database
            // Act
            Customer retrievedCustomer = _CustomerAccess.GetCustomerByPhone(cus.Phone);

            // Assert
            Assert.NotNull(retrievedCustomer);
            Assert.Equal(insertedId, retrievedCustomer.Id);
            Assert.Equal(actualyFN, retrievedCustomer.FirstName);
            _CustomerAccess.DeleteCustomerById(insertedId);
        }

        [Fact]
        public void TestDeleteLaneById()
        {
            // Arrange
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "12345678"); // Create a new Lane object
            int insertedId = _CustomerAccess.CreateCustomer(cus); // Insert the Lane into the database

            // Act
            bool isDeleted = _CustomerAccess.DeleteCustomerById(insertedId);

            // Assert
            Assert.True(isDeleted);
        }
        [Fact]
        public void TestUpdateCustomer()
        {
            // Arrange
            var customer = new Customer("Karl", "Hansen", "karl@gmail.com", "12345678");
            int insertedId = _CustomerAccess.CreateCustomer(customer);

            var updatedCustomer = new Customer(insertedId, "John", "Doe", "john@gmail.com", "87654321");

            // Act
            bool isUpdated = _CustomerAccess.UpdateCustomer(updatedCustomer);

            // Assert
            Assert.True(isUpdated);

            // Retrieve the updated customer from the database
            Customer retrievedCustomer = _CustomerAccess.GetCustomerById(insertedId);

            // Verify that the customer has been updated correctly
            Assert.NotNull(retrievedCustomer);
            Assert.Equal(updatedCustomer.Id, retrievedCustomer.Id);
            Assert.Equal(updatedCustomer.FirstName, retrievedCustomer.FirstName);
            Assert.Equal(updatedCustomer.LastName, retrievedCustomer.LastName);
            Assert.Equal(updatedCustomer.Email, retrievedCustomer.Email);
            Assert.Equal(updatedCustomer.Phone, retrievedCustomer.Phone);
            _CustomerAccess.DeleteCustomerById(insertedId);
        }
    }
}