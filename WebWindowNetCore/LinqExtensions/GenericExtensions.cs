namespace LinqExtensions.Extensions;

public static class GenericExtensions
{
    public static T SideEffect<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    public static async Task<T> SideEffect<T>(this Task<T> value, Action<T> sideEffect)
    {
        var val = await value;
        sideEffect(val);
        return val;
    }

    public static async Task<T> SideEffect<T>(this T text, Func<T, Task> selector)
    {
        await selector(text);
        return text;
    }

    public static Task<T> ToAsync<T>(this T t) => Task.FromResult(t);

    public static void ToVoid<T>(this T _) { }
}
