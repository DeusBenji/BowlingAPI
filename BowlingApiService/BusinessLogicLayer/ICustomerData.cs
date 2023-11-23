using BowlingApiService.DTOs;
using BowlingData.ModelLayer;

namespace BowlingApiService.BusinessLogicLayer
{
    public interface ICustomerData {

        CustomerDto? Get(int id);
        List<CustomerDto>? Get();
        int Add(CustomerDto customerToAdd);
        bool Put(CustomerDto customerToUpdate, int idToUpdate);
        bool Delete(int id);
        CustomerDto? Get(string phone);
    }
}
