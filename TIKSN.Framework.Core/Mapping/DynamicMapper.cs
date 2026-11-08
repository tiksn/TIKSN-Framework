using System.Linq.Expressions;
using System.Reflection;
using static DotNext.Metaprogramming.CodeGenerator;

namespace TIKSN.Mapping;

/// <summary>
/// A high-performance dynamic mapper compiled at runtime using DotNext metaprogramming.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public class DynamicMapper<TSource, TDestination> : IMapper<TSource, TDestination>, IAsyncMapper<TSource, TDestination>
{
    private static readonly Lazy<Func<TSource, TDestination>> CompiledMapper = new(CreateMapper);

    public TDestination Map(TSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return CompiledMapper.Value(source);
    }

    public Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(this.Map(source));
    }

    private static Func<TSource, TDestination> CreateMapper()
    {
        var destType = typeof(TDestination);
        var sourceType = typeof(TSource);

        var destCtor = destType.GetConstructor(Type.EmptyTypes);
        if (destCtor is null && !destType.IsValueType)
        {
            throw new InvalidOperationException(
                $"Type '{destType.FullName}' must have a public parameterless constructor.");
        }

        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetGetMethod() is not null)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var destProps = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetSetMethod() is not null)
            .ToList();

        var lambda = Lambda<Func<TSource, TDestination>>(fun =>
        {
            var sourceParam = fun[0];
            var destVar = DeclareVariable<TDestination>("dest");

            Assign(destVar, Expression.New(destType));

            foreach (var destProp in destProps)
            {
                if (!sourceProps.TryGetValue(destProp.Name, out var srcProp))
                {
                    continue;
                }

                var srcExpr = Expression.Property(sourceParam, srcProp);

                if (destProp.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    Assign(Expression.Property(destVar, destProp), srcExpr);
                }
                else
                {
                    try
                    {
                        var convertExpr = Expression.Convert(srcExpr, destProp.PropertyType);
                        Assign(Expression.Property(destVar, destProp), convertExpr);
                    }
                    catch (InvalidOperationException)
                    {
                        // Types are incompatible for direct conversion; skip property
                    }
                }
            }

            Return(destVar);
        });

        return lambda.Compile();
    }
}
