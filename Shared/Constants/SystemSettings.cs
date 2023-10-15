namespace CarBootFinderAPI.Shared.Constants;

public static class SystemSettings
{
    public const double SearchMaxDistanceKilometers = 160;
    public const string AdminEmailAddress = "lazygraylabs@gmail.com";
    public const int SmtpServerPort = 587;
    // public const string EmailTimerCronExpression = "0 0 7 * * *";
    public const string EmailTimerCronExpression = "* * * * *";
}