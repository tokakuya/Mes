using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mes.DoFlat;

public static class HashiraToCommentExtension
{
   /// 柱書きをコメントに変換するための処理
    public static MesBuilder DoFlat_HashiraToComment(this MesBuilder builder)
    {
        var rx = new Regex("^(◯|○)", RegexOptions.Compiled);
        builder.RawText = rx.Replace(builder.RawText, "#");
        return builder;
    }
}