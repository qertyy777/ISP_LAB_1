namespace Sivsivadze.Domain;

public class FilterData
{
    public DateTime? Deadline { get; set; }
    public int? EmployeesCount { get; set; }
    public PriorityTypes? Priority { get; set; }
    public int? CompletionPercentage { get; set; }
}
