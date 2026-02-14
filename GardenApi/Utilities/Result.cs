namespace GardenApi.Utilities.Results;

public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful or not. 
    /// This is a read-only property that is set when the Result object is created.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Contains the error message if the operation failed. 
    /// This is null if the operation was successful.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Indicates whether the operation failed. 
    /// This is the inverse of IsSuccess.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Protected constructor to initialize the Result object. 
    /// The success flag and error message are set based on the parameters provided. 
    /// This constructor is not intended to be called directly; 
    /// instead, use the static Success and Failure methods to create instances of Result.
    /// </summary>
    /// <param name="success">Whether the operation was successful.</param>
    /// <param name="error">The error message if the operation failed.</param>
    protected Result(bool success, string? error = null)
    {
        IsSuccess = success;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result. The error message is null in this case.
    /// </summary>
    /// <returns></returns>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failed result with the provided error message. The success flag is false in this case.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result Failure(string error) => new(false, error);
}