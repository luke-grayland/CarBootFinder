using System.Runtime.Serialization;

namespace CarBootFinderAPI;

public static class Constants
{
    public static class Collections
    {
        public const string Sales = "sales";
    }

    public enum Region
    {
        [EnumMember(Value = "South East")]
        SouthEast,
        [EnumMember(Value = "London")]
        London,
        [EnumMember(Value = "North West")]
        NorthWest,
        [EnumMember(Value = "East of England")]
        EastOfEngland,
        [EnumMember(Value = "West Midlands")]
        WestMidlands,
        [EnumMember(Value = "South West")]
        SouthWest,
        [EnumMember(Value = "Yorkshire and the Humber")]
        YorkshireAndTheHumber,
        [EnumMember(Value = "East Midlands")]
        EastMidlands,
        [EnumMember(Value = "North East")]
        NorthEast
    }

    public enum Days
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum Environment
    {
        Outdoor,
        Indoor,
        Mixed
    }
    
}