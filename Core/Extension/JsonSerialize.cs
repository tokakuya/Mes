using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Mes.Core;


public interface IJsonSerialize
{
    public string ToJson() => JsonSerialize.ToJson<IJsonSerialize>(this, WriteIndented: false);
}


public static class JsonSerialize
{
    public static string ToJson<T>(this T thisObj, bool WriteIndented = false)
    {
        var options = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = WriteIndented
        };
        return JsonSerializer.Serialize<T>(thisObj, options);
    }
    
}