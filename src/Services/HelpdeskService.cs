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
            _cases = new List<HelpdeskCase>();
        }

        public IEnumerable<HelpdeskCase> GetAllCases()
        {
            return _cases;
        }

        public HelpdeskCase GetCaseById(Guid  id)
        {
            return _cases.FirstOrDefault(c => c.Id == id)!;
        }

        public void AddCase(HelpdeskCase helpdeskCase)
        {
            helpdeskCase.Id = Guid.NewGuid();
            helpdeskCase.OpenedDate = DateTime.UtcNow;
            helpdeskCase.ClosedDate= null;
            helpdeskCase.Status = "Open";
            _cases.Add(helpdeskCase);
        }

        public void UpdateCase(Guid id, HelpdeskCase updatedCase)
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

        public void DeleteCase(Guid id)
        {
            var helpdeskCase = GetCaseById(id);
            if (helpdeskCase != null)
            {
                _cases.Remove(helpdeskCase);
            }
        }
    }
}