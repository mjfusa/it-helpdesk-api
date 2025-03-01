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
        [SwaggerOperation(Summary = "Get all helpdesk cases", OperationId = "idGetAllCases", Description = "Returns all helpdesk cases")]
        [SwaggerResponse(200, "Returns all helpdesk cases", typeof(IEnumerable<HelpdeskCase>))]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Get all helpdesk cases")]
        public ActionResult<IEnumerable<HelpdeskCase>> GetAllCases()
        {
            var cases = _helpdeskService.GetAllCases();
            return Ok(cases);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a helpdesk case by ID", OperationId = "idGetCaseById", Description = "Returns a helpdesk case by its ID")]
        [SwaggerResponse(200, "Returns the helpdesk case", typeof(HelpdeskCase))]
        [SwaggerResponse(404, "Helpdesk case not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<HelpdeskCase> GetCase(
            [SwaggerParameter("Case id", Required = true)]
            Guid id)
        {
            var helpdeskCase = _helpdeskService.GetCaseById(id);
            if (helpdeskCase == null)
            {
                return NotFound();
            }
            return Ok(helpdeskCase);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new helpdesk case", OperationId = "idCreateCase", Description = "Creates a new helpdesk case")]
        [SwaggerResponse(201, "Helpdesk case created", typeof(HelpdeskCase))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Create a new helpdesk case")]
        public ActionResult<HelpdeskCase> CreateCase(
            [FromBody, SwaggerParameter("A HelpdeskCase object", Required = true)]
            HelpdeskCase helpdeskCase)
        {
            if (helpdeskCase == null)
            {
                return BadRequest();
            }

            _helpdeskService.AddCase(helpdeskCase);
            return CreatedAtAction(nameof(GetCase), new { id = helpdeskCase.Id }, helpdeskCase);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing helpdesk case", OperationId = "idUpdateCase", Description = "Updates an existing helpdesk case")]
        [SwaggerResponse(204, "Helpdesk case updated")]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "Helpdesk case not found")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Update an existing helpdesk case")]
        public ActionResult UpdateCase(
            [SwaggerParameter("Id of Case", Required = true)]
            Guid id,
            [FromBody,SwaggerParameter("A Helpdeskcase object", Required = true)]
            HelpdeskCase helpdeskCase)
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
        [SwaggerOperation(Summary = "Delete a helpdesk case", OperationId = "idDeleteCase", Description = "Deletes a helpdesk case by its ID")]
        [SwaggerResponse(204, "Helpdesk case deleted")]
        [SwaggerResponse(404, "Helpdesk case not found")]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Delete a helpdesk case")]
        public ActionResult DeleteCase(
            [SwaggerParameter("Id of Case", Required = true)]
            Guid id)
        {
            var existingCase = _helpdeskService.GetCaseById(id);
            if (existingCase == null)
            {
                return NotFound();
            }

            _helpdeskService.DeleteCase(id);
            return NoContent();
        }

        [HttpGet("{CreateCases},{NumberOfCases}")]
        [SwaggerOperation(Summary = "Create three helpdesk cases", OperationId = "idCreateStarterCases", Description = "Creates three helpdesk cases")]
        [SwaggerResponse(200, "Returns all helpdesk cases", typeof(IEnumerable<HelpdeskCase>))]
        [SwaggerResponse(500, "Internal server error")]
        [DisplayName("Get all helpdesk cases")]
        public ActionResult<IEnumerable<HelpdeskCase>> CreateCases(
        [SwaggerParameter("Command 'CreateCases'", Required = true)]
        string CreateCases,
        [SwaggerParameter("Number of cases to create", Required = true)]
        string NumberOfCases)
        {
            HelpdeskCase[]? hdCase = new HelpdeskCase[3];
            hdCase[0] = new HelpdeskCase();
            hdCase[0].Id = Guid.NewGuid();
            hdCase[0].OpenedBy = "John Freeman";
            hdCase[0].Title = "Problem with VPN";
            hdCase[0].Description = "VPN will not connect.";
            hdCase[0].AssignedTo = "Shri Ilich";
            hdCase[0].Priority = "Normal";

            hdCase[1] = new HelpdeskCase();
            hdCase[1].Id = Guid.NewGuid();
            hdCase[1].OpenedBy = "Steve Rogers";
            hdCase[1].Title = "Locked out of computer";
            hdCase[1].Description = "Computer will turn on, but does not accept password.";
            hdCase[1].AssignedTo = "Bob Smith";
            hdCase[1].Priority = "Urgent";

            hdCase[2] = new HelpdeskCase();
            hdCase[2].Id = Guid.NewGuid();
            hdCase[2].OpenedBy = "Bharath Kumar";
            hdCase[2].Title = "Windows update failure";
            hdCase[2].Description = "Windows update fails to install.";
            hdCase[2].AssignedTo = "Shri Ilich";
            hdCase[2].Priority = "Normal";

            foreach (var case1 in hdCase)
            {
                _helpdeskService.AddCase(case1);
            }
            var cases = _helpdeskService.GetAllCases();
            return Ok(cases);
        }
    }
}