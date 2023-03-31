using System;
using HtmlAgilityPack;
using System.Net;

namespace Arcacon_Parser {
    public class Arcacon_Manager {
        string _url = "https://arca.live/e/";


        /// <summary> 아카콘 메인 페이지 데이터 가져오기 </summary>
        public List<Dictionary<string,dynamic>> _get_main_site(int index = 1) {
            if (index < 1) { index = 1; }
            //http://arca.live/e/?p=1
            HtmlDocument _doc = this._get_web_data(this._url +"?p="+index.ToString());
            var node = _doc.DocumentNode;

            List< Dictionary<string, dynamic> >_retunner_data = new();


            var emote_node_root = node.SelectSingleNode(get_class_by_id("emoticon-list"));
            HtmlNodeCollection emote_node_lists = _get_my_child_nodes(emote_node_root);

            foreach(HtmlNode i in emote_node_lists) {
                if (i.Name != "a") { continue; }
                string content_name = i.SelectSingleNode(".//div" +get_class_by_id("title")).InnerHtml;
                string upload_user = i.SelectSingleNode(".//div" +get_class_by_id("maker")).InnerHtml ;
                string post_url = i.Attributes["href"].Value;

                _retunner_data.Add(new Dictionary<string, dynamic>() {
                    {"content_name", content_name },
                    {"upload_user", upload_user },
                    {"post_url", post_url }
                });
            }
           
            return _retunner_data;
           
        }

        void _search_by_title(string title, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }

        }
        void _search_by_nickname(string nickname, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }
        }
        void _search_by_tag(string tag, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }
        }

        void _get_post_data(string post_url) {}
        void _get_all_content_urls(string post_url) { }

        void _download_image(string url) { }
        void _download_video(string url) { }
        void _convert_gif(string url) { }


        /// <summary> Html Document Parser </summary>
        public HtmlDocument _get_web_data(string url = "https://arca.live/e/") {
            Console.WriteLine("URL parsing... -> " + url);
            WebRequest _req = WebRequest.Create(url);
            WebResponse _res;
            // html parsing
            try {
                _res = _req.GetResponse();
            } catch (Exception e) {
                return null;
            }
            Stream _stream = _res.GetResponseStream();
            HtmlDocument _doc = new HtmlDocument();
            _doc.Load(_stream);
            return _doc;
        }

        /*============== HtmlAgilityPack Utility ==============*/
        string get_class_by_id(string id) {
            return "//*[contains(@class, '" + id + "')]";
        }

        HtmlNodeCollection _get_my_child_nodes(HtmlNode _target_node) {
            HtmlNodeCollection _nodes = _target_node.ChildNodes;
            foreach (var _node in _nodes) {
                if (_node.ParentNode != _target_node) { _node.Remove(); }
            }

            return _nodes;
        }
    }
    /// <summary>  </summary>
    public class Arca_Content_Jar {
        string content_name = string.Empty;                 // 아카콘 이름 (DB)
        string post_url = string.Empty;             // 아카콘 주소 (DB)
        string upload_user = string.Empty;                  // 업로드 유저 (DB)
        int sell_count = 0;
        DateTime? upload_time;                               // 업로드 일시 (DB)
        DateTime update_time;                               // 업데이트 일시 (DB)

        public Arca_Content_Jar(
            string title, string post_url, string upload_user, int sell_count, DateTime? upload_time
        ) {
            this.content_name = title;
            this.post_url = post_url;
            this.upload_user = upload_user;
            this.sell_count = sell_count;
            this.upload_time = upload_time;
            this.update_time = DateTime.Now;
        }

        public void set_upload_time(DateTime upload_time) {
            this.upload_time = upload_time;
        }
    }

    // FOR DB 
    public class Arca_Content {
        int data_id = -1;
        string content_name = string.Empty;                 // 해당 아카콘의 글 이름
        string content_url = string.Empty;                 // 해당 아카콘의 주소
        bool is_video = false;                        // 이미지 인지
        bool is_convert = false;                        // mp4 -> gif화 된건지
        string? convert_url = string.Empty;                 // gif 변환된 주소
    }
}