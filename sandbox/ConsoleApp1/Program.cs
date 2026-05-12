var result1 = ValueResult.Try(() => 42);
Console.WriteLine(result1.IsOk); // True

var result2 = result1.Select(x => x.ToString());
Console.WriteLine(result2.GetValue()); // "42"

var result3 = ValueResult.Try(RaiseError);
Console.WriteLine(result3.IsError); // True
Console.WriteLine(result3.GetError().Message); // "An error occurred."

var result4 = ValueResult.Ok<int, string>(100);
result4
    .AndThen(x => ValueResult.Ok<string, string>(x.ToString()))
    .AndThen(x => ValueResult.Ok<int, string>(x.Length))
    .AndThen(x => ValueResult.Ok<string, string>($"Length: {x}"))
    .Match(
        static ok => Console.WriteLine(ok), // "Length: 3"
        static error => Console.WriteLine($"Error: {error}")
    );

// C# 15 union pattern matching
_ = ValueResult.Ok<int, string>(123) switch
{
    int value => $"Value: {value}",
    string error => $"Error: {error}",
};

static int RaiseError()
{
    throw new InvalidOperationException("An error occurred.");
}
