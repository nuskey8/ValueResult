# ValueResult

[![Nuget](https://img.shields.io/nuget/v/valueresult.svg)](https://www.nuget.org/packages/ValueResult/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A lightweight, zero-allocation result type implementation for C#. 

`ValueResult<T, E>` is a discriminated union that represents either a successful value (`Ok`) or an error value (`Error`), enabling functional error handling without exceptions.

## Features

- **Zero-allocation struct**: Implemented as a `readonly struct` for maximum performance
- **Functional API**: `Select`, `AndThen`, `OrElse`, `Match` operations for composable error handling
- **Exception handling**: Built-in `Try<T>` helper to convert exceptions to results

## Installation

```bash
dotnet add package ValueResult
```

## Quick Start

### Creating Results

```csharp
// Create an Ok result with implicit conversion
ValueResult<int, string> okResult = 42;

// Create an Error result with implicit conversion
ValueResult<int, string> errorResult = "Something went wrong";

// Using helper methods
var success = ValueResult.Ok<int, string>(100);
var failure = ValueResult.Error<int, string>("Error message");
```

### Checking Results

```csharp
var result = ValueResult.Ok<int, string>(42);

if (result.IsOk)
{
    int value = result.GetValue();
    Console.WriteLine($"Success: {value}");
}

if (result.IsError)
{
    string error = result.GetError();
    Console.WriteLine($"Error: {error}");
}
```

### Extracting Values

```csharp
var result = ValueResult.Ok<int, string>(42);

int value = result.GetValue();
int valueOrDefault = result.GetValueOr(0);
int maybeValue = result.GetValueOrDefault();
int unsafeValue = result.UnsafeGetValue();
```

### Exception Handling

Convert exceptions to results with the `Try<T>` helper:

```csharp
var result = ValueResult.Try(() => int.Parse("not-a-number"));

if (result.IsError)
{
    Exception ex = result.GetError();
    Console.WriteLine($"Parsing failed: {ex.Message}");
}
```

## Operators

### Select

```csharp
var result = ValueResult.Ok<int, string>(10);

var doubled = result.Select(x => x * 2);
// doubled is Ok<int, string>(20)

var asString = result.Select(x => $"Value: {x}");
// asString is Ok<string, string>("Value: 10")
```

### SelectError

```csharp
var result = ValueResult.Error<int, string>("error");

var httpStatus = result.SelectError(err => 500);
// httpStatus is Error<int, int>(500)
```

### AndThen

```csharp
ValueResult<int, string> ParseInt(string input)
{
    if (int.TryParse(input, out var value))
        return value;
    return "Invalid integer";
}

ValueResult<int, string> Divide(int a, int b)
{
    if (b == 0)
        return "Division by zero";
    return a / b;
}

var result = ParseInt("10")
    .AndThen(x => Divide(x, 2));
// Success: 5
```

### OrElse

```csharp
var result = ValueResult.Error<int, string>("First error")
    .OrElse(err => 
    {
        if (err == "First error")
            return 42; // Recover with fallback value
        return err; // Propagate other errors
    });
// Success: 42
```

### And

```csharp
var result1 = ValueResult.Ok<int, string>(10);
var result2 = ValueResult.Ok<int, string>(20);

var combined = result1.And(result2);
// combined is result2 (since result1 is Ok)
```

### Or

```csharp
var result1 = ValueResult.Error<int, string>("Failed");
var result2 = ValueResult.Ok<int, string>(42);

var fallback = result1.Or(result2);
// fallback is Ok<int, string>(42)
```

### Match

```csharp
var result = ValueResult.Ok<int, string>(42);

result.Match(
    value => Console.WriteLine($"Success: {value}"),
    error => Console.WriteLine($"Error: {error}")
);

// C# 15 union also supported
_ = result switch
{
    int value => $"Success: {value}",
    string error => $"Error: {error}",
};
```

## License

This library is provided under the [MIT License](LICENSE)
