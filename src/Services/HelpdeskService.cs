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
                    Id = "0f66832a-35f8-4e79-a97c-53f1a366cd15,
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
                    Id = "ce7fb80a-571d-4948-b5fe-944ba3e48a8c",
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
                    Id = "173a8447-5bf6-4047-8af9-1ee76000bc09",
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