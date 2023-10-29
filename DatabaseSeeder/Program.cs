using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data.SQLite;
using System.Drawing;

namespace DatabaseSeeder
{
    internal class Program
    {
        static private Bitmap ResizeBitmap(Bitmap original, int width, int height, System.Drawing.Drawing2D.InterpolationMode interpolationMode)
        {
            Bitmap bmpResize;
            Bitmap bmpResizeColor;
            Graphics graphics = null;

            try
            {
                System.Drawing.Imaging.PixelFormat pf = original.PixelFormat;

                if (original.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    // モノクロの時は仮に24bitとする
                    pf = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                }

                bmpResizeColor = new Bitmap(width, height, pf);
                var dstRect = new RectangleF(0, 0, width, height);
                var srcRect = new RectangleF(-0.5f, -0.5f, original.Width, original.Height);
                graphics = Graphics.FromImage(bmpResizeColor);
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = interpolationMode;
                graphics.DrawImage(original, dstRect, srcRect, GraphicsUnit.Pixel);

            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }
            }

            if (original.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                // モノクロ画像のとき、24bit→8bitへ変換

                // モノクロBitmapを確保
                bmpResize = new Bitmap(
                    bmpResizeColor.Width,
                    bmpResizeColor.Height,
                    System.Drawing.Imaging.PixelFormat.Format8bppIndexed
                    );

                var pal = bmpResize.Palette;
                for (int i = 0; i < bmpResize.Palette.Entries.Length; i++)
                {
                    pal.Entries[i] = original.Palette.Entries[i];
                }
                bmpResize.Palette = pal;

                // カラー画像のポインタへアクセス
                var bmpDataColor = bmpResizeColor.LockBits(
                        new Rectangle(0, 0, bmpResizeColor.Width, bmpResizeColor.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        bmpResizeColor.PixelFormat
                        );

                // モノクロ画像のポインタへアクセス
                var bmpDataMono = bmpResize.LockBits(
                        new Rectangle(0, 0, bmpResize.Width, bmpResize.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        bmpResize.PixelFormat
                        );

                int colorStride = bmpDataColor.Stride;
                int monoStride = bmpDataMono.Stride;

                unsafe
                {
                    var pColor = (byte*)bmpDataColor.Scan0;
                    var pMono = (byte*)bmpDataMono.Scan0;
                    for (int y = 0; y < bmpDataColor.Height; y++)
                    {
                        for (int x = 0; x < bmpDataColor.Width; x++)
                        {
                            // R,G,B同じ値のため、Bの値を代表してモノクロデータへ代入
                            pMono[x + y * monoStride] = pColor[x * 3 + y * colorStride];
                        }
                    }
                }

                bmpResize.UnlockBits(bmpDataMono);
                bmpResizeColor.UnlockBits(bmpDataColor);

                //　解放
                bmpResizeColor.Dispose();
            }
            else
            {
                // カラー画像のとき
                bmpResize = bmpResizeColor;
            }

            return bmpResize;
        }

        static void GenerateDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=mydb.sqlite"))
            {
                conn.Open();

                string sql = "";
                using (var reader = new StreamReader("./Sqls/create.sql"))
                {
                    sql = reader.ReadToEnd();
                }
                Console.WriteLine(sql);

                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        static void Seeding()
        {
            Dictionary<string, int> map = new Dictionary<string, int>();

            Console.WriteLine("ファイル一覧を取得");
            for (int i = 0; i < 180; ++i)
            {
                var img_files = Directory.GetFiles($"D:\\img\\{i}\\", "*");
                foreach (var file in img_files)
                {
                    var basename = file.Split('.')[0].Split('\\').Last();
                    map[basename] = i;
                }
            }
            Console.WriteLine("ファイルをマップに変換");

            Console.WriteLine("CSVファイルを展開");
            using (var reader = new StreamReader("D:\\mongo.csv"))
            {
                /*
                 * [0] 1,
                 * [1] d34e4cf0a437a5d65f8e82b7bcd02606,
                 * [2] jpg,
                 * [3] 127238,
                 * [4] 459,
                 * [5] 650,
                 * [6] 1girl ;p animal_ears bangs blue_bow blue_panties blue_ribbon blush bow bow_panties breasts brown_eyes cat_ears cat_girl cat_tail collarbone cowboy_shot eyebrows eyebrows_visible_through_hair kemonomimi_mode kousaka_tamaki kyougoku_shin large_breasts long_hair long_sleeves looking_at_viewer no_pants one_eye_closed orange_background panties red_hair ribbon school_uniform serafuku smile solo standing striped striped_background striped_panties tail thigh_gap thighhighs thighs to_heart_2 tongue tongue_out underwear very_long_hair white_legwear,
                 * [7] /data/__kousaka_tamaki_to_heart_2_drawn_by_kyougoku_shin__d34e4cf0a437a5d65f8e82b7bcd02606.jpg
                 */

                var line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var sep = line.Split(',');
                    var hash = sep[1];
                    var ext = sep[2];
                    var length = int.Parse(sep[3]);
                    var tags = sep[6].Split(' ');

                    // たまに存在しないファイルがある
                    if (!map.ContainsKey(hash)) continue;
                    var path = $"D:\\img\\{map[hash]}\\{hash}.{ext}";

                    // ファイルを読み込む
                    byte[] bytes_image;
                    using (var file = new BinaryReader(File.OpenRead(path)))
                    {
                        bytes_image = file.ReadBytes(length);
                    }

                    // ビットマップとして読み込む
                    Bitmap bitmap, thumbnail;
                    byte[] bytes_thumbnail;
                    using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        bitmap = new Bitmap(file);
                    }
                    var long_side = Math.Min(bitmap.Width, bitmap.Height);
                    var powW = bitmap.Width / long_side * 100;
                    var powH = bitmap.Height / long_side * 100;
                    thumbnail = ResizeBitmap(bitmap, (int)powW, (int)powH, System.Drawing.Drawing2D.InterpolationMode.High);

                    var converter = new ImageConverter();
                    bytes_thumbnail = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                }
            }
            Console.WriteLine("ファイルの取得が完了しました");
        }

        static void DropDatabase()
        {
            using (var conn = new SQLiteConnection("Data Source=mydb.sqlite"))
            {
                conn.Open();

                string sql = "";
                using (var reader = new StreamReader("./Sqls/drop.sql"))
                {
                    sql = reader.ReadToEnd();
                }
                Console.WriteLine(sql);

                using (var command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(System.Data.SQLite.SQLiteException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                conn.Close();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("モードを切り替えます");
            Console.WriteLine("1. データベース作成");
            Console.WriteLine("2. シード");
            Console.WriteLine("3. データベースをdropする");
            var s = Console.ReadLine();
            
            switch (s)
            {
                case "1":
                    GenerateDatabase();
                    break;
                case "2":
                    Seeding();
                    break;
                case "3":
                    DropDatabase();
                    break;
                default:
                    Console.WriteLine("終了します");
                    break;
            }

            Console.WriteLine("プログラムが終了しました");
            Environment.Exit(0);
        }
    }
}
