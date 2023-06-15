using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mes.Core.Extension;

public static class YukkuriMovieMakerExtension
{
    public static string ExportDaihon4YukkuriMovieMaker(this Mes.core.Mes mes)
    {
        var lines = mes.GetMesPieces().Where(f => f.dialogue is not "").Select(v => $"\"{v.charactor}\",\"{v.dialogue}\"");
        var csv = String.Join("\n",lines);
        return csv;
    }
}
