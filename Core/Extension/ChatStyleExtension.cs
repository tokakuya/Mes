using Mes.Core;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;


namespace Mes.Core.Extension;

public interface IChatStyleExtention
{
    public IEnumerable<MesPiece> GetMesPieces();
    public IEnumerable<string> ToChatStyleHTML() => ChatStyleExtension.ToChatStyleHTML(this, ChatStyleExtension.DefaultTemplate);

    public IEnumerable<string> ToChatStyleHTML(Func<MesPiece, string> TextTemplate) => ChatStyleExtension.ToChatStyleHTML(this, TextTemplate);
}

public static class ChatStyleExtension
{
    public static IEnumerable<string> ToChatStyleHTML(this IChatStyleExtention mes)
    {
        return mes.ToChatStyleHTML(DefaultTemplate);
    }

    public static IEnumerable<string> ToChatStyleHTML(this IChatStyleExtention mes, Func<MesPiece, string> TextTemplate)
    {
        foreach (var piece in mes.GetMesPieces())
        {
            yield return TextTemplate(piece);
        }
    }

    internal static string DefaultTemplate(MesPiece piece)
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