namespace SpeedGameApp.Business.Common;

/// <summary>
///     Represents the result of an operation that can succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public readonly record struct Result<T>
{
    /// <summary>
    ///     Gets the value if the operation succeeded.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    ///     Gets the error message if the operation failed.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The value to return.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Success(T value) => new()
    {
        Value = value,
        IsSuccess = true,
        Error = string.Empty
    };

    /// <summary>
    ///     Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failure(string error) => new()
    {
        Value = default,
        IsSuccess = false,
        Error = error
    };
}

/// <summary>
///     Represents the result of an operation without a return value.
/// </summary>
public readonly record struct Result
{
    /// <summary>
    ///     Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    ///     Gets the error message if the operation failed.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new()
    {
        IsSuccess = true,
        Error = string.Empty
    };

    /// <summary>
    ///     Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
