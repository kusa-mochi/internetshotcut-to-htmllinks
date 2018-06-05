using System;
using System.IO;
using System.Text;
using System.Windows;

namespace InternetShortcutHTMLMaker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

            if (files != null)
            {
                string[] urls = new string[files.Length];
                string[] titles = new string[files.Length];

                // 各ファイルを UTF-8 テキストとして読み込む
                for (int i = 0; i < files.Length; i++)
                {
                    // ファイル名を取得する。
                    titles[i] = System.IO.Path.GetFileNameWithoutExtension(files[i]);

                    // ファイルの拡張子を取得する。
                    string ext = System.IO.Path.GetExtension(files[i]);

                    // ファイルの拡張子がインターネットショートカットのものでない場合
                    if ((ext != @".url") && (ext != @".website")) continue;

                    using (var fileReader = new StreamReader(files[i], Encoding.UTF8))
                    {
                        string line = "";
                        while ((line = fileReader.ReadLine()) != null)
                        {
                            // 文字列の先頭4文字を取得する。
                            string lineHeader = line.Substring(0, 4);

                            // 文字列の先頭が URL= の場合
                            if (lineHeader == "URL=")
                            {
                                // インターネットショートカットに含まれるURLを取得する。
                                urls[i] = line.Substring(4);
                                break;
                            }
                        }
                    }
                }

                MakeHTMLFile(titles, urls);
            }
        }

        private void Border_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            // ファイルをドロップされた場合のみ e.Handled を True にする
            e.Handled = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop);
        }

        private void MakeHTMLFile(string[] titles, string[] urls)
        {
            DateTime now = DateTime.Now;
            string htmlFileName = @"links-" + now.Year.ToString("D4") + "-" + now.Month.ToString("D2") + "-" + now.Day.ToString("D2") + "-" + now.Hour.ToString("D2") + "-" + now.Minute.ToString("D2") + "-" + now.Second.ToString("D2") + ".html";
            string htmlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + htmlFileName;

            // jquery
            string jQueryString = "";
            using (var fileReader = new StreamReader(@"./js/jquery-3.3.1.min.js"))
            {
                jQueryString = fileReader.ReadToEnd();
            }

            // bootstrap - js
            string bootstrapJsString = "";
            using (var fileReader = new StreamReader(@"./js/bootstrap.min.js"))
            {
                bootstrapJsString = fileReader.ReadToEnd();
            }

            // bootstrap - css
            string bootstrapCssString = "";
            using (var fileReader = new StreamReader(@"./css/bootstrap.min.css"))
            {
                bootstrapCssString = fileReader.ReadToEnd();
            }

            using (var fileWriter = new StreamWriter(htmlFilePath, false, Encoding.UTF8))
            {
                string fileData = "";
                fileData += "<!DOCTYPE html>";
                fileData += "<html>";
                fileData += "<head>";
                fileData += "<meta charset=\"UTF-8\">";
                fileData += "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\">";
                fileData += "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
                fileData += "<title>リンク集 - " + htmlFileName + "</title>";
                fileData += "<script>";
                fileData += jQueryString;
                fileData += "</script>";
                fileData += "<script>";
                fileData += bootstrapJsString;
                fileData += "</script>";
                fileData += "<style>";
                fileData += bootstrapCssString;
                fileData += "</style>";
                fileData += "<style>";
                fileData += ".table-striped { table-layout: fixed; width: 100%; }";
                fileData += "div.checkbox { margin-left: 12px; margin-top: 10px; }";
                fileData += ".table_checkbox { width: 24px; height: 24px; }";
                fileData += ".table_checkbox_header { width: 50px; }";
                fileData += ".table_title_header { min-width: 40%; }";
                fileData += ".table-striped td, .table-striped th { padding: 16px; }";
                fileData += ".break_word_link { word-wrap: break-word; }";
                fileData += ".block_link_wrapper { position: relative; }";
                fileData += ".block_link { display: flex; align-items: center; position: absolute; width: 100%; height: 100%; top: 0; left: 0; padding: 16px; text-decoration: none; }";
                fileData += ".block_link:hover { background-color: #d1e2ff; color: #111111; text-decoration: none; }";
                fileData += "</style>";
                fileData += "</head>";
                fileData += "<body>";
                fileData += "<table class=\"table-striped\">";
                fileData += "<thead>";
                fileData += "<tr>";
                fileData += "<th class=\"table_checkbox_header\"></th>";
                fileData += "<th class=\"table_title_header\">タイトル</th>";
                fileData += "<th>リンク</th>";
                fileData += "</tr>";
                fileData += "</thead>";
                fileData += "<tbody>";

                for (int i = 0; i < titles.Length; i++)
                {
                    fileData += "<tr>";
                    fileData += "<td><div class=\"checkbox\"><label><input class=\"table_checkbox\" type=\"checkbox\" value=\"\"/></label></div></td>";
                    fileData += "<td>";
                    fileData += titles[i];
                    fileData += "</td>";
                    fileData += "<td class=\"block_link_wrapper\">";
                    fileData += "<a class=\"break_word_link block_link\" href=\"";
                    fileData += urls[i];
                    fileData += "\" target=\"_blank\">";
                    fileData += urls[i];
                    fileData += "</a>";
                    fileData += "</td>";
                    fileData += "</tr>";
                }

                fileData += "</tbody>";
                fileData += "</table>";
                fileData += "</body>";
                fileData += "</html>";
                fileWriter.Write(fileData);
            }

            // 完了のメッセージを表示する。
            MessageBox.Show("デスクトップにHTMLファイル " + htmlFileName + " を生成しました。");
        }
    }
}
