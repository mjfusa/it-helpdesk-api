using ITHelpdeskAPI.Services;
using ITHelpdeskAPI.Models;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ITHelpdeskAPI.Tools
{
    [McpServerToolType]
    public class HelpdeskMcpTools
    {
        private readonly HelpdeskService _helpdeskService;

        public HelpdeskMcpTools(HelpdeskService helpdeskService)
        {
            _helpdeskService = helpdeskService;
        }

        [McpServerTool]
        [Description("Get all helpdesk cases from the IT helpdesk system")]
        public IEnumerable<HelpdeskCase> GetAllHelpdeskCases()
        {
            return _helpdeskService.GetAllCases();
        }

        [McpServerTool]
        [Description("Get a specific helpdesk case by its ID")]
        public HelpdeskCase? GetHelpdeskCase(
            [Required, Description("The ID of the helpdesk case to retrieve")]
            string caseId)
        {
            return _helpdeskService.GetCaseById(caseId);
        }

        [McpServerTool]
        [Description("Create a new helpdesk case")]
        public HelpdeskCase CreateHelpdeskCase(
            [Required, Description("Title of the helpdesk case")]
            string title,
            [Required, Description("Description of the problem")]
            string description,
            [Required, Description("Person who opened the case")]
            string openedBy,
            [Description("Priority level (Normal, High, Urgent)")]
            string priority = "Normal",
            [Description("User assigned to handle the case")]
            string? assignedTo = null)
        {
            var newCase = new HelpdeskCase
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Description = description,
                OpenedBy = openedBy,
                OpenedDate = DateTime.UtcNow,
                Priority = priority,
                AssignedTo = assignedTo,
                Status = "Open"
            };

            _helpdeskService.AddCase(newCase);
            return newCase;
        }

        [McpServerTool]
        [Description("Update an existing helpdesk case")]
        public HelpdeskCase? UpdateHelpdeskCase(
            [Required, Description("ID of the case to update")]
            string caseId,
            [Description("New title for the case")]
            string? title = null,
            [Description("New description for the case")]
            string? description = null,
            [Description("New status (Open, In Progress, Resolved, Closed)")]
            string? status = null,
            [Description("New priority (Normal, High, Urgent)")]
            string? priority = null,
            [Description("User to assign the case to")]
            string? assignedTo = null)
        {
            var existingCase = _helpdeskService.GetCaseById(caseId);
            if (existingCase == null)
            {
                return null;
            }

            var updatedCase = new HelpdeskCase
            {
                Id = caseId,
                Title = title ?? existingCase.Title,
                Description = description ?? existingCase.Description,
                Status = status ?? existingCase.Status,
                Priority = priority ?? existingCase.Priority,
                AssignedTo = assignedTo ?? existingCase.AssignedTo,
                OpenedBy = existingCase.OpenedBy,
                OpenedDate = existingCase.OpenedDate,
                ClosedDate = status == "Closed" ? DateTime.UtcNow : existingCase.ClosedDate
            };

            _helpdeskService.UpdateCase(caseId, updatedCase);
            return updatedCase;
        }

        [McpServerTool]
        [Description("Delete a helpdesk case")]
        public bool DeleteHelpdeskCase(
            [Required, Description("ID of the case to delete")]
            string caseId)
        {
            var existingCase = _helpdeskService.GetCaseById(caseId);
            if (existingCase == null)
            {
                return false;
            }

            _helpdeskService.DeleteCase(caseId);
            return true;
        }

        [McpServerTool]
        [Description("Search helpdesk cases by title, description, or assigned user")]
        public IEnumerable<HelpdeskCase> SearchHelpdeskCases(
            [Description("Search term to look for in title and description")]
            string? searchTerm = null,
            [Description("Filter by status")]
            string? status = null,
            [Description("Filter by priority")]
            string? priority = null,
            [Description("Filter by assigned user")]
            string? assignedTo = null)
        {
            var cases = _helpdeskService.GetAllCases();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                cases = cases.Where(c =>
                    (c.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (!string.IsNullOrEmpty(status))
            {
                cases = cases.Where(c => c.Status?.Equals(status, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (!string.IsNullOrEmpty(priority))
            {
                cases = cases.Where(c => c.Priority?.Equals(priority, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (!string.IsNullOrEmpty(assignedTo))
            {
                cases = cases.Where(c => c.AssignedTo?.Equals(assignedTo, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            return cases;
        }
    }
}