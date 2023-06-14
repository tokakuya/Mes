using Mes.core;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;


namespace Mes.Extension;

public static class ChatStyleExtension
{
    public static IEnumerable<string> ToChatStyleHTML(this Mes.core.Mes mes)
    {
        return ToChatStyleHTML(mes, DefaultTemplate);
    }

    public static IEnumerable<string> ToChatStyleHTML(this Mes.core.Mes mes, Func<MesPiece, string>TextTemplate)
    {
        foreach (var piece in mes.GetMesPieces())
        {
            yield return TextTemplate(piece);
        }
    }

    private static string DefaultTemplate(MesPiece piece)
    {
        return $"<span style=\"color: #{CharactorNameToColorCode(piece.charactor)}\">{piece.charactor}:</span>{piece.dialogue}";
    }
    private static string CharactorNameToColorCode(string charactorName)
    {
        using (var sha1 = new SHA1Managed())
        {
            byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(charactorName));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++) // 3�o�C�g���̃n�b�V���l���g�p����6����16�i���\�L�ɕϊ�����
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}