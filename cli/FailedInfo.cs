namespace MyCameraRoll;

public class FailedInfo(string filePath, Exception exception)
{
    readonly Exception Exception = exception;
    readonly string FilePath = filePath;

    public override string ToString() => $"{FilePath} - {Exception.Message}";
}
