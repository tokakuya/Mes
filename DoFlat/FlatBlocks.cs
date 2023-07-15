using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mes.DoFlat;

public static class FlatBlocksExtension
{
    public static MesBuilder DoFlat_Blocks(this MesBuilder builder)
    {
        DoFlat_Blocks(ref builder.RawText);
        return builder;
    }

    private static string DoFlat_Block(string mesText)
    {
        var rx_args = new Regex(@"\((?<args>.*?)\)", RegexOptions.Multiline | RegexOptions.Compiled);
        var rx_lines = new Regex("{{(?<line>(.|\n)*)}}", RegexOptions.Multiline | RegexOptions.Compiled);

        var args = rx_args.Match(mesText).Groups["args"].Value; //.Value.Replace("(", "").Replace(")", "");
        var lines = rx_lines.Match(mesText).Groups["line"].Value; //.Value.Replace("{{", "").Replace("}}", "");

        var li = lines.Split(Environment.NewLine + Environment.NewLine).Select((line) =>
        {
            return $"{args.Replace(",", "\n")}\n{line.Trim()}";
        });
        return string.Join("\n\n", li);
    }
    internal static string DoFlat_Blocks(ref string mesText)
    {
        //ブロックを抽出して
        var rx_block = new Regex("^(.*){{(.|\n)*?}}", RegexOptions.Multiline | RegexOptions.Compiled);

        foreach (Match block in rx_block.Matches(mesText))
        {

            if (block.Success)
            {
                mesText = mesText.Replace(block.Value, DoFlat_Block(block.Value));
            }
            //ここで展開して、展開後のテキストに置換する

        }
        return mesText;
    }
}