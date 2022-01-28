namespace ApexDataExtracter.Models.Apex;

public class Status
{
    public Istat istat { get; set; }
}

public class Extra
{
    public string sdver { get; set; }
}

public class Feed
{
    public int name { get; set; }
    public int active { get; set; }
}

public class Power
{
    public int failed { get; set; }
    public int restored { get; set; }
}

public class Input
{
    public string did { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public double value { get; set; }

    public bool ToBool()
    {
       if(value == 0)
           return false;
       return true;
    }
}

public class Output
{
    public List<string> status { get; set; }
    public string name { get; set; }
    public string gid { get; set; }
    public string type { get; set; }
    public int ID { get; set; }
    public string did { get; set; }
}

public class Link
{
    public int linkState { get; set; }
    public string linkKey { get; set; }
    public bool link { get; set; }
}

public class Istat
{
    public string hostname { get; set; }
    public string software { get; set; }
    public string hardware { get; set; }
    public string serial { get; set; }
    public string type { get; set; }
    public Extra extra { get; set; }
    public string timezone { get; set; }
    public int date { get; set; }
    public Feed feed { get; set; }
    public Power power { get; set; }
    public List<Input> inputs { get; set; }
    public List<Output> outputs { get; set; }
    public Link link { get; set; }

    public Input? GetInputByDid(string did)
    {
        return inputs.SingleOrDefault(x => x.did == did);
    }
    public Input? GetInputByName(string name)
    {
        return inputs.SingleOrDefault(x => x.name == name);
    }
    
    public Output? GetOutputByName(string name)
    {
        return outputs.SingleOrDefault(x => x.name == name);
    }
}

