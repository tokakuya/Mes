using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mes.Core;


public interface IExportDaihonText
{
    public MesBody body { get; set; }
    public string ExportDaihonText() => DaihonTextExtention.ExportDaihonText(this);
}

public static class DaihonTextExtention
{
    public static string ExportDaihonText(IExportDaihonText mes)
    {
        StringBuilder result = new StringBuilder();

        //Bodyの生成
        foreach (var section in mes.body.sections)
        {
            if (section.name != "") result.Append("§" + section.name + "\n\n");

            foreach (var piece in section.pieces)
            {
                //MEMO: 下手するとデコレーターの参照時にout of indexになる可能性がある（稀）
                result.Append("\n");
                if (piece.comments is not "") result.Append(piece.comments + "\n");
                if (piece.sound_note is not "") result.Append($"（{piece.sound_note}） \n");
                if (piece.sound_position is not "") result.Append($"（音位置：{piece.sound_position}）\n");
                if (piece.timing is not "") result.Append($"（{piece.timing}）\n");
                if (piece.ext_field is not "") result.Append($"（{piece.ext_field}）\n");

                if (piece.dialogue is not "")
                {
                    //result.Append("\n");
                    result.Append($"{piece.charactor.Replace("\n", "・")}「{piece.dialogue}」");
                }
                result.Append("\n");
            }

        }
        return result.ToString();
    }

}
