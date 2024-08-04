using System.Text;
using Shared.Enums;

namespace Shared.Api.Commands;

public class DeviceFeaturesCommand : AbstractCommand
{
    private byte? _batteryCapacity;
    private HashSet<FeatureValue> _featureList;
    private int? _special;

    public DeviceFeaturesCommand()
    {
    }

    public DeviceFeaturesCommand(byte[]? data) : base(data)
    {
    }

    public override void ProcessData()
    {
        _batteryCapacity = Data[0];
        _special = BitConverter.ToInt32(Data, 1);
        Console.WriteLine(BitConverter.ToString(Data));
        uint features = BitConverter.ToUInt32(Data, 1);
        _featureList = ProcessFeatures(features);
    }

    protected string ReadFeatures(byte[] data = null)
    {
        var features = new StringBuilder();
        var temp = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            temp.Append(PadHexString(data[i].ToString("X2")));
            if (i % 4 != 3) continue;
            features.Insert(0, temp.ToString());
            temp.Clear();
        }

        int j = 0;
        while (j < features.Length && features[j] == '0')
        {
            j++;
        }

        if (j == features.Length)
        {
            return "0";
        }

        return features.ToString(j, features.Length - j);
    }

    protected HashSet<FeatureValue> ProcessFeatures(uint features)
    {
        HashSet<FeatureValue> featureValues = new HashSet<FeatureValue>();
        string featuresBinary = Convert.ToString(features, 2);
        foreach (FeatureValue feature in Enum.GetValues(typeof(FeatureValue)))
        {
            if (featuresBinary.Length <= (int) feature) continue;
            if (featuresBinary[featuresBinary.Length - (int) feature - 1] == '1')
            {
                featureValues.Add(feature);
            }
        }

        return featureValues;
    }

    public int GetBatteryCapacity()
    {
        return _batteryCapacity.HasValue ? _batteryCapacity.Value : -1;
    }

    public int GetSpecial()
    {
        return _special.HasValue ? _special.Value : 0;
    }

    public HashSet<FeatureValue> GetFeaturesList()
    {
        return _featureList ?? new HashSet<FeatureValue>();
    }


    private string PadHexString(string hexString)
    {
        return hexString.PadLeft(2, '0');
    }

    public override byte[] Build()
    {
        return new byte[0];
    }

    public override CommandType GetCommandType()
    {
        return CommandType.COMM_SEARCHE_DEVICE_FEATURE;
    }
}