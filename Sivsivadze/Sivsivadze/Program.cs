using Sivsivadze.Domain;
using LoremNET;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
      Init();

    }

    public static void Init()
    {
        List<KaitenTask> tasks = GenerateTasks((int)Lorem.Number(50, 100));
        Console.WriteLine($"Generated {tasks.Count} items");
        KaitenTask? recentTask = GetRecentTask(tasks);
        Console.Write($"Most recent task is task with ");
        PrintTask(recentTask, false);
        Console.Write("\n");
        List<KaitenTask> highPriorityTasks = GetTasksByPriority(tasks, PriorityTypes.High);
        Console.Write($"Tasks with High priority:");
        foreach (var t in highPriorityTasks)
        {
            PrintTask(t, true);
        }
    }

    private static void PrintTask(KaitenTask task, bool isStartWithNewLine = true)
    {
        Console.Write($"{(isStartWithNewLine ? '\n' :"")}{task.Deadline}; {task.EmployeesCount} employees; {task.Priority.ToString()} priority; {task.CompletionPercentage}% completed");
    }

    private static List<KaitenTask> GetTasksByPriority(List<KaitenTask> tasks, PriorityTypes priority)
    {
        return tasks.Where(task => task.Priority.HasValue && task.Priority == priority).OrderBy(task => task.Deadline).ToList();
    }

    private static KaitenTask? GetRecentTask(List<KaitenTask> tasks)
    {
        return tasks.Where(task => task.Deadline.HasValue)
            .OrderBy(task => task.Deadline.Value)
            .FirstOrDefault();
    }

    private static List<KaitenTask> GenerateTasks(int quantity)
    {
        List<KaitenTask> output = new();
        PriorityTypes[] allTypes = new PriorityTypes[(int)PriorityTypes.High + 1];
        for (int i = 0; i < allTypes.Length; i++)
        {
            allTypes[i] = (PriorityTypes)i;
        }
        for (int i = 0; i < quantity; i++)
        {  
            DateTime dateTime = Lorem.DateTime(new DateTime(2002, 3, 27), DateTime.Now);
            int count = (int)Lorem.Number(1, 5);
            PriorityTypes priority = Lorem.Random<PriorityTypes>(allTypes);
            int percentage = (int)Lorem.Number(0, 100);
            output.Add(new KaitenTask(dateTime, count, priority, percentage));
        }
        return output;
    }

}