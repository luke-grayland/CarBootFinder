using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CarBootFinderAPI;

public static class Constants
{
    public static class Collections
    {
        public const string Sales = "sales";
    }

    public static class Region
    {
        public const string SouthEast = "South East";
        public const string London = "London";
        public const string NorthWest = "North West";
        public const string EastOfEngland = "East Of England";
        public const string WestMidlands = "West Midlands";
        public const string SouthWest = "South West";
        public const string YorkshireAndTheHumber = "Yorkshire And The Humber";
        public const string EastMidlands = "East Midlands";
        public const string NorthEast = "North East";
        
        public static readonly List<string> AllRegions = new List<string>
        {
            SouthEast, 
            London, 
            NorthWest, 
            EastOfEngland,
            WestMidlands, 
            SouthWest, 
            YorkshireAndTheHumber,
            EastMidlands, 
            NorthEast
        };
    }

    public static class Days
    {
        public const string Monday = "Monday";
        public const string Tuesday = "Tuesday";
        public const string Wednesday = "Wednesday";
        public const string Thursday = "Thursday";
        public const string Friday = "Friday";
        public const string Saturday = "Saturday";
        public const string Sunday = "Sunday";

        public static readonly List<string> AllDays = new List<string>()
        {
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        };
    }

    public static class Environment
    {
        public const string Outdoor = "Outdoor";
        public const string Indoor = "Indoor";
        public const string Mixed = "Mixed";

        public static readonly List<string> AllEnvironments = new List<string>()
        {
            Outdoor,
            Indoor,
            Mixed
        };
    }

    public static class Search
    {
        public const double EarthRadiusKilometers = 6371;
        public const double MeterToMileMultiplier = 0.00062137;
    }
    
}