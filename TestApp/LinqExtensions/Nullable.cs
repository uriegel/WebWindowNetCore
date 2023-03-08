namespace LinqExtensions.Extensions;

static class GenericNullable
{
    public static R? Select<T, R>(this T? opt, Func<T, R> func) 
        where T : class
        where R : class 
        => opt != null
        ? func(opt)
        : null;

    public static R? Select<T, R>(this T? opt, Func<T, R> func) 
        where T : struct
        where R : struct 
        => opt.HasValue
        ? func(opt.Value)
        : null;

    public static RR? SelectMany<T, R, RR>(this T? t, Func<T, R?> bind, Func<T, R, RR> project)
        where RR : class
        where R : class
        where T : class
        => t != null
            ? bind(t) switch 
            {
                null => null,
                R r => project(t, r)
            }
            : null;
    
    public static Nullable<RR> SelectMany<T, R, RR>(this T? t, Func<T, R?> bind, Func<T, R, RR> project)
        where RR : struct
        where R : struct
        where T : struct
        => t.HasValue
            ? bind(t.Value) switch 
            {
                null => null,
                R r => project(t.Value, r)
            }
            : null;

    public static T GetOrDefault<T>(this T? t, T defaultValue)
        where T: class
        => t switch
        {
            null => defaultValue,
            _ => t
        };

    public static T GetOrDefault<T>(this T? t, T defaultValue)
        where T : struct
        => t.HasValue
            ? t.Value
            : defaultValue;

    public static void ForEach<T>(this T? opt, Action<T> action)
        where T : class
    {
        if (opt != null)
            action(opt);
    }

    public static void ForEach<T>(this T? opt, Action<T> action)
        where T : struct
    {
        if (opt.HasValue)
            action(opt.Value);
    }
}