using System.Runtime.CompilerServices;

namespace Mes.Core;


public interface ICountDialogueWords
{
    public Dictionary<string, int> CountDialogueWithCharactor(char[] ignore_char);
    public Dictionary<string, int> CountDialogueWithCharactor();
    
}

//拡張メソッドd
public static class DialogueTextCount
{
    /// <summary>
    /// return int TextCount.
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="ignore_char"></param>
    /// <returns></returns>
    public static int GetDialogueTextCount(this string dialogue, char[] ignore_char)
    {
        var ignoreLength = dialogue.Length - ignore_char.Select(c => dialogue.Count(v => v == c)).Sum();
        return ignoreLength;
    }
    public static int GetDialogueTextCount(this string dialogue, MesConfig conf)
    {
        return GetDialogueTextCount(dialogue, conf.dialogue_count_config.ignore_char);
    }

    /// <summary>
    /// return Dictionary <string charactorName, int textCount>
    /// </summary>
    /// <param name="mes"></param>
    /// <param name="conf"></param>
    /// <returns></returns>
    public static Dictionary<string, int> GetDialogueTextCount(this Mes mes, MesConfig conf) => GetDialogueTextCount(mes.body.sections.SelectMany(v => v.pieces).ToArray(),  conf.dialogue_count_config.ignore_char);
    public static Dictionary<string, int> GetDialogueTextCount(this Mes mes, char[] ignore_char) => GetDialogueTextCount(mes.body.sections.SelectMany(v => v.pieces).ToArray(),ignore_char);
    public static Dictionary<string, int> GetDialogueTextCount(this MesBody body, char[] ignore_char) => GetDialogueTextCount(body.sections.SelectMany(v => v.pieces).ToArray(),ignore_char);
    public static Dictionary<string, int> GetDialogueTextCount(this MesSection[] sections, char[] ignore_char) => GetDialogueTextCount(sections.SelectMany(v => v.pieces).ToArray(),ignore_char);
    private static Dictionary<string, int> GetDialogueTextCount(MesPiece[] pieces, char[] ignore_char)
    {
        var dict = new Dictionary<string, int>();
        foreach (var p in pieces)
        {
            //たぶん、こっちのほうが文字列のアロケーションしてないはずなので効率いい
            var ignoreLength = GetDialogueTextCount(p.dialogue, ignore_char);

            //キャラクター名が存在する場合
            if (dict.ContainsKey(p.charactor)) dict[p.charactor] += ignoreLength;
            else dict.Add(p.charactor, ignoreLength);
        }
        return dict;
    }
}
