using System;
using System.Collections.Generic;
using System.Linq;

namespace ITHelpdeskAPI.Services
{
    public class HelpdeskService
    {
        private readonly List<HelpdeskCase> _cases;

        public HelpdeskService()
        {
            _cases = new List<HelpdeskCase>
            {
                new HelpdeskCase
                {
                    Id = Guid.NewGuid().ToString(),
                    OpenedBy = "John Freeman",
                    OpenedDate = DateTime.UtcNow,
                    Title = "Problem with VPN",
                    Description = "VPN will not connect.",
                    AssignedTo = "Shri Ilich",
                    ClosedDate = null,
                    Priority = "Normal",
                    Status = "Open"
                },
                new HelpdeskCase
                {
                    Id = Guid.NewGuid().ToString(),
                    OpenedBy = "Steve Rogers",
                    OpenedDate = DateTime.UtcNow,
                    Title = "Locked out of computer",
                    Description = "Computer will turn on, but does not accept password.",
                    AssignedTo = "Bob Smith",
                    ClosedDate = null,
                    Priority = "Urgent",
                    Status = "Open"
                },
                new HelpdeskCase
                {
                    Id = Guid.NewGuid().ToString(),
                    OpenedBy = "Bharath Kumar",
                    OpenedDate = DateTime.UtcNow,
                    Title = "Windows update failure",
                    Description = "Windows update fails to install.",
                    AssignedTo = "Shri Ilich",
                    ClosedDate = null,
                    Priority = "Normal",
                    Status = "Open"
                }
            };
            


        }

        public IEnumerable<HelpdeskCase> GetAllCases()
        {
            return _cases;
        }

        public HelpdeskCase GetCaseById(string id)
        {
            return _cases.FirstOrDefault(c => c.Id == id)!;
        }

        public void AddCase(HelpdeskCase helpdeskCase)
        {
            helpdeskCase.Id = Guid.NewGuid().ToString();
            helpdeskCase.OpenedDate = DateTime.UtcNow;
            helpdeskCase.ClosedDate= null;
            helpdeskCase.Status = "Open";
            _cases.Add(helpdeskCase);
        }

        public void UpdateCase(string id, HelpdeskCase updatedCase)
        {
            var existingCase = GetCaseById(id);
            if (existingCase != null)
            {
                existingCase.OpenedBy = updatedCase.OpenedBy;
                existingCase.OpenedDate = updatedCase.OpenedDate;
                existingCase.Title = updatedCase.Title;
                existingCase.Description = updatedCase.Description;
                existingCase.AssignedTo = updatedCase.AssignedTo;
                existingCase.ClosedDate = updatedCase.ClosedDate;
                existingCase.Priority = updatedCase.Priority;
                existingCase.Status = updatedCase.Status;
            }
        }

        public void DeleteCase(string id)
        {
            var helpdeskCase = GetCaseById(id);
            if (helpdeskCase != null)
            {
                _cases.Remove(helpdeskCase);
            }
        }
    }
}