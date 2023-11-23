using BowlingApiService.DTOs;
using BowlingData.ModelLayer;

namespace BowlingApiService.ModelConversion
{
    public class CustomerDtoConvert
    {
        // Convert from Customer objects to CustomerDto objects
        public static List<CustomerDto>? FromCustomerCollection(List<Customer> inCustomers)
        {
            List<CustomerDto>? aCustomerReadDtoList = null;
            if (inCustomers != null)
            {
                aCustomerReadDtoList = new List<CustomerDto>();
                CustomerDto? tempDto;
                foreach (Customer aCustomer in inCustomers)
                {
                    if (aCustomer != null)
                    {
                        tempDto = FromCustomer(aCustomer);
                        aCustomerReadDtoList.Add(tempDto);
                    }
                }
            }
            return aCustomerReadDtoList;
        }

        // Convert from Customer object to CustomerDto object
        public static CustomerDto? FromCustomer(Customer inCustomer)
        {
            CustomerDto? aCustomerReadDto = null;
            if (inCustomer != null)
            {
                aCustomerReadDto = new CustomerDto(inCustomer.FirstName, inCustomer.LastName, inCustomer.Email, inCustomer.Phone);
            }
            return aCustomerReadDto;
        }

        // Convert from CustomerDto object to Customer object
        public static Customer? ToCustomer(CustomerDto inDto, int idToUpdate)
        {
            Customer? aCustomer = null;
            
            if (inDto != null)
            {
                aCustomer = new Customer(inDto.FirstName, inDto.LastName, inDto.Email, inDto.Phone);
                aCustomer.Id= idToUpdate;
            }
            return aCustomer;
           
        }
        // Convert from CustomerDto object to Customer object
        public static Customer? ToCustomer(CustomerDto inDto)
        {
            Customer? aCustomer = null;
            if (inDto != null)
            {
                aCustomer = new Customer(inDto.FirstName, inDto.LastName, inDto.Email, inDto.Phone);
            }
            return aCustomer;
        }
    }
}
