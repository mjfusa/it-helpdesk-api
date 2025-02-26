using System;
using Swashbuckle.AspNetCore.Annotations;

public class HelpdeskCase
{
     public Guid Id { get; set; }
    public string? OpenedBy { get; set; }
    public DateTime OpenedDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? ClosedDate { get; set; }
    public string? Priority { get; set; }
    public string? Status { get; set; }
}