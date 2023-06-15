using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Mes.Core;


public class MesHeader
{
    string defaultCharactorName = "";
    public string headerRaw { get; } = "";

    public MesHeader()
    {
        
    }
    public MesHeader(string headerText)
    {
        this.headerRaw = headerText;
    }

    public MesHeader(string headerText, ref MesConfig conf)
    {
        this.headerRaw = headerText;
        //SetConfigVariable(headerText, ref conf);
        SetHeaderVariableToCofig(headerText, ref conf);
    }

    
    /// <summary>
    /// confがref MesConfigでない場合はヘッダーのコンフィグ設定変数をconfに上書きしません。
    /// ref MesConfigと間違えて使わないように、あえて引数の順序を変更しています。
    /// </summary>
    /// <param name="headerText"></param>
    /// <param name="conf"></param>
    public MesHeader(MesConfig conf,string headerText)
    {
        this.headerRaw = headerText;
        //SetConfigVariable(headerText, ref conf);

    }


    public string SetConfigVariable(string headerText, ref MesConfig conf)
    {
        //リファクタしたほうがいい
        var SetVariable = (MesPieceProperty property, ref MesConfig _conf) =>
        {
            var varName = (property) switch
            {
                MesPieceProperty.Charactor => "\\$deco_charactor\\s*=.*",
                MesPieceProperty.Comments => "\\$deco_comments\\s*=.*",
                MesPieceProperty.SoundPosition => "\\$deco_sound_position\\s*=.*",
                MesPieceProperty.SoundNote => "\\$deco_sound_note\\s*=.*",
                MesPieceProperty.Timing => "\\$deco_timing\\s*=.*",
                MesPieceProperty.ExtField => "\\$deco_ext_field\\s*=.*",
                _ => ""
            };
            var rx = new Regex(varName, RegexOptions.Compiled);
            var varNameMatch = rx.Match(headerText);
            if (varNameMatch.Success)
            {
                //var delstr = Regex.Replace(varNameMatch.Value, "\\$.*=", "");
                var value = Regex.Replace(varNameMatch.Value, "\\$.*=", "").Replace("(\r|\n)*","").Trim();
                System.Console.WriteLine("value:"+value);
                if(value.Length == 1)//一文字だけの設定しか許可しない
                {
                    switch (property)
                    {
                        case MesPieceProperty.Charactor:
                            _conf.mes_piece_config.decorator.charactor = value.ToCharArray();
                            break;
                        case MesPieceProperty.Comments:
                            _conf.mes_piece_config.decorator.comments = value.ToCharArray();
                            break;
                        case MesPieceProperty.SoundNote:
                            _conf.mes_piece_config.decorator.sound_note = value.ToCharArray();
                            break;
                        case MesPieceProperty.SoundPosition:
                            _conf.mes_piece_config.decorator.sound_position = value.ToCharArray();
                            break;
                        case MesPieceProperty.Timing:
                            _conf.mes_piece_config.decorator.timing = value.ToCharArray();
                            break;
                        case MesPieceProperty.ExtField:
                            _conf.mes_piece_config.decorator.ext_field = value.ToCharArray();
                            break;
                        default:
                            break;
                    }
                }
            }
        };
        
        SetVariable(MesPieceProperty.Comments, ref conf);
        SetVariable(MesPieceProperty.Charactor, ref conf);
        SetVariable(MesPieceProperty.SoundPosition, ref conf);
        SetVariable(MesPieceProperty.SoundNote, ref conf);
        SetVariable(MesPieceProperty.ExtField, ref conf);
        SetVariable(MesPieceProperty.Timing, ref conf);

        return "";
    }

    public MesHeader SetHeaderVariableToCofig(string headerText, ref MesConfig _conf)
    {

        //NOTE: 本当はヘッダーテキストからの探査を1回でやるほうが処理が早い
        SetValiable(headerText, ref _conf.dialogue_count_config.ignore_char, new Regex("\\$ignore_char\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.charactor, new Regex("\\$deco_charactor\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.comments, new Regex("\\$deco_comments\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.sound_position, new Regex("\\$deco_sound_position\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.sound_note, new Regex("\\$deco_sound_note\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.timing, new Regex("\\$deco_timing\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.decorator.ext_field, new Regex("\\$deco_ext_field\\s*=.*", RegexOptions.Compiled));
        SetValiable(headerText, ref _conf.mes_piece_config.default_charactor_name, new Regex("\\$default_charactor_name\\s*=.*", RegexOptions.Compiled));


        return this;
    }
    public void SetValiable(string headerText, ref string _confStringRef, Regex varNameReg, string varPattern = "\\$.*=")
    {
        var (isHit, value) = GetHeaderVariable(headerText, varNameReg, varPattern);
        if (isHit) _confStringRef = value;
    }

    public void SetValiable(string headerText, ref char[] _confCharArrayRef, Regex varNameReg, string varPattern = "\\$.*=")
    {
        var (isHit, value) = GetHeaderVariable(headerText, varNameReg, varPattern);
        if (isHit) _confCharArrayRef = value.ToCharArray().Distinct().ToArray(); //重複を削除してセット
    }

    public (bool, string) GetHeaderVariable(string headerText, Regex varNameReg, string varPattern = "\\$.*=")
    {
        var varNameMatch = varNameReg.Match(headerText);
        if (varNameMatch.Success)
        {
            //TODO:リファクタ
            var value = Regex.Replace(varNameMatch.Value, varPattern, "").Replace("(\r|\n)*", "").Trim();
            return (true, value);
        }
        else return (false, "");
    }


    public string GetDefaultCharactorName(string headerText)
    {
        var rx = new Regex(@"^\$default_name\s*=\s*", RegexOptions.Compiled);
        var nameMatch = rx.Match(headerText);
        if(nameMatch.Success)
        {
            return "";
            //デフォルト名を抽出して返す
        }

        return "";
    }
}
