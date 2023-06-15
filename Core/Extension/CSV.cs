using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace Mes.Core;

public interface IExportCSV
{
    public MesBody body { get; set; }
    public MesHeader header { get; set; }
    public IEnumerable<MesPiece> GetMesPieces();
    public string ExportCSVForASMRDaihon(MesPieceProperty[] propertys, string delimiter = "\t", bool inHeader = false) => CSV.ExportCSVForASMRDaihon(this, propertys, delimiter);
    public string ExportCSVForASMRDaihon(string delimiter = "\t") => CSV.ExportCSVForASMRDaihon(this, CSV.defaultExportProperty, delimiter);
    public string ExportCSV(string delimiter = "\t", bool inHeader = false) => CSV.ExportCSV(this, CSV.defaultExportProperty, delimiter, inHeader);
    public string ExportCSV(MesPieceProperty[] propertys, string delimiter = "\t", bool inHeader = false) => CSV.ExportCSV(this, propertys, delimiter, inHeader);



}

public static class CSV
{
    internal static MesPieceProperty[] defaultExportProperty = new MesPieceProperty[] {
        MesPieceProperty.Comments,
        MesPieceProperty.SoundNote,
        MesPieceProperty.SoundPosition,
        MesPieceProperty.Dialogue
    };

    public static string ExportCSVForASMRDaihon(this IExportCSV mes, string delimiter = "\t", bool inHeader = false)
          => CSV.ExportCSVForASMRDaihon(mes, CSV.defaultExportProperty, delimiter, inHeader);
    /*古いコード
    {

    string[] header = new string[] { "ト書き", "距離", "位置", "セリフ" };

    List<string> comments = new List<string>();
    List<string> sound_note = new List<string>();
    List<string> sound_positon = new List<string>();
    List<string> dialogue = new List<string>();

    foreach (var section in mes.body.sections)
    {
        foreach (var piece in section.pieces)
        {
            comments.Add(piece.comments);
            sound_note.Add(piece.sound_note);
            sound_positon.Add(piece.sound_position);
            dialogue.Add(piece.dialogue);
        }
    }

    var result = String.Join(delimiter, comments) + Environment.NewLine
                + String.Join(delimiter, sound_note) + Environment.NewLine
                + String.Join(delimiter, sound_positon) + Environment.NewLine
                + String.Join(delimiter, dialogue) + Environment.NewLine;
    return result;

    }
    */

    public static string ExportCSVForASMRDaihon(this IExportCSV mes, IEnumerable<MesPieceProperty> propertys, string delimiter = "\t", bool inHeader = false)
    {
        StringBuilder rows = new StringBuilder();
        List<string> header = new List<string>();

        //rowsに追加するための内部関数
        var AddRows = (MesPieceProperty p) =>
        {
            header.Add(p.GetNameJapanese());
            foreach (var section in mes.body.sections)
            {
                rows.Append(String.Join(delimiter, section.GetPiecesProperty(p)));
            }
            rows.Append(Environment.NewLine);
        };

        foreach (var property in propertys)
        {
            /*  ASMR台本形式は行列が転置なのでヘッダーは各行の先頭にヘッダーがくる
                Header1, row1, row2, row3, ...
                Header2, row1, row2, row3, ...
                ...
            */
            if (inHeader) rows.Append(property.GetNameJapanese() + delimiter);
            AddRows(property);
        }

        return rows.ToString();
    }

    public static string ExportCSV(this IExportCSV mes, MesPieceProperty[] propertys, string delimiter = "\t", bool inHeader = false)
    {
        //string rows = "";
        //rowsに追加するための内部関数
        var rows = mes.GetMesPieces().Select(piece =>
        {
            //generate line(row)
            return String.Join(delimiter, piece.GetPropertys(propertys)) + Environment.NewLine;
        });

        if (inHeader) return String.Join(delimiter, propertys.Select(p => p.GetNameJapanese())) + "\n" + String.Join("\n",rows);
        else return String.Join("\n", rows);
    }

    public static string ExportCSV(this IExportCSV mes, string delimiter = "\t")
    {
        MesPieceProperty[] allPropertys = new MesPieceProperty[]
        {
            MesPieceProperty.Comments,
            MesPieceProperty.Charactor,
            MesPieceProperty.Dialogue,
            MesPieceProperty.SoundNote,
            MesPieceProperty.SoundPosition,
            MesPieceProperty.Timing,
            MesPieceProperty.ExtField,
        };
        return ExportCSV(mes, allPropertys, delimiter);
    }
    public static string ExportCSVForASMR(this IExportCSV mes, string delimiter = "\t")
    {

        string[] header = new string[] { "ト書き","距離","位置", "セリフ"};
        var csv = mes.body.sections.Select(v =>
        {
            //TODO: リファクタできそう
            var _csv = v.pieces.Select(p =>
            {
                return $"{p.comments}{delimiter}{p.sound_note}{delimiter}{p.sound_position}{delimiter}{p.dialogue}{Environment.NewLine}";
            });
            return String.Join(Environment.NewLine, _csv);
        });


        var result = String.Join(delimiter, header) + Environment.NewLine + String.Join(Environment.NewLine, csv);
        return result;
    }
    

}
