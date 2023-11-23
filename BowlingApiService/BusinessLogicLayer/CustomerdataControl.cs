using BowlingApiService.DTOs;
using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;
using System.Data.SqlClient;

namespace BowlingApiService.BusinessLogicLayer
{
    public class CustomerdataControl : ICustomerData
    {
        private readonly ICustomerAccess _customerAccess;

        public CustomerdataControl(ICustomerAccess inCustomerAccess)
        {
            _customerAccess = inCustomerAccess;
        }

        // Adds a new customer to the database
        public int Add(CustomerDto newCustomer)
        {
            int insertedId = 0;
            try
            {
                Customer? foundCustomer = ModelConversion.CustomerDtoConvert.ToCustomer(newCustomer);
                if (foundCustomer != null)
                {
                    // Create the customer in the database and get the inserted ID
                    insertedId = _customerAccess.CreateCustomer(foundCustomer);
                }
            }
            catch
            {
                // If an exception occurs, set the inserted ID to -1
                insertedId = -1;
            }
            return insertedId;
        }

        // Deletes a customer from the database by ID
        public bool Delete(int id)
        {
            try
            {
                // Delete the customer from the database by ID
                bool isDeleted = _customerAccess.DeleteCustomerById(id);
                return isDeleted;
            }
            catch
            {
                // If an exception occurs, return false
                return false;
            };
        }

        // Retrieves a customer from the database by ID
        public CustomerDto? Get(int id)
        {
            CustomerDto? foundCustomerDto;
            try
            {
                // Get the customer from the database by ID
                Customer? foundCustomer = _customerAccess.GetCustomerById(id);
                // Convert the retrieved Customer object to CustomerDto
                foundCustomerDto = ModelConversion.CustomerDtoConvert.FromCustomer(foundCustomer);
            }
            catch
            {
                // If an exception occurs, set foundCustomerDto to null
                foundCustomerDto = null;
            }
            return foundCustomerDto;
        }

        // Retrieves all customers from the database
        public List<CustomerDto>? Get()
        {
            List<CustomerDto>? foundDtos;
            try
            {
                // Get all customers from the database
                List<Customer>? foundCustomers = _customerAccess.GetAllCustomers();
                // Convert the list of Customer objects to CustomerDto objects
                foundDtos = ModelConversion.CustomerDtoConvert.FromCustomerCollection(foundCustomers);

                if (foundDtos != null)
                {
                    foreach (var dto in foundDtos)
                    {
                        // Retrieve the customer from the database by phone
                        Customer cus = _customerAccess.GetCustomerByPhone(dto.Phone);
                        // Update the ID of the CustomerDto with the retrieved customer's ID
                        dto.Id = cus.Id;
                    }
                }
            }
            catch
            {
                // If an exception occurs, set foundDtos to null
                foundDtos = null;
            }
            return foundDtos;
        }

        // Updates a customer in the database
        public bool Put(CustomerDto customerToUpdate, int idToUpdate)
        {
            try
            {
                // Convert the updated CustomerDto object to Customer object
                Customer? updatedCustomer = ModelConversion.CustomerDtoConvert.ToCustomer(customerToUpdate, idToUpdate);
                // Update the customer in the database
                return _customerAccess.UpdateCustomer(updatedCustomer);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex);
                // If an exception occurs, return false
                return false;
            }
        }

        // Retrieves a customer from the database by phone number
        public CustomerDto? Get(string phone)
        {
            CustomerDto? foundCustomerDto;
            try
            {
                // Get the customer from the database by phone number
                Customer? foundCustomer = _customerAccess.GetCustomerByPhone(phone);
                // Convert the retrieved Customer object to CustomerDto
                foundCustomerDto = ModelConversion.CustomerDtoConvert.FromCustomer(foundCustomer);

                if (foundCustomer != null)
                {
                    // Update the ID of the CustomerDto with the retrieved customer's ID
                    foundCustomerDto.Id = foundCustomer.Id;
                }
            }
            catch
            {
                // If an exception occurs, set foundCustomerDto to null
                foundCustomerDto = null;
            }
            return foundCustomerDto;
        }
    }
}
