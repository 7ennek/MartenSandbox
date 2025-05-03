namespace EventsourcingSandbox.Helpers;

public static class Some
{
    public static Guid Guid => Guid.NewGuid();
    public static string String => Guid.NewGuid().ToString();
}