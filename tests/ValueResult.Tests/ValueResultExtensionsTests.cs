using NUnit.Framework;

namespace ValueResult.Tests;

public sealed class ValueResultExtensionsTests
{
    [Test]
    public void Test_Select_TransformsOkValue()
    {
        var result = System.ValueResult.Ok<int, string>(21).Select(value => value * 2);

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Test_Select_PreservesErrorWithoutCallingSelector()
    {
        var result = System.ValueResult.Error<int, string>("error").Select<int, string, string>(
            _ => throw new InvalidOperationException("Selector should not run.")
        );

        Assert.That(result.IsError, Is.True);
        Assert.That(result.GetError(), Is.EqualTo("error"));
    }

    [Test]
    public void Test_Select_ErrorTransformsError()
    {
        var result = System.ValueResult.Error<int, string>("missing").SelectError(error => error.Length);

        Assert.That(result.IsError, Is.True);
        Assert.That(result.GetError(), Is.EqualTo(7));
    }

    [Test]
    public void Test_Select_ErrorPreservesOkWithoutCallingSelector()
    {
        var result = System.ValueResult.Ok<int, string>(42).SelectError<int, string, int>(
            _ => throw new InvalidOperationException("Selector should not run.")
        );

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Test_And_ReturnsSecondResultWhenFirstIsOk()
    {
        var result = System.ValueResult.Ok<int, string>(1).And(System.ValueResult.Ok<int, string>(2));

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(2));
    }

    [Test]
    public void Test_And_ReturnsFirstErrorWhenFirstIsError()
    {
        var result = System.ValueResult.Error<int, string>("first").And(System.ValueResult.Ok<int, string>(2));

        Assert.That(result.IsError, Is.True);
        Assert.That(result.GetError(), Is.EqualTo("first"));
    }

    [Test]
    public void Test_Or_ReturnsFirstResultWhenFirstIsOk()
    {
        var result = System.ValueResult.Ok<int, string>(1).Or(System.ValueResult.Ok<int, string>(2));

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(1));
    }

    [Test]
    public void Test_Or_ReturnsSecondResultWhenFirstIsError()
    {
        var result = System.ValueResult.Error<int, string>("first").Or(System.ValueResult.Ok<int, string>(2));

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(2));
    }

    [Test]
    public void Test_AndThen_BindsOkValue()
    {
        var result = System.ValueResult.Ok<int, string>(21).AndThen(value => System.ValueResult.Ok<int, string>(value * 2));

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Test_AndThen_PreservesErrorWithoutCallingBinder()
    {
        var result = System.ValueResult.Error<int, string>("error").AndThen<int, int, string>(
            _ => throw new InvalidOperationException("Binder should not run.")
        );

        Assert.That(result.IsError, Is.True);
        Assert.That(result.GetError(), Is.EqualTo("error"));
    }

    [Test]
    public void Test_OrElse_BindsErrorValue()
    {
        var result = System.ValueResult.Error<int, string>("recover").OrElse(error => System.ValueResult.Ok<int, int>(error.Length));

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(7));
    }

    [Test]
    public void Test_OrElse_PreservesOkWithoutCallingBinder()
    {
        var result = System.ValueResult.Ok<int, string>(42).OrElse<int, string, int>(
            _ => throw new InvalidOperationException("Binder should not run.")
        );

        Assert.That(result.IsOk, Is.True);
        Assert.That(result.GetValue(), Is.EqualTo(42));
    }

    [Test]
    public void Test_Match_InvokesOkCallbackForOk()
    {
        var matched = string.Empty;

        System.ValueResult.Ok<int, string>(42).Match(
            value => matched = $"ok:{value}",
            error => matched = $"error:{error}"
        );

        Assert.That(matched, Is.EqualTo("ok:42"));
    }

    [Test]
    public void Test_Match_InvokesErrorCallbackForError()
    {
        var matched = string.Empty;

        System.ValueResult.Error<int, string>("failed").Match(
            value => matched = $"ok:{value}",
            error => matched = $"error:{error}"
        );

        Assert.That(matched, Is.EqualTo("error:failed"));
    }
}
