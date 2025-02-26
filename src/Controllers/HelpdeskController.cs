using ITHelpdeskAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ITHelpdeskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpdeskController : ControllerBase
    {
        private readonly HelpdeskService _helpdeskService;

        public HelpdeskController(HelpdeskService helpdeskService)
        {
            _helpdeskService = helpdeskService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all helpdesk cases")]
        [SwaggerResponse(200, "Returns all helpdesk cases", typeof(IEnumerable<HelpdeskCase>))]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Get all helpdesk cases")]
        public ActionResult<IEnumerable<HelpdeskCase>> GetAllCases()
        {
            var cases = _helpdeskService.GetAllCases();
            return Ok(cases);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a helpdesk case by ID")]
        public ActionResult<HelpdeskCase> GetCase(Guid id)
        {
            var helpdeskCase = _helpdeskService.GetCaseById(id);
            if (helpdeskCase == null)
            {
                return NotFound();
            }
            return Ok(helpdeskCase);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new helpdesk case")]
        [SwaggerResponse(201, "Helpdesk case created", typeof(HelpdeskCase))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Create a new helpdesk case")]
        public ActionResult<HelpdeskCase> CreateCase([FromBody] HelpdeskCase helpdeskCase)
        {
            if (helpdeskCase == null)
            {
                return BadRequest();
            }

            _helpdeskService.AddCase(helpdeskCase);
            return CreatedAtAction(nameof(GetCase), new { id = helpdeskCase.Id }, helpdeskCase);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing helpdesk case")]
        [SwaggerResponse(204, "Helpdesk case updated")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Helpdesk case not found")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Update an existing helpdesk case")]
        public ActionResult UpdateCase(Guid id, [FromBody] HelpdeskCase helpdeskCase)
        {
            if (helpdeskCase == null || helpdeskCase.Id != id)
            {
                return BadRequest();
            }

            var existingCase = _helpdeskService.GetCaseById(id);
            if (existingCase == null)
            {
                return NotFound();
            }

            _helpdeskService.UpdateCase(id, helpdeskCase);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a helpdesk case")]
        [SwaggerResponse(204, "Helpdesk case deleted")]
        [SwaggerResponse(404, "Helpdesk case not found")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Delete a helpdesk case")]
        public ActionResult DeleteCase(Guid id)
        {
            var existingCase = _helpdeskService.GetCaseById(id);
            if (existingCase == null)
            {
                return NotFound();
            }

            _helpdeskService.DeleteCase(id);
            return NoContent();
        }
    }
}