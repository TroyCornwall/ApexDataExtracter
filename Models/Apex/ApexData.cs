namespace ApexDataExtracter.Models.Apex;

public class ApexData
{
    public ApexData(Status status)
    {
        //Convert horribad format into something usable..
        Timestamp = DateTimeOffset.Now; //TODO: nzdt 
        ApexName = status.istat.hostname;
        Temp = status.istat.GetInputByDid("base_Temp")?.value ?? Double.NaN;
        Ph = status.istat.GetInputByDid("base_pH")?.value ?? Double.NaN;
        Alk = status.istat.GetInputByName("Alkx4")?.value ?? Double.NaN;
        Calc = status.istat.GetInputByName("Cax4")?.value ?? Double.NaN;
        Mg = status.istat.GetInputByName("Mgx4")?.value ?? Double.NaN;
        ATO = status.istat.GetOutputByName("ATO");
        ATOLow = status.istat.GetInputByName("ATO_Lo")?.ToBool() ?? false;
        ATOHigh = status.istat.GetInputByName("ATO_Hi")?.ToBool() ?? false;
    }
    
    public DateTimeOffset Timestamp { get; set; }
    public string ApexName { get; set; }
    public double Temp { get; set; }
    public double Ph { get; set; }
    public double Alk { get; set; }
    public double Calc { get; set; }
    public double Mg { get; set; }
    public Output? ATO { get; set; }
    public bool ATOLow { get; set; }
    public bool ATOHigh { get; set; }
}