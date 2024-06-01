using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

namespace MyCameraRoll;

public class MediaFile
{
    public readonly string FilePath;

    private DateTime? _creationDate;
    public DateTime CreationDate
    {
        get
        {
            if (_creationDate != null) return (DateTime)_creationDate;
            _creationDate = File.GetCreationTime(FilePath);
            return (DateTime)_creationDate;
        }
    }

    private readonly DateTime? TakenDate = null;

    public MediaFile(string filePath)
    {
        if (!Path.Exists(filePath)) throw new FileNotFoundException(filePath);
        FilePath = filePath;
        TakenDate = GetTakenDate();
    }

    private DateTime? GetTakenDate()
    {
        IReadOnlyList<MetadataExtractor.Directory> metadata = ImageMetadataReader.ReadMetadata(FilePath);
        ExifSubIfdDirectory? exifDirectory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        if (exifDirectory?.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime takenDate) ?? false) return takenDate;

        QuickTimeMovieHeaderDirectory? quickTimeDirectory = metadata.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
        if (
            (quickTimeDirectory?.TryGetInt64(QuickTimeMovieHeaderDirectory.TagCreated, out long timestamp) ?? false)
            && timestamp > 0
            && (quickTimeDirectory?.TryGetDateTime(QuickTimeMovieHeaderDirectory.TagCreated, out takenDate) ?? false)
        ) return takenDate;

        GpsDirectory? gpsDirectory = metadata.OfType<GpsDirectory>().FirstOrDefault();
        if (gpsDirectory?.TryGetGpsDate(out takenDate) ?? false) return takenDate;

        return null;
    }

    public DateTime GetCreatedDate()
    {
        return (DateTime)(TakenDate == null ? CreationDate : TakenDate);
    }

    public string GetDateString()
    {
        DateTime createdDate = GetCreatedDate();
        string year = (int.Parse(createdDate.ToString("yyyy")) + 543).ToString();
        return createdDate.ToString($"{year}MMdd-HHmmss");
    }

    public string Rename()
    {
        string directory = Path.GetDirectoryName(FilePath) ?? "";

        if (directory == "") throw new Exception("Unable to retreive directory path");

        string extension = Path.GetExtension(FilePath).ToLower();

        string newPath;

        int i = 0;
        do
        {
            newPath = Path.Join(directory, $"{GetDateString()}-{(++i).ToString().PadLeft(2, '0')}{extension}");
        }
        while (Path.Exists(newPath));

        File.Move(FilePath, newPath);
        return newPath;
    }
}
