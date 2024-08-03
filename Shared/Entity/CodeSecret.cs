namespace Shared.Entity;

public record CodeSecret
{
    public int Year { get; set; }
    public int Code { get; set; }
    public string Secret { get; set; }
}