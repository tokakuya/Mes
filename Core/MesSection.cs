namespace Mes.Core;
using System.Linq;

public record MesSection
    :IJsonSerialize
{
    public string name{get; set;} = "";
    public MesPiece[] pieces{get; set;}

    public MesSection(string text, MesConfig conf): this(text, "", conf){}

    public MesSection(string text, string name, MesConfig conf)
    {
        text = text.Replace(Environment.NewLine, "\n").Trim(); //TODO:いらんかも
        this.name = name;
        this.pieces = GetMesPieces(text, conf);
    }

    public MesSection(){
        //基本的に使わない //TODO: 最終的にPrivate
        this.name = "";
        this.pieces = new[]{ new MesPiece() };
    }

    public MesPiece[] GetMesPieces(string text, MesConfig conf)
    {
        return text.Split(conf.mes_piece_config.block_delimitor)
            .AsParallel() //NOTE: ここのパラレルは効果的っぽい
            .Where(v => v.Trim() != "")
            .Select(v => new MesPiece(v, conf))
            .ToArray();    //ここ並列処理で早くしたい
    }

    public string[] GetPiecesProperty(MesPieceProperty property)
    {
        if (property == MesPieceProperty.Comments) return this.pieces.Select(p => p.comments).ToArray();
        if (property == MesPieceProperty.Charactor) return this.pieces.Select(p => p.charactor).ToArray();
        if (property == MesPieceProperty.Dialogue) return this.pieces.Select(p => p.dialogue).ToArray();
        if (property == MesPieceProperty.SoundNote) return this.pieces.Select(p => p.sound_note).ToArray();
        if (property == MesPieceProperty.SoundPosition) return this.pieces.Select(p => p.sound_position).ToArray();
        if (property == MesPieceProperty.Timing) return this.pieces.Select(p => p.timing).ToArray();
        if (property == MesPieceProperty.ExtField) return this.pieces.Select(p => p.ext_field).ToArray();

        return new string[]{"unknown property"};
    }
    
}

