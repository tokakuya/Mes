using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mes.Core;

public interface IExportMesText {

    public MesHeader header { get; set; }
    public MesBody body { get; set; }
    public string ExportMesText(MesConfig config) => MesTextExtention.ExportMesText(this, config);

}

public static class MesTextExtention
{
    public static string ExportMesText(this IExportMesText mes, MesConfig config)
    {
        StringBuilder result = new StringBuilder();

        //Headerの生成
        if (mes.header.headerRaw != "")
        {
            result.Append(mes.header.headerRaw);
            result.Append("\n" + config.header_delimiter + "\n\n");
        }

        //Bodyの生成
        foreach (var section in mes.body.sections)
        {
            if (section.name is not "") result.Append(config.section_delimiter + section.name + "\n\n");
            else result.Append(config.section_delimiter + "\n\n");

            foreach (var piece in section.pieces)
            {
                //MEMO: 下手するとデコレーターの参照時にout of indexになる可能性がある（稀）
                if (piece.comments is not "") result.Append(config.mes_piece_config.decorator.comments[0] + piece.comments + "\n");
                if (piece.charactor is not "") result.Append(config.mes_piece_config.decorator.charactor[0] + piece.charactor + "\n");
                if (piece.sound_note is not "") result.Append(config.mes_piece_config.decorator.sound_note[0] + piece.sound_note + "\n");
                if (piece.sound_position is not "") result.Append(config.mes_piece_config.decorator.sound_position[0] + piece.sound_position + "\n");
                if (piece.timing is not "") result.Append(config.mes_piece_config.decorator.timing[0] + piece.timing + "\n");
                if (piece.ext_field is not "") result.Append(config.mes_piece_config.decorator.ext_field[0] + piece.ext_field + "\n");
                if (piece.dialogue is not "") result.Append(piece.dialogue + "\n");
                result.Append("\n");
            }

        }
        return result.ToString();
    }


}
