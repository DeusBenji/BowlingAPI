using BowlingApiService.DTOs;
using BowlingData.ModelLayer;

namespace BowlingApiService.ModelConversion
{
    public class PriceDtoConvert
    {
        // Convert from Price objects to PriceDto objects
        public static List<PriceDto>? FromPriceCollection(List<Price> inPrices)
        {
            List<PriceDto>? priceDtoList = null;
            if (inPrices != null)
            {
                priceDtoList = new List<PriceDto>();
                PriceDto? tempDto;
                foreach (Price aPrice in inPrices)
                {
                    if (aPrice != null)
                    {
                        tempDto = FromPrice(aPrice);
                        priceDtoList.Add(tempDto);
                    }
                }
            }
            return priceDtoList;
        }

        // Convert from Price object to PriceDto object
        public static PriceDto? FromPrice(Price inPrice)
        {
            PriceDto? priceDto = null;
            if (inPrice != null)
            {
                priceDto = new PriceDto(inPrice.NormalPrice, inPrice.Weekday);
                priceDto.Id = inPrice.Id;
            }
            return priceDto;
        }

        // Convert from PriceDto object to Price object
        public static Price? ToPrice(PriceDto inDto)
        {
            Price? price = null;
            if (inDto != null)
            {
                price = new Price(inDto.NormalPrice, inDto.Weekday);
            }
            return price;
        }
        // Convert from PriceDto object to Price object
        public static Price? ToPrice(PriceDto inDto, int idToUpdate)
        {
            Price? aPrice = null;
            if (inDto != null)
            {
                aPrice = new Price(inDto.NormalPrice, inDto.Weekday);
                aPrice.Id = idToUpdate;
            }
            return aPrice;
        }
    }
}
