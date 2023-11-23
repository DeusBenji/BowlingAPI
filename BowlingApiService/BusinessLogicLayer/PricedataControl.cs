using BowlingApiService.DTOs;
using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;

namespace BowlingApiService.BusinessLogicLayer
{
    public class PricedataControl : IPriceData
    {
        private readonly IPriceAccess _priceAccess;

        public PricedataControl(IPriceAccess inPriceAccess)
        {
            _priceAccess = inPriceAccess;
        }

        // Adds a new price to the database
        public int Add(PriceDto newPrice)
        {
            int insertedId = 0;
            try
            {
                Price? foundPrice = ModelConversion.PriceDtoConvert.ToPrice(newPrice);
                if (foundPrice != null)
                {
                    // Create the price in the database and get the inserted ID
                    insertedId = _priceAccess.CreatePrice(foundPrice);
                }
            }
            catch
            {
                // If an exception occurs, set the inserted ID to -1
                insertedId = -1;
            }
            return insertedId;
        }

        // Deletes a price from the database
        public bool Delete(int id)
        {
            try
            {
                // Delete the price from the database by ID
                bool isDeleted = _priceAccess.DeletePriceById(id);
                return isDeleted;
            }
            catch
            {
                // If an exception occurs, return false
                return false;
            };
        }

        // Retrieves a price from the database by ID
        public PriceDto? Get(int id)
        {
            PriceDto? foundPriceDto;
            try
            {
                // Get the price from the database by ID
                Price? foundPrice = _priceAccess.GetPriceById(id);
                // Convert the retrieved Price object to PriceDto
                foundPriceDto = ModelConversion.PriceDtoConvert.FromPrice(foundPrice);
            }
            catch
            {
                // If an exception occurs, set foundPriceDto to null
                foundPriceDto = null;
            }
            return foundPriceDto;
        }

        // Retrieves all prices from the database
        public List<PriceDto>? Get()
        {
            List<PriceDto>? foundDtos;
            try
            {
                // Get all prices from the database
                List<Price>? foundPrices = _priceAccess.GetAllPrices();
                // Convert the list of Price objects to PriceDto objects
                foundDtos = ModelConversion.PriceDtoConvert.FromPriceCollection(foundPrices);
            }
            catch
            {
                // If an exception occurs, set foundDtos to null
                foundDtos = null;
            }
            return foundDtos;
        }

        // Updates a price in the database
        public bool Put(PriceDto priceToUpdate, int idToUpdate)
        {
            try
            {
                // Convert the updated PriceDto object to Price object
                Price? updatedPrice = ModelConversion.PriceDtoConvert.ToPrice(priceToUpdate, idToUpdate);
                // Update the price in the database
                return _priceAccess.UpdatePrice(updatedPrice);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex);
                // If an exception occurs, return false
                return false;
            }
        }
    }
}
