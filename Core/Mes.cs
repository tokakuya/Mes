using Mes.core.Extension;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Mes.Core;

public class Mes
    :ICountDialogueWords
    ,IExportDaihonText
    ,IJsonSerialize
    ,IExportMesText
    ,IExportCSV
    ,IChatStyleExtention
    ,IGameBookExtention

{
    public MesHeader header{get; set;}
    public MesBody body{get; set;}

    public Mes()
    {

    }

    public Mes(string MesText, ref MesConfig conf)
    {
        //ヘッダーデリミタでの分離
        var text = new Regex(conf.header_delimiter).Split(MesText);
   
        if (text.Length == 1)
        {
            //ヘッダーなし
            this.header = new MesHeader();
            this.body = new MesBody(text[0]);
        }else if(text.Length == 2)
        {
            //ヘッダーあり
            this.header = new MesHeader(text[0], ref conf);
            this.body = new MesBody(text[1], conf);
        }
    }

    public IEnumerable<MesPiece> GetMesPieces()
    {
        foreach (var section in this.body.sections)
        {
            foreach(var piece in section.pieces)
            {
                yield return piece;
            }
        }
    }

    //Mesからテキストに再変換するためのメソッド
    public string ToMesText(){
        
        return "";
    }

    //public Dictionary<string, int> CountDialogueWithCharactor(char[] ignore_char) => DialogueTextCount.GetDialogueTextCount(this, ignore_char);

    public string ToJson(bool WriteIndented = true) => this.ToJson<Mes>(WriteIndented);
    public byte[] SerializeToUtf8Bytes()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this);
    }

    public Dictionary<string, int> CountDialogueWithCharactor(char[] ignore_char) => DialogueTextCount.GetDialogueTextCount(this, ignore_char);
    public Dictionary<string, int> CountDialogueWithCharactor() => DialogueTextCount.GetDialogueTextCount(this, new char[]{});

}

/// ジェネリクスでJSONは各種型をJSON出力できるようにする