using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data.SQLite;
using System.Drawing;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace DatabaseSeeder
{
    internal class Program
    {
        //private static string DataSource = "Data Source=E:\\danbooru\\database.sqlite";
        private static string DataSource = @"Data Source=\\\\Synology1\共有フォルダ\danbooru\database.sqlite";
        private static string MongoSource = @"C:\mongo.csv";
        static private string DumpFilesPath = @"D:\dump_files.csv";


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
            using (var conn = new SQLiteConnection(DataSource))
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


        static void DumpFiles()
        {
            Console.WriteLine("ファイルをダンプします");
            using (var writer = new StreamWriter(DumpFilesPath))
            {
                for (int i = 0; i < 180; ++i)
                {
                    var img_files = Directory.GetFiles($"D:\\img\\{i}\\", "*");
                    foreach (var file in img_files)
                    {
                        writer.WriteLine(file);
                    }
                }
            }
        }

        static void SeedingPicture()
        {
            var conn = new SQLiteConnection(DataSource);
            conn.Open();

            Dictionary<string, string> map = new Dictionary<string, string>();

            Console.WriteLine("ファイル一覧を取得");
            using (var reader = new StreamReader(DumpFilesPath))
            {
                var file = "";
                while ((file = reader.ReadLine()) != null)
                {
                    var info = new FileInfo(file);
                    var basename = info.Name.Split('.')[0];
                    map[basename] = file;
                }
            }
            Console.WriteLine("ファイルをマップに変換");
            Console.WriteLine("CSVファイルを展開");

            var insert_sql = "";
            using (var com = new StreamReader("./Sqls/insert_picture.sql"))
            {
                insert_sql = com.ReadToEnd();
            }

            using (var command = new SQLiteCommand("select max(id) from pictures;", conn))
            {
                var obj = command.ExecuteScalar();
                if (obj != null)
                {
                    try
                    {
                        var id = (long)obj;
                        Console.WriteLine($"現在挿入されている最大IDは{id}件です");
                    }
                    catch
                    {
                        Console.WriteLine("まだデータは挿入されていません");
                    }
                }
                else
                {
                    Console.WriteLine("まだデータは挿入されていません");
                }
            }

            Console.WriteLine("N万件のデータを挿入します。Nを入力してください");

            long id10k;
            var id10kStr = "";
            do
            {
                id10kStr = Console.ReadLine();
            } while (!long.TryParse(id10kStr, out id10k));

            using (var reader = new StreamReader(MongoSource))
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

                using (var transaction = conn.BeginTransaction())
                {
                    var line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sep = line.Split(',');
                        if (sep.Length <= 6) continue;
                        var id = long.Parse(sep[0]);
                        var hash = sep[1];
                        if (!map.ContainsKey(hash))
                        {
                            Console.WriteLine($"見つかりませんでした: {hash}");
                            continue;
                        }
                        var path = map[hash];
                        var ext = map[hash].Split('.').Last();
                        var length = new FileInfo(map[hash]).Length;
                        var tags = sep[6].Split(' ');

                        using (var command = new SQLiteCommand("select id from pictures where sha1 = @hash limit 1;", conn))
                        {
                            command.Parameters.Add(new SQLiteParameter("@hash", hash));
                            var obj = command.ExecuteScalar();
                            if (obj != null) continue;
                        }


                        if (id % 1000 == 0)
                        {
                            Console.WriteLine($"{id,7}件目までのデータです");
                        }

                        // 足きり 次は90万件
                        if (id % (id10k * 10000) == 0)
                        {
                            break;
                        }

                        // ファイルを読み込む
                        byte[] bytes_image;
                        try
                        {
                            using (var file = new BinaryReader(File.OpenRead(path)))
                            {
                                bytes_image = file.ReadBytes((int)length);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"ファイルが存在しません: {path}");
                            continue;
                        }

                        // ビットマップとして読み込む
                        Bitmap bitmap, thumbnail;
                        byte[] bytes_thumbnail;
                        using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            try
                            {
                                bitmap = new Bitmap(file);
                            }
                            catch
                            {
                                Console.WriteLine($"変換に失敗しました: {path}");
                                continue;
                            }
                        }
                        // アスペクト比を計算して単位長さに縮小する
                        var long_side = Math.Max(bitmap.Width, bitmap.Height);
                        var powW = (double)bitmap.Width / long_side * 100;
                        var powH = (double)bitmap.Height / long_side * 100;
                        try
                        {
                            thumbnail = ResizeBitmap(bitmap, (int)powW, (int)powH, System.Drawing.Drawing2D.InterpolationMode.High);
                        }
                        catch
                        {
                            // 無効な画像サイズ対策
                            continue;
                        }
                        thumbnail.Save("./tmp.jpg");    // tmpファイルとしてjpegを書き出す

                        var picture_info = new FileInfo(path);
                        var fileinfo = new FileInfo("./tmp.jpg");

                        // JPEGになったサムネイルのバイト列を取得する
                        using (var tmp = new BinaryReader(File.OpenRead("./tmp.jpg")))
                        {
                            bytes_thumbnail = tmp.ReadBytes((int)fileinfo.Length);
                        }

                        using (var command = new SQLiteCommand(insert_sql, conn))
                        {
                            command.Parameters.Add(new SQLiteParameter("@picture", bytes_image));
                            command.Parameters.Add(new SQLiteParameter("@thumbnail", bytes_thumbnail));
                            command.Parameters.Add(new SQLiteParameter("@sha1", hash));
                            command.Parameters.Add(new SQLiteParameter("@ext", ext));
                            command.Parameters.Add(new SQLiteParameter("@width", bitmap.Width));
                            command.Parameters.Add(new SQLiteParameter("@height", bitmap.Height));
                            command.Parameters.Add(new SQLiteParameter("@import_path", path));
                            command.Parameters.Add(new SQLiteParameter("@created_at", picture_info.CreationTime.ToString()));
                            command.Parameters.Add(new SQLiteParameter("@saved_at", DateTime.Now.ToString()));

                            try
                            {
                                var result = command.ExecuteNonQuery();
                            }
                            catch
                            {
                                //Console.WriteLine($"既に存在するオブジェクトです: {hash}");
                                continue;
                            }
                        }
                    }
                    transaction.Commit();
                    Console.WriteLine("画像の挿入が完了しました");
                }
            }
            conn.Close();
        }

        static void MigrateRootTag() 
        {
            var conn = new SQLiteConnection(DataSource);
            conn.Open();

            Console.WriteLine("ルートのタグを挿入します");

            using (var transaction = conn.BeginTransaction())
            {
                int maxid = 0;
                var text = "select max(id) from pictures limit 1;";
                using (var command = new SQLiteCommand(text, conn))
                {
                    maxid = (int)command.ExecuteScalar();
                }

                for (int i = 1; i <= maxid; ++i)
                {
                    using (var command = new SQLiteCommand("insert into tag2pic(tag_id, pic_id) values (@tag_id, @pic_id);", conn))
                    {
                        command.Parameters.Add(new SQLiteParameter("@tag_id", 1));
                        command.Parameters.Add(new SQLiteParameter("@pic_id", i));
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch 
                        { 
                            continue; 
                        }
                        
                    }
                }
                transaction.Commit();
                Console.WriteLine("ルートタグの挿入を完了しました");
            }
            Console.WriteLine("ファイルの取得が完了しました");
            conn.Close();
            Console.WriteLine("データベースを閉じました");
        }

        static void MigrateTags()
        {
            var conn = new SQLiteConnection(DataSource);
            conn.Open();

            Console.WriteLine("データベースとのコネクションを確立しました");

            using (var transaction = conn.BeginTransaction())
            {
                Console.WriteLine("トランザクションを開始します");
                transaction.Rollback();
                Console.WriteLine("不要なジャーナルをロールバックします");

                var tagSql = "";
                using (var r = new StreamReader("./Sqls/insert_tag.sql"))
                {
                    tagSql = r.ReadToEnd();
                }

                using (var reader = new StreamReader(MongoSource))
                {
                    var line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sep = line.Split(',');
                        if (sep.Length <= 6) continue; 
                        var hash = sep[1];
                        var tags = sep[6].Split(' ');
                        long id = 0;

                        // データベース上のIDを特定する
                        try
                        {
                            using (var command = conn.CreateCommand())
                            {
                                command.CommandText = "select id from pictures where sha1 = @hash limit 1";
                                command.Parameters.Add(new SQLiteParameter("@hash", hash));
                                var obj = command.ExecuteScalar();
                                if (obj == null) continue;  // ファイルが存在しない
                                id = (long)obj;
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"ファイルが見つかりませんでした: {hash}");
                            continue;
                        }

                        // タグを追加する
                        for (int i = 0; i < tags.Length; ++i)
                        {
                            using (var command = new SQLiteCommand(tagSql, conn))
                            {
                                command.Parameters.Add(new SQLiteParameter("@name", tags[i]));
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch
                                {
                                    // 既存のタグが追加された
                                    continue;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("CSVファイルのロードが完了しました");
                Console.WriteLine("トランザクションをコミットします");
                transaction.Commit();
            }
            Console.WriteLine("マイグレーションを終了します");
            conn.Close();
        }

        static void MigrateTag2Pic()
        {
            using (var conn = new SQLiteConnection(DataSource))
            {
                Console.WriteLine("コネクションを確立しました");
                conn.Open();
                Console.WriteLine("コネクションをオープンします");

                using (var transaction =  conn.BeginTransaction())
                {
                    Console.WriteLine("トランザクションを開始します");

                    using (var reader = new StreamReader(MongoSource))
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
                        var tag2picSql = "";
                        using (var r = new StreamReader("./Sqls/insert_tag2pic_with_name.sql"))
                        {
                            tag2picSql = r.ReadToEnd();
                        }

                        var line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            var sep = line.Split(',');
                            var id = long.Parse(sep[0]);
                            var tags = sep[6].Split(' ');
                            var hash = sep[1];

                            using (var command = new SQLiteCommand("select id from pictures where sha1 = @hash limit 1;", conn))
                            {
                                command.Parameters.Add(new SQLiteParameter("@hash", hash));

                                var obj = command.ExecuteScalar();
                                if (obj == null) continue;
                                id = (long)obj;
                            }

                            foreach (var tag in tags)
                            {
                                using (var command = new SQLiteCommand(tag2picSql, conn))
                                {
                                    command.Parameters.Add(new SQLiteParameter("@name", tag));
                                    command.Parameters.Add(new SQLiteParameter("@pic_id", id));
                                    
                                    try
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    transaction.Commit();
                    Console.WriteLine("トランザクションをコミットしました");
                }

                conn.Close();
                Console.WriteLine("コネクションを閉じました");
            }
        }

        static void DropDatabase()
        {
            using (var conn = new SQLiteConnection())
            {
                conn.ConnectionString = "Data Source=" + DataSource;
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
            Console.WriteLine("0. ファイル一覧をダンプする");
            Console.WriteLine("1. データベース作成");
            Console.WriteLine("2. 画像の設定");
            Console.WriteLine("3. ルートタグの設定");
            Console.WriteLine("4. 画像にタグ付けを行う");
            Console.WriteLine("5. データベースをdropする");
            var s = Console.ReadLine();
            
            switch (s)
            {
                case "0":
                    DumpFiles();
                    break;
                case "1":
                    GenerateDatabase();
                    break;
                case "2":
                    SeedingPicture();
                    break;
                case "3":
                    MigrateTags();
                    break;
                case "4":
                    MigrateTag2Pic();
                    break;
                case "5":
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
