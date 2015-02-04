public class enumeratedTypes
{
    public enum enumPhotoOrientation
    {
        Landscape = 1,
        Portrait = 2
    }

    public enum enumPhotoInstanceType
    {
        Thumbnail = 1,
        Large = 2,
        HighRes = 3,
        Custom = 4,
        Small = 5,
        Medium = 6
    }

    public enum enumNewsItemState
    {
        None,
        Draft,
        Approval,
        Live,
        Deleted
    }
}