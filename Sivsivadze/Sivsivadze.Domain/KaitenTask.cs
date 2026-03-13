
namespace Sivsivadze.Domain
{
    public enum PriorityTypes
    {
        Backlog,
        Low,
        Medium,
        High
    }

    
    public class KaitenTask
    {
        public DateTime? Deadline { get; private set; }
        public int? EmployeesCount { get; private set; }
        public PriorityTypes? Priority { get; private set; }
        public float? CompletionPercentage { get; private set; }

        public KaitenTask(DateTime? deadline, int? employeesCount, PriorityTypes? priority, float? completionPercentage)
        {
            Deadline = deadline;
            EmployeesCount = employeesCount;
            Priority = priority;
            CompletionPercentage = completionPercentage;
        }
    }
}
