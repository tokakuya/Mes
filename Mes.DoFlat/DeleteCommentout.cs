using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mes.DoFlat;

public static class DeleteCommentoutExtension
{
    /// <summary>Delete Commentout from mes text</summary>
    public static MesBuilder DoFlat_DeleteCommentout(this MesBuilder builder)
    {
        var rx = new Regex("//.*", RegexOptions.Compiled);
        builder.RawText = rx.Replace(builder.RawText, "");
        return builder;
    }
}