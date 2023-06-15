using System.Linq;
using System.Text.RegularExpressions;

namespace Mes.Core;

public record MesBody
    :ICountDialogueWords
{
    public MesSection[] sections{get; set;}

    public MesBody(string MesBodyText): this(MesBodyText, new MesConfig()){}

    public MesBody(string MesBodyText, MesConfig conf)
    {
        //this.sections = getSectionText(MesBodyText, conf).Select(v => new MesSection(v, conf)).ToArray();
        this.sections = getSections(MesBodyText, conf);
    }
    public MesBody()
    {
        this.sections = new MesSection[]{};
    }

    private MesSection[] getSections(string MesBodyText, MesConfig conf)
    {
        //TODO: 変数名のりファクタしたほうがいい
        //const string tmpDecoratorString = "__";
        const string tmpDecorator = $"^__";
        var rx_tmp = new Regex(tmpDecorator, RegexOptions.Compiled);
        var rx = new Regex($"{tmpDecorator}.*", RegexOptions.Compiled);
        //TODO: リファクタしたほうがいい
        var result = Regex.Replace(MesBodyText, conf.section_delimiter, "==__").Split("==", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) //両方のフラグを利用するときor演算すればいいので
            .AsParallel()  //NOTE: ここのパラレルはあってもなくてもそんなに影響ないっぽい
            //.Where(v => v != "")　//Splitのフラグで解消
            .Select(v => {
                //Console.WriteLine(v);
                // Piece全体を探索すると良くないので、最初の行だけでマッチングを行うべき
                // 正規表現マッチも遅いので、デコレーターのような字句解析のほうがいい
                var pieecText = v.Trim();
                var nameMatch = rx.Match(pieecText);
                var name = (nameMatch.Success) switch {
                    //true => Regex.Replace(nameMatch.Value, tmpDecorator, "", 1).Trim(),
                    true => rx_tmp.Replace(nameMatch.Value, "", 1).Trim(),
                    false => ""
                };
                return new MesSection(rx.Replace(pieecText, "", 1).Trim(), name, conf);
            })
            .ToArray();
        return result;
    }
    private string[] getSectionText(string MesBodyText, MesConfig conf)
    {
        //TODO:セクションの名前を取得できるようにする。
        return MesBodyText.Split(conf.section_delimiter).Where(v => v.Trim() != "").ToArray();
    }


    public Dictionary<string, int> CountDialogueWithCharactor(char[] ignore_char) => DialogueTextCount.GetDialogueTextCount(this, ignore_char);
    public Dictionary<string, int> CountDialogueWithCharactor() => DialogueTextCount.GetDialogueTextCount(this, new char[]{});

    public string ToJson(bool WriteIndented = true) => this.ToJson<MesBody>(WriteIndented);

}

