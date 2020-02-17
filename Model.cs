
// A class containing all the different levels (or classes) in the retrieved data


namespace Weather_Report
{

    public class Stations
    {
        public Station[] station { get; set; }
        public long updated { get; set; }
        public Parameter parameter { get; set; }
        public Link[] link { get; set; }
    }

    public class Parameter
    {
        public string key { get; set; }
        public string name { get; set; }
        public string summary { get; set; }
        public string unit { get; set; }
    }

    public class Station
    {
        public string key { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public long from { get; set; }
        public long to { get; set; }
        public float height { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public long date { get; set; }
        public string value { get; set; }
        public string quality { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string type { get; set; }
        public string href { get; set; }
    }

    public class StationCurrentTemp
    {
        public string key { get; set; }
        public long updated { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public bool active { get; set; }
        public string summary { get; set; }
        public long from { get; set; }
        public long to { get; set; }
        public Period[] period { get; set; }
    }

    public class Period
    {
        public string key { get; set; }
        public long updated { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public Link[] link { get; set; }
    }

    public class StationTemp
    {
        public TempValue[] value { get; set; }
        public StationTempName station { get; set; }
    }

    public class StationTempName
    {
        public string name { get; set; }
    }

    public class TempValue
    {
        public string value { get; set; }
    }
}
