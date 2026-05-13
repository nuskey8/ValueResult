using NUnit.Framework;

namespace ValueResult.Tests;

public sealed class ValueResultTests
{
    [Test]
    public void Test_OkCreatesResultWithValue()
    {
        var result = System.ValueResult.Ok<int, string>(42);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.HasValue, Is.True);
        Assert.That(result.TryGetValue(out int value), Is.True);
        Assert.That(value, Is.EqualTo(42));
        Assert.That(result.TryGetValue(out string? error), Is.False);
        Assert.That(result.GetValue(), Is.EqualTo(42));
        Assert.That(result.UnsafeGetValue(), Is.EqualTo(42));
        Assert.That(result.GetValueOr(10), Is.EqualTo(42));
        Assert.That(result.GetValueOrDefault(), Is.EqualTo(42));
        Assert.That(result.GetErrorOr("fallback"), Is.EqualTo("fallback"));
        Assert.That(result.GetErrorOrDefault(), Is.Null);
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Test_ErrorCreatesResultWithError()
    {
        var result = System.ValueResult.Error<int, string>("failed");

        Assert.That(result.IsOk, Is.False);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.HasValue, Is.True);
        Assert.That(result.TryGetValue(out int value), Is.False);
        Assert.That(result.TryGetValue(out string? error), Is.True);
        Assert.That(error, Is.EqualTo("failed"));
        Assert.That(result.GetError(), Is.EqualTo("failed"));
        Assert.That(result.UnsafeGetError(), Is.EqualTo("failed"));
        Assert.That(result.GetValueOr(10), Is.EqualTo(10));
        Assert.That(result.GetValueOrDefault(), Is.EqualTo(0));
        Assert.That(result.GetErrorOr("fallback"), Is.EqualTo("failed"));
        Assert.That(result.GetErrorOrDefault(), Is.EqualTo("failed"));
        Assert.That(result.Value, Is.EqualTo("failed"));
    }

    [Test]
    public void Test_NullableOk()
    {
        var result = System.ValueResult.Ok<int?, string>(null);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.HasValue, Is.True);
        Assert.That(result.TryGetValue(out int? value), Is.True);
        Assert.That(value, Is.Null);
        Assert.That(result.TryGetValue(out string? error), Is.False);
        Assert.That(result.GetValue(), Is.Null);
        Assert.That(result.UnsafeGetValue(), Is.Null);
        Assert.That(result.GetValueOr(10), Is.Null);
        Assert.That(result.GetValueOrDefault(), Is.Null);
        Assert.That(result.GetErrorOr("fallback"), Is.EqualTo("fallback"));
        Assert.That(result.GetErrorOrDefault(), Is.Null);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public void Test_OkAndErrorWithSameTypeAreOk()
    {
        var result = System.ValueResult.Ok<int, int>(42);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.HasValue, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(42));
        Assert.That(result.UnsafeGetValue(), Is.EqualTo(42));
        Assert.That(result.GetValueOr(10), Is.EqualTo(42));
        Assert.That(result.GetValueOrDefault(), Is.EqualTo(42));
        Assert.That(result.GetErrorOr(10), Is.EqualTo(10));
        Assert.That(result.GetErrorOrDefault(), Is.EqualTo(0));
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Test_OkAndErrorWithSameTypeAreError()
    {
        var result = System.ValueResult.Error<int, int>(42);

        Assert.That(result.IsOk, Is.False);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.HasValue, Is.True);
        Assert.That(result.GetError(), Is.EqualTo(42));
        Assert.That(result.UnsafeGetError(), Is.EqualTo(42));
        Assert.That(result.GetValueOr(10), Is.EqualTo(10));
        Assert.That(result.GetValueOrDefault(), Is.EqualTo(0));
        Assert.That(result.GetErrorOr(10), Is.EqualTo(42));
        Assert.That(result.GetErrorOrDefault(), Is.EqualTo(42));
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Test_ImplicitConversionsCreateResults()
    {
        System.ValueResult<int, string> ok = 42;
        System.ValueResult<int, string> error = "failed";

        Assert.That(ok.IsOk, Is.True);
        Assert.That(ok.IsError, Is.False);
        Assert.That(ok.GetValue(), Is.EqualTo(42));
        Assert.That(error.IsOk, Is.False);
        Assert.That(error.IsError, Is.True);
        Assert.That(error.GetError(), Is.EqualTo("failed"));
    }

    [Test]
    public void Test_GetValueThrowsWhenResultIsError()
    {
        var result = System.ValueResult.Error<int, string>("failed");

        Assert.Throws<InvalidOperationException>(() => _ = result.GetValue());
    }

    [Test]
    public void Test_GetErrorThrowsWhenResultIsOk()
    {
        var result = System.ValueResult.Ok<int, string>(42);

        Assert.Throws<InvalidOperationException>(() => _ = result.GetError());
    }

    [Test]
    public void Test_TryReturnsOkWhenFunctionSucceeds()
    {
        var result = System.ValueResult.Try(() => 42);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Test_TryReturnsErrorWhenFunctionThrows()
    {
        var expected = new FormatException("bad input");

        var result = System.ValueResult.Try<int>(() => throw expected);

        Assert.That(result.IsOk, Is.False);
        Assert.That(result.IsError, Is.True);
        Assert.That(result.GetError(), Is.SameAs(expected));
    }

    [Test]
    public void Test_UnionPatternMatching()
    {
        Assert.That(System.ValueResult.Ok<int, string>(123) switch
        {
            int value => $"Value: {value}",
            string error => $"Error: {error}",
        }, Is.EqualTo("Value: 123"));

        Assert.That(System.ValueResult.Error<int, string>("failed") switch
        {
            int value => $"Value: {value}",
            string error => $"Error: {error}",
        }, Is.EqualTo("Error: failed"));
    }
}
