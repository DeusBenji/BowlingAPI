using BowlingApiService.DTOs;

namespace BowlingApiService.BusinessLogicLayer
{
    public interface IPriceData
    {
        PriceDto? Get(int id);
        List<PriceDto>? Get();
        int Add(PriceDto priceToAdd);
        bool Put(PriceDto priceToUpdate, int idToUpdate);
        bool Delete(int id);
    }
}
