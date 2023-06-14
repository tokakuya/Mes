using Mes.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mes.DoFlat;

internal static class CommonScenario
{
    internal static Dictionary<string, Regex> regList = new Dictionary<string, Regex>
    {
        { "()", new Regex("^(\\(|）).*(\\)|）)") },
        { "【】", new Regex("^【.*】") },
        { "○", new Regex("^○.*") }, //これ使わないかも
    };

    internal static Dictionary<string, Regex> replaceRegList = new Dictionary<string, Regex>
    {
        { "()", new Regex("(\\(|）|\\)|）)") },
        { "【】", new Regex("(【|】)") },
        { "○", new Regex("○") }, //これ使わないかも
    };

    internal static MesPiece pieceInstance = new MesPiece();

}

public static class FlatCommonScenarioStyleExtention
{
    public static MesBuilder DoFlat_CommonScenarioStyle(this MesBuilder builder)
    {

        var lineList = builder.RawText.Split("\n").Select(line =>
        {
            
            line = DoFlat_ParenthesisToComment(line)//（）で囲まれているものはコメントへ
                    .DoFlat_BlackLenticularBracket() //【】で囲まれているのもコメントへ
                    ;

            //○の柱は○をつけたままコメントへ？
            //キャラ名「セリフ」は変換

            //そのまま記述されているのもコメントへ
            //var hasNotDecoratorText = CommonScenario.pieceInstance.getDialogue(line, builder.MesConfig.mes_piece_config.decorator.getDecorators(), builder.MesConfig);
            //if (hasNotDecoratorText is not "") line = "#" + line;

            return line;
        });

        builder.RawText = String.Join("\n", lineList);
        return builder;
    }

    public static string DoFlat_ParenthesisToComment(this string line)
    {
        return CommonScenario.regList["()"].Match(line).Success 
               ? "#" + CommonScenario.replaceRegList["()"].Replace(line, "")
               : line;
    }

    public static string DoFlat_BlackLenticularBracket(this string line)
    {
        return CommonScenario.regList["【】"].Match(line).Success
               ? "#" + CommonScenario.replaceRegList["【】"].Replace(line, "")
               : line;
    }
}