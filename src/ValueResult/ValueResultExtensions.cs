using System.Runtime.CompilerServices;

namespace System;

public static class ValueResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<S, E> Select<T, S, E>(
        this ValueResult<T, E> result,
        Func<T, S> valueSelector
    ) =>
        result.IsOk
            ? ValueResult.Ok<S, E>(valueSelector(result.UnsafeGetValue()))
            : ValueResult.Error<S, E>(result.UnsafeGetError());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<T, F> SelectError<T, E, F>(
        this ValueResult<T, E> result,
        Func<E, F> errorSelector
    ) =>
        result.IsError
            ? ValueResult.Error<T, F>(errorSelector(result.UnsafeGetError()))
            : ValueResult.Ok<T, F>(result.UnsafeGetValue());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<T, E> And<T, E>(
        this ValueResult<T, E> result0,
        ValueResult<T, E> result1
    ) => result0.IsOk ? result1 : result0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<T, E> Or<T, E>(
        this ValueResult<T, E> result0,
        ValueResult<T, E> result1
    ) => result0.IsError ? result1 : result0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<S, E> AndThen<T, S, E>(
        this ValueResult<T, E> result,
        Func<T, ValueResult<S, E>> binder
    ) =>
        result.IsOk
            ? binder(result.UnsafeGetValue())
            : ValueResult.Error<S, E>(result.UnsafeGetError());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueResult<T, F> OrElse<T, E, F>(
        this ValueResult<T, E> result,
        Func<E, ValueResult<T, F>> binder
    ) =>
        result.IsError
            ? binder(result.UnsafeGetError())
            : ValueResult.Ok<T, F>(result.UnsafeGetValue());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Match<T, E>(this ValueResult<T, E> result, Action<T> onOk, Action<E> onError)
    {
        if (result.IsOk)
            onOk(result.UnsafeGetValue());
        else
            onError(result.UnsafeGetError());
    }
}
