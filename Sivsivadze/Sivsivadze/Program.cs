using System.Reflection;
using Microsoft.Extensions.Configuration;
using Sivsivadze.Domain;
using Sivsivadze.Features;
using Sivsivadze.Mediator;

internal class Program
{
    private static readonly Random Random = new();

    private static void Main()
    {
        List<KaitenTask> tasks = GenerateTasks(Random.Next(50, 101));
        Console.WriteLine($"Generated {tasks.Count} project tasks");

        KaitenTask firstTaskToFinish = GetFirstTaskToFinish(tasks);
        Console.WriteLine();
        Console.WriteLine("Task that must finish first:");
        PrintTask(firstTaskToFinish);

        Console.WriteLine();
        Console.WriteLine("Tasks grouped by priority:");
        PrintTasksGroupedByPriority(tasks);

        Console.WriteLine();
        Console.WriteLine("Filtering examples:");
        foreach (FilterData filter in CreateFilters(tasks))
        {
            PrintFilterResult(tasks, filter);
        }

        string fileName = GetDataFileName();
        Assembly featuresAssembly = Assembly.GetAssembly(typeof(SaveDataRequest))
                                    ?? throw new InvalidOperationException("Features assembly was not found.");
        ISender sender = new Sender(featuresAssembly);

        sender.Send(new SaveDataRequest(tasks, fileName));
        IEnumerable<KaitenTask> loadedTasks = sender.Send(new ReadDataRequest(fileName));

        Console.WriteLine();
        Console.WriteLine($"Collection was saved to and loaded from file: {fileName}");
        Console.WriteLine("First task to finish from loaded collection:");
        PrintTask(GetFirstTaskToFinish(loadedTasks));
    }

    private static string GetDataFileName()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        string fileName = configuration["Storage:FileName"]
                          ?? throw new InvalidOperationException("Storage:FileName is not configured.");

        return Path.IsPathRooted(fileName)
            ? fileName
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, fileName));
    }

    private static List<KaitenTask> GenerateTasks(int quantity)
    {
        PriorityTypes[] priorities = Enum.GetValues<PriorityTypes>();
        List<KaitenTask> output = new(quantity);

        for (int i = 0; i < quantity; i++)
        {
            output.Add(new KaitenTask(
                DateTime.Today.AddDays(Random.Next(1, 181)),
                Random.Next(1, 6),
                priorities[Random.Next(priorities.Length)],
                Random.Next(0, 101)));
        }

        return output;
    }

    private static KaitenTask GetFirstTaskToFinish(IEnumerable<KaitenTask> tasks)
    {
        return tasks.OrderBy(task => task.Deadline).First();
    }

    private static void PrintTasksGroupedByPriority(IEnumerable<KaitenTask> tasks)
    {
        var groupedTasks = tasks
            .GroupBy(task => task.Priority)
            .OrderBy(group => group.Key);

        foreach (var group in groupedTasks)
        {
            Console.WriteLine($"{group.Key}: {group.Count()} task(s)");

            foreach (KaitenTask task in group.OrderBy(task => task.Deadline))
            {
                PrintTask(task, "  ");
            }
        }
    }

    private static IEnumerable<FilterData> CreateFilters(List<KaitenTask> tasks)
    {
        KaitenTask sample = tasks[0];
        KaitenTask earliest = GetFirstTaskToFinish(tasks);

        return
        [
            new FilterData { Priority = PriorityTypes.High },
            new FilterData { EmployeesCount = sample.EmployeesCount, Priority = sample.Priority },
            new FilterData { CompletionPercentage = sample.CompletionPercentage },
            new FilterData { Deadline = earliest.Deadline }
        ];
    }

    private static void PrintFilterResult(IEnumerable<KaitenTask> tasks, FilterData filter)
    {
        Func<KaitenTask, bool> predicate = ExpressionBuilder.Build(filter).Compile();
        List<KaitenTask> filteredTasks = tasks
            .Where(predicate)
            .OrderBy(task => task.Deadline)
            .ToList();

        Console.WriteLine();
        Console.WriteLine(DescribeFilter(filter));
        Console.WriteLine($"Found {filteredTasks.Count} task(s)");

        foreach (KaitenTask task in filteredTasks)
        {
            PrintTask(task, "  ");
        }
    }

    private static string DescribeFilter(FilterData filter)
    {
        List<string> values = [];

        if (filter.Deadline.HasValue)
        {
            values.Add($"deadline = {filter.Deadline:yyyy-MM-dd}");
        }

        if (filter.EmployeesCount.HasValue)
        {
            values.Add($"employees = {filter.EmployeesCount}");
        }

        if (filter.Priority.HasValue)
        {
            values.Add($"priority = {filter.Priority}");
        }

        if (filter.CompletionPercentage.HasValue)
        {
            values.Add($"completion = {filter.CompletionPercentage}%");
        }

        return "Filter: " + string.Join(", ", values);
    }

    private static void PrintTask(KaitenTask task, string prefix = "")
    {
        Console.WriteLine(
            $"{prefix}{task.Deadline:yyyy-MM-dd}; {task.EmployeesCount} employee(s); {task.Priority}; {task.CompletionPercentage}% completed");
    }
}
