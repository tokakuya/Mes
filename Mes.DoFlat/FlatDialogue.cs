using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mes.DoFlat;

public static class FlatDialogueExtension
{
    /// メソッドチェーンするために自己を返した方がいい
    public static MesBuilder DoFlat_Dialogue(this MesBuilder builder)
    {
        // TODO: 効率悪いのでリファクタ必要かも
        // 名前「.*」にマッチしたら、Mes記法に変換する=>OK
        // 名前\tセリフ　にマッチしたら、Mes記法に変換する
        var conf = builder.MesConfig;
        var lineList = builder.RawText.Split("\n");
        var rx = new Regex($"^.*{conf.flat_dialogue_config.start_str}", RegexOptions.Compiled); //名前「　のマッチング
        var rxtab = new Regex($"^.*\t", RegexOptions.Compiled); //名前\t　のマッチング
        var rxsp4 = new Regex($"^.*    ", RegexOptions.Compiled); //スペース4つのマッチング
        var result = lineList.Select(line =>
        {
            var nameMatch = rx.Match(line);
            var nameMatchTab = rxtab.Match(line);
            var nameMatchSp4 = rxsp4.Match(line);

            //TODO: ここのコードもうすこし整理する
            if (nameMatch.Success == true)
            {
                var name = nameMatch.Value.Replace(conf.flat_dialogue_config.start_str, "").Trim();
                var dialogue = rx.Replace(line, "").Replace(conf.flat_dialogue_config.end_str, "");
                return $"{conf.mes_piece_config.decorator.charactor[0]}{name}\n{dialogue}";
            }
            else if (nameMatchTab.Success == true)
            {
                var name = nameMatchTab.Value.Replace("\t", "").Trim();
                var dialogue = rxtab.Replace(line, "");
                return $"{conf.mes_piece_config.decorator.charactor[0]}{name}\n{dialogue}";
            }
            else if (nameMatchSp4.Success == true)
            {
                var name = nameMatchSp4.Value.Replace("    ", "").Trim();
                var dialogue = rxsp4.Replace(line, "");
                return $"{conf.mes_piece_config.decorator.charactor[0]}{name}\n{dialogue}";
            }
            else
            {
                return line;
            }
        }).ToArray();
        builder.RawText = String.Join("\n", result);

        return builder;
    }

}