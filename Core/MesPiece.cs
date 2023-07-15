namespace Mes.Core;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;


[Flags]
public enum MesPieceProperty
{
    Dialogue = 1,
    Charactor = 2,
    Comments = 4,
    SoundNote = 8,
    SoundPosition = 16,
    Timing = 32,
    ExtField = 64,
}

public static class MesPiecePropertyExtention 
{
    public static string GetNameJapanese(this MesPieceProperty property)
    {
        if (property == MesPieceProperty.Dialogue) return "セリフ";
        if (property == MesPieceProperty.Charactor) return "キャラクター";
        if (property == MesPieceProperty.Comments) return "ト書き";
        if (property == MesPieceProperty.SoundNote) return "音メモ";
        if (property == MesPieceProperty.SoundPosition) return "音位置";
        if (property == MesPieceProperty.Timing) return "タイミング";
        if (property == MesPieceProperty.ExtField) return "拡張フィールド";
        return "不明なpropertyです";
    }
    public static string[] GetPropertys(this MesPiece piece, MesPieceProperty property)
    {
        List<string> result = new List<string>();
        if ((property & MesPieceProperty.Comments) == MesPieceProperty.Comments) result.Add(piece.comments);
        if ((property & MesPieceProperty.Charactor) == MesPieceProperty.Charactor) result.Add(piece.charactor);
        if ((property & MesPieceProperty.Dialogue) == MesPieceProperty.Dialogue) result.Add(piece.dialogue);
        if ((property & MesPieceProperty.SoundNote) == MesPieceProperty.SoundNote) result.Add(piece.sound_note);
        if ((property & MesPieceProperty.SoundPosition) == MesPieceProperty.SoundPosition) result.Add(piece.sound_position);
        if ((property & MesPieceProperty.Timing) == MesPieceProperty.Timing) result.Add(piece.timing);
        if ((property & MesPieceProperty.ExtField) == MesPieceProperty.ExtField) result.Add(piece.ext_field);

        return result.ToArray();
    }

    public static string[] GetPropertys(this MesPiece piece, MesPieceProperty[] properties)
    {
        //指定した配列の順番でプロパティをしゅつりょくする
        List<string> result = new List<string>();
        foreach (var property in properties)
        {
            foreach(var item in piece.GetPropertys(property))
            {
                result.Add(item);
            }
        }
        return result.ToArray();
    }
}


public record MesPiece
    :IJsonSerialize
{
    [JsonInclude]
    public string dialogue;

    [JsonInclude]
    public string charactor;
    [JsonInclude]
    public string comments;
    public string sound_note;
    [JsonInclude]
    public string sound_position;
    [JsonInclude]
    public string timing;
    [JsonInclude]
    public string ext_field;

    public MesPiece(string pieceText): this(pieceText, new MesConfig()){}
    public MesPiece(string pieceText, MesConfig conf)
    {
        //pieceTextを渡したらパースしてくれる
        var decorator = conf.mes_piece_config.decorator;
        
        this.dialogue = "";
        this.comments = "";
        this.charactor = "";
        this.sound_note = "";
        this.sound_position = "";
        this.timing = "";
        this.ext_field = "";
        this.SetAttributes(pieceText, conf);
        this.dialogue = getDialogue(pieceText, decorator.getDecorators(), conf);
        if (conf.mes_piece_config.default_charactor_name != "" && this.dialogue != "" && this.charactor == "") this.charactor = conf.mes_piece_config.default_charactor_name;

    }

    public MesPiece(){
        //
        this.dialogue = "";
        this.comments = "";
        this.charactor = "";
        this.sound_note = "";
        this.sound_position = "";
        this.timing = "";
        this.ext_field = "";
    }

    public bool Equal(MesPiece other){
        return
            other.dialogue == this.dialogue &&
            other.comments == this.comments &&
            other.charactor == this.charactor &&
            other.ext_field == this.ext_field &&
            other.timing == this.timing &&
            other.sound_note == this.sound_note &&
            other.sound_position == this.sound_position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetAttributes(string pieceText, in MesConfig conf)
    {
        List<string> comments = new List<string> { };
        List<string> charactor = new List<string> { };
        List<string> ext_field = new List<string> { };
        List<string> sound_note = new List<string> { };
        List<string> sound_position = new List<string> { };
        List<string> timing = new List<string> { };

        foreach (var line in pieceText.Split(conf.mes_piece_config.attribute_delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            setAttr(line, conf.mes_piece_config.decorator.charactor, ref charactor);
            setAttr(line, conf.mes_piece_config.decorator.comments, ref comments);
            setAttr(line, conf.mes_piece_config.decorator.ext_field, ref ext_field);
            setAttr(line, conf.mes_piece_config.decorator.sound_note, ref sound_note);
            setAttr(line, conf.mes_piece_config.decorator.sound_position, ref sound_position);
            setAttr(line, conf.mes_piece_config.decorator.timing, ref timing);
        }
        //TODO：デコレーター内の配列要素の区切り文字の仕様をちゃんと決める
        this.charactor = String.Join("\n", charactor);
        this.comments = String.Join("\n", comments);
        this.ext_field = String.Join("\n", ext_field);
        this.sound_note = String.Join("\n", sound_note);
        this.sound_position = String.Join("\n", sound_position);
        this.timing = String.Join("\n", timing);
    }

    private void setAttr(string line, IReadOnlyCollection<char> prefixList, ref List<string> target)
    {
        if (prefixList.Any(prefix => prefix == line[0]))
        {
            //直接targetをぶっ壊す
            target.Add(line.Remove(0,1));
        }
    }
    private void setAttr2(string line, IReadOnlyCollection<char> prefixList, ref string target)
    {
        if (prefixList.Any(prefix => prefix == line[0]))
        {
            target = target == "" ? line.Remove(0,1) : "," + line.Remove(0,1);
        }
    }


    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string getAttribute(string pieceText, char[] prefixList, in MesConfig conf)
    {
        return String.Join("\n",
            pieceText.Split(conf.mes_piece_config.attribute_delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                //.AsParallel() //NOTE: ここのパラレルは著しく遅くなる
                .Where(v => prefixList.Any(prefix => prefix == v[0]))
                .Select(v => v.Remove(0,1) )
                .ToArray()
        );

        //return String.Join(",", result);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal string getDialogue(string pieceText, char[] ignorePrefixList, in MesConfig conf)
    {
        //prefixに該当する行を取り除く
        return String.Join("\n",
            pieceText.Split(conf.mes_piece_config.attribute_delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Where(v => !ignorePrefixList.Any(prefix => prefix == v[0]))
                .ToArray()
            );
        //return String.Join(",", result);
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
    public byte[] SerializeToUtf8Bytes()
    {
        return JsonSerializer.SerializeToUtf8Bytes(this);
    }

}


//MEMO たぶんシングルトンにしたほうがいいかもしれない