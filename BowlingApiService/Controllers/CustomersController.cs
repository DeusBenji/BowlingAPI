using BowlingApiService.BusinessLogicLayer;
using BowlingApiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BowlingApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomersController : ControllerBase
    {
        private readonly ICustomerData _businessLogicCtrl;

        public CustomersController(ICustomerData inBusinessLogicCtrl)
        {
            _businessLogicCtrl = inBusinessLogicCtrl;
        }

        // URL: api/customers
        [HttpGet]
        [Authorize]
        public ActionResult<List<CustomerDto>> Get()
        {
            ActionResult<List<CustomerDto>> foundReturn;
            // retrieve data - converted to DTO
            List<CustomerDto>? foundCustomers = _businessLogicCtrl.Get();
            // evaluate
            if (foundCustomers != null)
            {
                if (foundCustomers.Count > 0)
                {
                    foundReturn = Ok(foundCustomers);
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

        // URL: api/customers
        [HttpPost]
        public ActionResult<int> PostNewCustomer(CustomerDto inCustomerDto)
        {
            ActionResult<int> foundReturn;
            int insertedId = -1;
            if (inCustomerDto != null)
            {
                insertedId = _businessLogicCtrl.Add(inCustomerDto);
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
        // URL: api/customers/{id}
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
        // URL: api/customers/{id}
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, [FromBody] CustomerDto updatedCustomerDto)
        {
            
            if (updatedCustomerDto == null)
            {
                return BadRequest();    // Bad request, missing input
            }

            // Retrieve the existing customer details
            CustomerDto? existingCustomerDto = _businessLogicCtrl.Get(id);

            if (existingCustomerDto == null)
            {
                return NotFound();    // Customer not found
            }

            // Update the existing customer details with the new values
            existingCustomerDto.FirstName = updatedCustomerDto.FirstName;
            existingCustomerDto.LastName = updatedCustomerDto.LastName;
            existingCustomerDto.Email = updatedCustomerDto.Email;
            existingCustomerDto.Phone = updatedCustomerDto.Phone;

            bool isUpdated = _businessLogicCtrl.Put(existingCustomerDto, id);
            if (isUpdated)
            {
                return Ok(isUpdated);     // Statuscode 200
            }
            else
            {
                return StatusCode(500); // Internal server error
            }
        }
        // URL: api/customers/{phone}
        [HttpGet, Route("{phone}")]
        public ActionResult<CustomerDto> Get(string phone)
        {
            CustomerDto? foundCustomer = _businessLogicCtrl.Get(phone);

            if (foundCustomer != null)
            {
                // Include customer ID in the response headers
                Response.Headers.Add("CustomerId", foundCustomer.Id.ToString());
                return Ok(foundCustomer);       // Status code 200
            }
            else
            {
                return NotFound();    // Status code 404
            }
        }

    }
}

