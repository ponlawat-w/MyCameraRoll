using MyCameraRoll;

if (args.Length < 1) throw new Exception("Required argument to be directory path");

string directory = args[0];
if (!Directory.Exists(directory)) throw new Exception("Directory does not exist");

foreach (string filePath in Directory.GetFiles(directory))
{
    if (Path.GetFileName(filePath) == ".DS_Store") continue;
    if (Directory.Exists(filePath)) continue;
    Console.WriteLine(filePath);
    MediaFile file = new(filePath);
    string newPath = file.Rename();
    Console.WriteLine($"  -> {newPath}");
}

Console.WriteLine("Done");
