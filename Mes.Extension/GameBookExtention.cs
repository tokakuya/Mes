using Mes.core;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace Mes.Extension;

public static class SelecterRegex
{
    static internal Regex regChoose = new Regex("choose:.*?,",RegexOptions.Compiled);
    static internal Regex regJump = new Regex("jump:.*?,", RegexOptions.Compiled);
}

public class BranchNode
{
    [JsonInclude]
    public string section = "";
    [JsonInclude]
    public Select[] selects = new Select[] { };

    public class Select
    {
        [JsonInclude]
        public string value = "";
        [JsonInclude]
        public string jump = "";

        public Select(string extFieldText)
        {
            extFieldText = extFieldText.Trim();
            value = SelecterRegex.regChoose.Match(extFieldText).Value.Replace("choose:","").Replace(",","").Trim();
            jump = SelecterRegex.regJump.Match(extFieldText).Value.Replace("jump:", "").Replace(",", "").Trim();
        }
        public Select() { }
    }
    public BranchNode(string section, Select[] selects)
    {
        this.section = section;
        this.selects = selects;
    }
}

public static class GameBookExtention
{

    public static BranchNode[] GetGameBook()
    {
        return new BranchNode[] { };
    }

    public static IEnumerable<BranchNode> GetBranchNodes(this Mes.core.Mes mes)
    {
        var regJump = new Regex("jump:.*?,", RegexOptions.Compiled);
        var regChoose = new Regex("choose:.*?,", RegexOptions.Compiled);
        foreach (var section in mes.body.sections)
        {
            var SelectList = section.pieces
                    .Select(v => v.ext_field.Split("\n"))
                    .SelectMany(x => x)
                    .Where(ex => regJump.IsMatch(ex) || regChoose.IsMatch(ex))
                    .Select(ex => new BranchNode.Select(ex))
                    .ToArray();
            yield return new BranchNode(section.name, SelectList);            
        }
    }
    public static string ExportMermaid(this Mes.core.Mes mes)
    {
        var branches = mes.GetBranchNodes();
        StringBuilder result = new StringBuilder();

        result.Append("graph TD;\n");
        foreach (var branch in branches)
        {
            foreach (var select in branch.selects)
            {
                if (select.value != "")
                {
                    result.Append($"{branch.section}-->|{select.value}|{select.jump};\n");
                }
                else
                {
                    result.Append($"{branch.section}-->{select.jump};\n");
                }
            }
        }
        return result.ToString();
    }
    public static string ExportPlantUML(this Mes.core.Mes mes)
    {
        var branches = mes.GetBranchNodes();
        StringBuilder result = new StringBuilder();

        result.Append("@startuml\n");
        foreach (var branch in branches)
        {
            foreach (var select in branch.selects)
            {
                if (select.value != "")
                {
                    result.Append($"[{branch.section}]-->[{select.jump}]:{select.value}\n");
                }
                else
                {
                    result.Append($"[{branch.section}]-->[{select.jump}]\n");
                }
            }
        }
        result.Append("@enduml");

        return result.ToString();
    }
}