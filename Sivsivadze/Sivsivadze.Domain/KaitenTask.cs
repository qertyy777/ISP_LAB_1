namespace Sivsivadze.Domain;

public enum PriorityTypes
{
    Backlog,
    Low,
    Medium,
    High
}

public class KaitenTask
{
    public DateTime Deadline { get; set; }
    public int EmployeesCount { get; set; }
    public PriorityTypes Priority { get; set; }
    public int CompletionPercentage { get; set; }

    public KaitenTask()
    {
    }

    public KaitenTask(
        DateTime deadline,
        int employeesCount,
        PriorityTypes priority,
        int completionPercentage)
    {
        Deadline = deadline;
        EmployeesCount = employeesCount;
        Priority = priority;
        CompletionPercentage = completionPercentage;
    }
}
