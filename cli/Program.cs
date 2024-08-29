using MyCameraRoll;

if (args.Length < 1) throw new Exception("Required argument to be directory path");

string directory = args[0];
if (!Directory.Exists(directory)) throw new Exception("Directory does not exist");

List<FailedInfo> failedFiles = [];
foreach (string filePath in Directory.GetFiles(directory))
{
    if (Path.GetFileName(filePath) == ".DS_Store") continue;
    if (Directory.Exists(filePath)) continue;
    Console.Write(Path.GetFileName(filePath));
    try
    {
        MediaFile file = new(filePath);
        RenameResult result = file.Rename();
        Console.WriteLine(result.Renamed ? $" -> {result.FileName}" : " -x (Ignored)");
    }
    catch (Exception ex)
    {
        failedFiles.Add(new(filePath, ex));
        Console.WriteLine(" -x FAILED");
    }
}

if (failedFiles.Count > 0) foreach (FailedInfo failedFile in failedFiles) Console.WriteLine(failedFile.ToString());

Console.WriteLine("Done");
