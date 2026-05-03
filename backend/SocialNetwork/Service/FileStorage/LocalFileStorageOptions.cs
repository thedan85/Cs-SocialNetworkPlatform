namespace SocialNetwork.Service;

public class LocalFileStorageOptions
{
    public const string SectionName = "LocalFileStorage";

    public string UploadsPath { get; set; } = "uploads/images";
}
