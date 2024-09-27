namespace ResourceShortageManager.Models;

public enum Room
{
    None = 0,
    MeetingRoom,
    Bathroom,
    Kitchen
}

public enum Category
{
    None = 0,
    Electronics,
    Food,
    Other
}

public enum Status
{
    None = -1,
    AlreadyExists = 0,
    AddedSuccessfully = 1,
    DoesNotExist = 2,
    RemovedSuccessfully = 3
}
