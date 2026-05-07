using System.Linq.Expressions;

namespace Sivsivadze.Domain;

public static class ExpressionBuilder
{
    public static Expression<Func<KaitenTask, bool>> Build(FilterData filter)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(KaitenTask), "task");
        Expression? expression = null;

        expression = AppendEquals(expression, parameter, nameof(KaitenTask.Deadline), filter.Deadline);
        expression = AppendEquals(expression, parameter, nameof(KaitenTask.EmployeesCount), filter.EmployeesCount);
        expression = AppendEquals(expression, parameter, nameof(KaitenTask.Priority), filter.Priority);
        expression = AppendEquals(expression, parameter, nameof(KaitenTask.CompletionPercentage), filter.CompletionPercentage);

        expression ??= Expression.Constant(true);
        return Expression.Lambda<Func<KaitenTask, bool>>(expression, parameter);
    }

    private static Expression? AppendEquals<T>(
        Expression? current,
        ParameterExpression parameter,
        string propertyName,
        T? value)
    {
        if (value is null)
        {
            return current;
        }

        MemberExpression property = Expression.Property(parameter, propertyName);
        ConstantExpression constant = Expression.Constant(value, property.Type);
        BinaryExpression equals = Expression.Equal(property, constant);

        return current is null ? equals : Expression.AndAlso(current, equals);
    }
}
