namespace Mes;

using Mes.core;
using System.Text.Json.Serialization;

public record MesConfig
    :IJsonSerialize
{
    [JsonInclude]
    public string name = "default_config";
    [JsonInclude]
    public string header_delimiter = "----\n"; //NOTE:^が必要かも
    [JsonInclude]
    public string section_delimiter = "==";
    [JsonInclude]
    public FlatDialogueConfig flat_dialogue_config = new FlatDialogueConfig();
    [JsonInclude]
    public MesPieceConfig mes_piece_config = new MesPieceConfig();
    [JsonInclude]
    public DialogueCountConfig dialogue_count_config = new DialogueCountConfig();

    public record DialogueCountConfig
    {
        [JsonInclude]
        public char[] ignore_char = new[] {' '};
    }

    public record FlatDialogueConfig
    {
        [JsonInclude]
        public string start_str = "「";
        [JsonInclude]
        public string end_str = "」";
    }

    public class MesPieceConfig
    {
        [JsonInclude]
        public string block_delimitor = "\n\n";
        [JsonInclude]
        public string attribute_delimiter = "\n";
        [JsonInclude]
        public string default_charactor_name = "";

        [JsonInclude]
        public MesPieceDecorator decorator;

        public class MesPieceDecorator
        {
            [JsonInclude]
            public char[] dialogue = {};
            [JsonInclude]
            public char[] comments = {'#','＃', '○' };
            [JsonInclude]
            public char[] sound_note = {'$','＄'};
            [JsonInclude]
            public char[] sound_position = {'!','！'};
            [JsonInclude]
            public char[] charactor = {'@','＠'};
            [JsonInclude]
            public char[] timing = {'&','＆'};
            [JsonInclude]
            public char[] ext_field = {'?','？'};

            public char[] getDecorators()
            {
                return this.comments.Concat(this.sound_note).Concat(this.sound_position).Concat(this.charactor).Concat(this.timing).Concat(this.ext_field).ToArray();
            }
        }
        public MesPieceConfig()
        {
            this.decorator = new MesPieceDecorator();
        }

    }

    public MesConfig()
    {

    }
    public MesConfig(string jsonConf)
    {
    }

    public string ToJson() => JsonSerialize.ToJson<MesConfig>(this);
}

