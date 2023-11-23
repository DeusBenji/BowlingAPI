using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace BowlingDataTest
{
    public class TestPriceDataAccess
    {
        private readonly ITestOutputHelper _extraOutput;

        readonly private IPriceAccess _pAccess;

        readonly string _connectionString = "Server=localhost; Integrated Security=true; Database=BowlingTest";

        public TestPriceDataAccess(ITestOutputHelper output)
        {
            _extraOutput = output;
            _pAccess = new PriceDatabaseAccess(_connectionString);

        }

        [Fact]
        public void TestGetAllPrices()
        {
            // Arrange

            // Act
            List<Price> readPrices = _pAccess.GetAllPrices();
            bool pricesWereRead = (readPrices.Count > 0);
            // Print additional output
            _extraOutput.WriteLine("Number of prices: " + readPrices.Count);

            // Assert
            Assert.True(pricesWereRead);
        }
        [Fact]
        public void TestCreatePrice()
        {
            // Arrange
            Price price = new Price(120.00, "Torsdag"); // Create a new Price object

            // Act
            int insertedId = _pAccess.CreatePrice(price);

            // Assert
            Assert.True(insertedId > 0);
            _pAccess.DeletePriceById(insertedId);
        }

        [Fact]
        public void TestGetPriceById()
        {
            // Arrange
            double actualNP = 120.00;
            Price price = new Price(120.00, "Torsdag"); // Insert the Price into the database
            int insertedId = _pAccess.CreatePrice(price);
            // Act
            Price priceRetrived = _pAccess.GetPriceById(insertedId);

            // Assert
            Assert.NotNull(priceRetrived);
            Assert.Equal(insertedId, priceRetrived.Id);
            Assert.Equal(actualNP, priceRetrived.NormalPrice);
            _pAccess.DeletePriceById(insertedId);
        }

        [Fact]
        public void TestDeletePriceById()
        {
            // Arrange
            Price price = new Price(120.00, "Torsdag"); // Create a new Price object
            int insertedId = _pAccess.CreatePrice(price); // Insert the Price into the database

            // Act
            bool isDeleted = _pAccess.DeletePriceById(insertedId);

            // Assert
            Assert.True(isDeleted);
        }
        [Fact]
        public void TestUpdatePrice()
        {
            // Arrange
            Price price = new Price(120.00, "Torsdag"); // Create a new Price object
            int insertedId = _pAccess.CreatePrice(price); // Insert the Price into the database

            // Modify the Price object
            Price updatedPrice = new Price(insertedId, 130.00, "Torsdag");

            // Act
            bool isUpdated = _pAccess.UpdatePrice(updatedPrice);

            // Retrieve the updated Price from the database
            Price retrievedPrice = _pAccess.GetPriceById(insertedId);

            // Assert
            Assert.True(isUpdated);
            Assert.NotNull(retrievedPrice);
            Assert.Equal(updatedPrice.Id, retrievedPrice.Id);
            Assert.Equal(updatedPrice.NormalPrice, retrievedPrice.NormalPrice);
            _pAccess.DeletePriceById(insertedId);
        }
    }
}
