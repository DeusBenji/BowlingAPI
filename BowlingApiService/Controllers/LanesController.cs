using BowlingApiService.BusinessLogicLayer;
using BowlingApiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BowlingApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanesController : ControllerBase
    {
        private readonly ILaneData _businessLogicCtrl;
        public LanesController(ILaneData inBusinessLogicCtrl) 
        {
            _businessLogicCtrl = inBusinessLogicCtrl;
        }

        [HttpGet]
        public ActionResult<List<LaneDto>> Get()
        {
            ActionResult<List<LaneDto>> foundReturn;
            // retrieve data - converted to DTO
            List<LaneDto>? foundLanes = _businessLogicCtrl.Get();
            // evaluate
            if (foundLanes != null)
            {
                if (foundLanes.Count > 0)
                {
                    // Remove the foreach loop that retrieves the lane by id

                    foundReturn = Ok(foundLanes); // Statuscode 200
                }
                else
                {
                    foundReturn = new StatusCodeResult(204); // Ok, but no content
                }
            }
            else
            {
                foundReturn = new StatusCodeResult(500); // Internal server error
            }
            // send response back to client
            return foundReturn;
        }

        // URL: api/lanes/{id}
        [HttpGet, Route("{id}")]
        public ActionResult<LaneDto> Get(int id)
        {
            ActionResult<LaneDto> foundReturn;
            // retrieve data - converted to DTO
            LaneDto? foundLane = _businessLogicCtrl.Get(id);
            // evaluate
            if (foundLane != null)
            {
                // Remove the unnecessary code that retrieves the lane by id

                foundReturn = Ok(foundLane); // Statuscode 200
            }
            else
            {
                foundReturn = new StatusCodeResult(404); // Not found
            }
            // send response back to client
            return foundReturn;
        }

        // URL: api/lanes
        [HttpPost]
        public ActionResult<int> PostNewLane(LaneDto inLaneDto)
        {
            ActionResult<int> foundReturn;
            int insertedId = -1;
            if (inLaneDto != null)
            {
                insertedId = _businessLogicCtrl.Add(inLaneDto);
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
        // URL: api/lanes/{id}
        [HttpDelete("{id}")]
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
        // URL: api/lanes/{id}
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] LaneDto updatedLaneDto)
        {
            if (updatedLaneDto == null)
            {
                return BadRequest();    // Bad request, missing input
            }

            // Retrieve the existing lane details
            LaneDto? existingLaneDto = _businessLogicCtrl.Get(id);

            if (existingLaneDto == null)
            {
                return NotFound();    // Lane not found
            }
            // Update the existing lane details with the new values
            existingLaneDto.LaneNumber = updatedLaneDto.LaneNumber;

            bool isUpdated = _businessLogicCtrl.Put(existingLaneDto, id);
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
