namespace BuildingBlocks.Results;

/// <summary>
/// Result of an operation that has no return value beyond success/failure.
/// Application-layer handlers should return this (or Result&lt;T&gt;) instead of
/// throwing for expected failure paths (AGENTS.md section 12).
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public IReadOnlyList<string> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        Error = errors.Count > 0 ? errors[0] : null;
    }

    public static Result Success() => new(true, Array.Empty<string>());

    public static Result Failure(string error) => new(false, new[] { error });

    public static Result Failure(IEnumerable<string> errors) => new(false, errors.ToArray());

    public bool IsFailure => !IsSuccess;
}

/// <summary>
/// Result of an operation that returns a value of type <typeparamref name="T"/> on success.
/// </summary>
public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    public new static Result<T> Failure(string error) => new(false, default, new[] { error });

    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors.ToArray());

    public static implicit operator Result<T>(T value) => Success(value);
}
