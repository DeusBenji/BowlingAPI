using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingData.DatabaseLayer
{
    public interface IPriceAccess
    {
        Price GetPriceById(int id);
        List<Price> GetAllPrices();
        int CreatePrice(Price aPrice);
        bool UpdatePrice(Price priceToUpdate);
        bool DeletePriceById(int id);
    }
}
