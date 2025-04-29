
//

using shared.Models.Vibrations.Patterns;

namespace shared.Models.Vibrations;
public static class VibrationHelpers
{
    public const int HeaderLengthSize = 4;
    public const int HeaderModeSize = 1;
    public const int HeaderResolutionSize = 8;
    public const int HeaderSize = HeaderLengthSize + HeaderModeSize + HeaderResolutionSize;
    public static async Task<IVibrationPattern> ParseAsVibrationData(BinaryAdapter reader)
    {
        var size = reader.ReadInt32();
        var mode = FromFlagByte<VibrationMode>(reader.ReadByte());
        var resolution = reader.ReadDouble();
        var patternReader = reader.CreateSubReader(size - HeaderSize);

        return mode switch
        {
            VibrationMode.Dynamic => await VibrationPatternDynamic.ParseAsync(patternReader, resolution),
            VibrationMode.Expression => await VibrationPatternExpression.ParseAsync(patternReader, resolution),
            VibrationMode.Constant => await VibrationPatternConstant.ParseAsync(patternReader, resolution),
            VibrationMode.Mixed => await VibrationPatternMixed.ParseAsync(patternReader, resolution),
            _ => throw new NotImplementedException($"Vibration mode {mode} is not implemented"),
        };
    }

    public static byte[] ToBytes(this IVibrationPattern pattern)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        var data = pattern.GetDataBytes();
        writer.Write(data.Length + HeaderSize);
        writer.Write(pattern.Mode.ToFlagByte());
        writer.Write(pattern.Resolution);
        writer.Write(data);
        writer.Flush();
        var result = stream.ToArray();
        stream.Dispose();
        writer.Dispose();
        return result;
    }

    public static byte ToFlagByte<T>(this T flag) where T : Enum
    {
        var intFlag = Convert.ToInt32(flag);
        if (Enum.IsDefined(typeof(T), intFlag))
        {
            return (byte)intFlag;
        }
        else
        {
            throw new ArgumentException($"Flag {flag} is not defined in enum {typeof(T).Name}");
        }
    }

    public static T FromFlagByte<T>(byte flag) where T : IConvertible
    {
        int intFlag = Convert.ToInt32(flag);
        if (Enum.IsDefined(typeof(T), intFlag))
        {
            return (T)Enum.ToObject(typeof(T), intFlag);
        }
        else
        {
            throw new ArgumentException($"Flag {flag} is not defined in enum {typeof(T).Name}");
        }
    }
}

