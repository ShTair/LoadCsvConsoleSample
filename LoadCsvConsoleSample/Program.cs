using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoadCsvConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
            // 直接、1つ1つ読み書きしてみるパターン
            using (var writer = new StreamWriter(@"data1.csv", false, Encoding.Default))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteField("普通のフィールド");
                csv.WriteField(1);
                csv.WriteField("カンマを含む,フィールド");
                csv.WriteField("改行を\r\n含むフィールド");

                csv.NextRecord();

                csv.WriteField("次の行");
            }

            // ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            using (var reader = new StreamReader(@"data1.csv", Encoding.Default))
            using (var csv = new CsvReader(reader))
            {
                // 1行目を読み込む
                csv.Read();
                Console.WriteLine($"1行目の1列目 \"{csv.GetField(0)}\"");

                // 次の行を読み込む
                csv.Read();
                Console.WriteLine($"2行目の1列目 \"{csv.GetField(0)}\"");
            }


            // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
            // ヘッダとかフォーマットが決まってるなら、クラス定義すればそれに従って読み書きしてくれる。
            var data = new List<CsvModel>
            {
                new CsvModel { Name = "太郎", Age = 12 },
                new CsvModel { Name = "花子", Age = 13 },
            };

            using (var writer = new StreamWriter(@"data2.csv", false, Encoding.Default))
            using (var csv = new CsvWriter(writer))
            {
                // どのクラスに従って書き込むのかを登録
                csv.Configuration.RegisterClassMap<CsvModel.Map>();

                // ヘッダも書き込むならtrue
                csv.Configuration.HasHeaderRecord = true;

                csv.WriteRecords(data);
            }

            // ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            using (var reader = new StreamReader(@"data2.csv", Encoding.Default))
            using (var csv = new CsvReader(reader))
            {
                // どのクラスに従って書き込むのかを登録
                csv.Configuration.RegisterClassMap<CsvModel.Map>();
                // ヘッダがあるかどうかを指定
                csv.Configuration.HasHeaderRecord = true;

                // レコード全部読み込む
                var loadData = csv.GetRecords<CsvModel>();
            }

            Console.ReadLine();
        }
    }

    class CsvModel
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public class Map : ClassMap<CsvModel>
        {
            public Map()
            {
                // ヘッダとか、書き出す順番とかをClassMapクラスを作って制御できる。
                Map(t => t.Name).Name("名前");
                Map(t => t.Age).Name("年齢");
            }
        }
    }
}
