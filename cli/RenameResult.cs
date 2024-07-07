namespace MyCameraRoll;

public class RenameResult(string newName, bool renamed)
{
    public readonly string FileName = newName;
    public readonly bool Renamed = renamed;
}
