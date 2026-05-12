using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System; // Oh...

public static class ValueResult
{
    public static ValueResult<T, E> Ok<T, E>(T value) => value;

    public static ValueResult<T, E> Error<T, E>(E error) => error;

    public static ValueResult<T, Exception> Try<T>(Func<T> func)
    {
        try
        {
            return Ok<T, Exception>(func());
        }
        catch (Exception ex)
        {
            return Error<T, Exception>(ex);
        }
    }
}

[Union]
[StructLayout(LayoutKind.Auto)]
public readonly struct ValueResult<T, E> : IUnion
{
    readonly bool isOk;
    readonly T? value;
    readonly E? error;

    public ValueResult(T value)
    {
        this.value = value;
        this.error = default;
        this.isOk = true;
    }

    public ValueResult(E error)
    {
        this.value = default;
        this.error = error;
        this.isOk = false;
    }

    public static implicit operator ValueResult<T, E>(T value) => new(value);

    public static implicit operator ValueResult<T, E>(E error) => new(error);

    public bool IsOk => isOk;
    public bool IsError => !isOk;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValue()
    {
        if (isOk)
            return value!;

        ThrowInvalidOperationException("ValueResult does not contain a value.");
        return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E GetError()
    {
        if (!isOk)
            return error!;

        ThrowInvalidOperationException("ValueResult does not contain an error.");
        return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T UnsafeGetValue() => value!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E UnsafeGetError() => error!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetValueOr(T defaultValue) => isOk ? value! : defaultValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetValueOrDefault() => isOk ? value : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E GetErrorOr(E defaultError) => isOk ? defaultError : error!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E? GetErrorOrDefault() => isOk ? default : error;

    public object Value => isOk ? value! : error!;

    [DoesNotReturn]
    static void ThrowInvalidOperationException(string message) =>
        throw new InvalidOperationException(message);
}
