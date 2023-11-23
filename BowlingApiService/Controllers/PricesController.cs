using BowlingApiService.DTOs;
using BowlingApiService.BusinessLogicLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BowlingApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly IPriceData _businessLogicCtrl;

        public PricesController(IPriceData inBusinessLogicCtrl)
        {
            _businessLogicCtrl = inBusinessLogicCtrl;
        }
        //
        // URL: api/prices
        [HttpGet]
        public ActionResult<List<PriceDto>> Get()
        {
            ActionResult<List<PriceDto>> foundReturn;
            // retrieve data - converted to DTO
            List<PriceDto>? foundPrices = _businessLogicCtrl.Get();
            // evaluate
            if (foundPrices != null)
            {
                if (foundPrices.Count > 0)
                {
                    foundReturn = Ok(foundPrices);                   // Statuscode 200
                }
                else
                {
                    foundReturn = new StatusCodeResult(204);    // Ok, but no content
                }
            }
            else
            {
                foundReturn = new StatusCodeResult(500);        // Internal server error
            }
            // send response back to client
            return foundReturn;
        }
        
        // URL: api/prices/{id}
        [HttpGet, Route("{id}")]
        [Authorize]
        public ActionResult<PriceDto> Get(int id)
        {
            ActionResult<PriceDto> foundReturn;
            // retrieve data - converted to DTO
            PriceDto? foundPrice = _businessLogicCtrl.Get(id);
            // evaluate
            if (foundPrice != null)
            {
                foundReturn = Ok(foundPrice);       // Statuscode 200
            }
            else
            {
                foundReturn = new StatusCodeResult(404);    // Not found
            }
            // send response back to client
            return foundReturn;
        }

        // URL: api/prices
        [HttpPost]
        [Authorize]
        public ActionResult<int> PostNewPrice(PriceDto inPriceDto)
        {
            ActionResult<int> foundReturn;
            int insertedId = -1;
            if (inPriceDto != null)
            {
                insertedId = _businessLogicCtrl.Add(inPriceDto);
            }
            // Evaluate
            if (insertedId > 0)
            {
                foundReturn = Ok(insertedId);
            }
            else if (insertedId == 0)
            {
                foundReturn = BadRequest();     // missing input
            }
            else
            {
                foundReturn = new StatusCodeResult(500);    // Internal server error
            }
            return foundReturn;
        }

        // URL: api/prices/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            ActionResult foundReturn;
            bool isDeleted = _businessLogicCtrl.Delete(id);
            // Evaluate
            if (isDeleted)
            {
                foundReturn = Ok(isDeleted);           // Statuscode 200
            }
            else
            {
                foundReturn = new StatusCodeResult(404);    // Not found
            }
            // send response back to client
            return foundReturn;
        }

        // URL: api/prices/{id}
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, [FromBody] PriceDto updatedPriceDto)
        {
            if (updatedPriceDto == null)
            {
                return BadRequest();    // Bad request, missing input
            }

            // Retrieve the existing price details
            PriceDto? existingPriceDto = _businessLogicCtrl.Get(id);

            if (existingPriceDto == null)
            {
                return NotFound();    // Price not found
            }

            // Update the existing price details with the new values
            existingPriceDto.NormalPrice = updatedPriceDto.NormalPrice;
            existingPriceDto.Weekday = updatedPriceDto.Weekday;

            bool isUpdated = _businessLogicCtrl.Put(existingPriceDto, id);
            if (isUpdated)
            {
                return Ok(isUpdated);     // Statuscode 200
            }
            else
            {
                return StatusCode(500); // Internal server error
            }
        }
    }
}
