namespace Mes;

using Mes.Core;
using Mes.DoFlat;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

public interface IFlatter
{
    public MesBuilder DoFlat(MesBuilder builder);
}


/// <summary>
/// Builderをユーザが拡張したい場合は拡張メソッドで加工してください
/// </summary>
public class MesBuilder
{
    internal MesConfig MesConfig;
    public string RawText = "";

    /* セッター */
    public MesBuilder SetRawText(string text)
    {
        this.RawText = text.Replace("\r\n","\n").Replace("\r", "\n");
        return this;
    }

    public MesBuilder SetRawText(Func<string,string> flatter)
    {
        this.RawText = flatter(this.RawText);
        return this;
    }

    public MesBuilder SetMesConfig(MesConfig config)
    {
        this.MesConfig = config;
        return this;
    }

    /* コンストラクター */
    public MesBuilder(string rawText): this(rawText, new MesConfig()){}
    public MesBuilder(string rawText, MesConfig config)
    {
        SetRawText(rawText);
        this.MesConfig = config;
    }
    public Mes Build(bool skipDoFlat = false)
    {
        if (!skipDoFlat) DoFlat();
        return new Mes(this.RawText, ref MesConfig);
    }

    /// <summary>Alias of Build() </summary>
    public Mes Run(bool skipDoFlat = false) => this.Build(); 
    /// <summary>Alias of Build() </summary>
    public Mes Parse(bool skipDoFlat = false) => this.Build();

    public void DoFlat(){
        // using Mes.DoFlat Extension Methods.
        this.DoFlat_Blocks();
        this.DoFlat_DeleteCommentout();
        this.DoFlat_HashiraToComment();
        this.DoFlat_Dialogue();
    }

    public void printMesConfig(){
        System.Console.Out.WriteLine(System.Text.Json.JsonSerializer.Serialize(this.MesConfig));
    }
}

