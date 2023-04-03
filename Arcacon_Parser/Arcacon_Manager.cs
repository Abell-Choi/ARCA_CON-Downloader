using System;
using HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arcacon_Parser {
    public class Arcacon_Manager {
        string _url = "https://arca.live/e/";
        CookieContainer _cookie = new CookieContainer();


        private List<Dictionary<string,dynamic>> get_post_lists(string url) {
            HtmlDocument _doc = this._get_web_data(url);
            HtmlNode _doc_node = _doc.DocumentNode;

            List<Dictionary<string,dynamic>> _retunner_data = new();

            var emote_node_root = _doc_node.SelectSingleNode("//div" +get_class_by_id("emoticon-list"));
            HtmlNodeCollection emote_node_list = this._get_my_child_nodes(emote_node_root);

            if (emote_node_list == null) { throw new Exception("this node not exist child nodes"); }

            foreach(HtmlNode i in emote_node_list) {
                if (i.Name != "a") { continue; }
                string content_name = i.SelectSingleNode(".//div" + get_class_by_id("title")).InnerHtml;
                string upload_user  = i.SelectSingleNode(".//div" + get_class_by_id("maker")).InnerHtml;
                string post_url = i.Attributes["href"].Value;

                _retunner_data.Add(new Dictionary<string, dynamic>() {
                    {"content_name", content_name },
                    {"upload_user", upload_user },
                    {"post_url", post_url }
                });
            }
            return _retunner_data;
        }

        /// <summary> 아카콘 메인 페이지 데이터 가져오기 </summary>
        public List<Dictionary<string,dynamic>> _get_main_site(int index = 1, bool is_rank = false) {
            if (index < 1) { index = 1; }
            //http://arca.live/e/?p=1
            string url = this._url + "?p=" + index.ToString();
            url += is_rank ? "sort=rank" : "";
            return this.get_post_lists(url);
           
        }

        public List<Dictionary<string, dynamic>> _search_by_title(string title, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }
            string url  = this._url;
            url         += "?p=" + index.ToString();
            url         += "&target=title";
            url         += "&keyword=" +title;
            url         += is_rank?"sort=rank" : "";
            return this.get_post_lists(url);
        }

        public List<Dictionary<string, dynamic>> _search_by_nickname(string nickname, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }
            string url  = this._url;
            url         += "?p=" + index.ToString();
            url         += "&target=nickname";
            url         += "&keyword=" + nickname;
            url         += is_rank ? "sort=rank" : "";
            return this.get_post_lists(url);
        }


        /// <summary>  </summary>
        public List<Dictionary<string, dynamic>> _search_by_tag(string tag, bool is_rank = false, int index = 1) {
            if (index < 1) { index = 1; }
            string url  = this._url;
            url         += "?p=" + index.ToString();
            url         += "&target=tag";
            url         += "&keyword=" + tag;
            url         += is_rank ? "sort=rank" : "";
            return this.get_post_lists(url);

        }

        /// <summary> post id 의 아카콘 정보를 반환합니다. </summary>
        public Dictionary<string, dynamic> _get_post_data(int post_code) {
            // 테스트용 접속
            this._get_main_site();

            Thread.Sleep(2000);
            string url = this._url + post_code.ToString() +"?p=1";
            var _data = this._get_web_data(url);
            var node = _data.DocumentNode;
            var head_nodes = _data.DocumentNode.SelectSingleNode("//div" +this.get_class_by_id("article-head"));
            var body_nodes = _data.DocumentNode.SelectSingleNode("//div" +this.get_class_by_id("emoticons-wrapper"));


            //content parsing 
            var articles = head_nodes.SelectSingleNode("./div" + get_class_by_id("info-row clearfix"));
            var tags = body_nodes.SelectSingleNode("./div" + get_class_by_id("emoticon-tags"));

            string title_stirng = head_nodes.SelectSingleNode("./div" + get_class_by_id("title-row") +"/div" +get_class_by_id("title")).InnerText ;
            string uploader = articles.SelectSingleNode("./div" +get_class_by_id("member-info")).InnerText;
            int selling_count = int.Parse(articles.SelectSingleNode("./div" + get_class_by_id("article-info")).SelectSingleNode("./span" + get_class_by_id("body")).InnerText);
            string update_date = articles.SelectSingleNode("./div" + get_class_by_id("article-info")).SelectSingleNode(".//time").InnerText;
            List<string> _tags = new();
            foreach(HtmlNode _node in tags.ChildNodes) {
                if (_node.Name != "a") { continue; }
                _tags.Add(_node.InnerText);
            }

            Dictionary<string, dynamic> _retunner_type = new();
            var _content_info = new Arca_Content_Jar(
                title_stirng, this._url +post_code.ToString(), uploader, selling_count, _tags, DateTime.Parse(update_date));
            _retunner_type.Add("INFO", _content_info);

            Console.WriteLine(JsonConvert.SerializeObject((List<string>)_content_info.tags));

            return null;
        }
        void _download_image(string url) { }
        void _download_video(string url) { }
        void _convert_gif(string url) { }


        /// <summary> Html Document Parser </summary>
        public HtmlDocument _get_web_data(string url = "https://arca.live/e/") {
            Console.WriteLine("URL parsing... -> " + url);
            HttpWebRequest _req = (HttpWebRequest) WebRequest.Create(url);
            _req.CookieContainer = this._cookie;
            _req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            WebResponse _res;
            // html parsing
            try {
                _res = _req.GetResponse();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return null;
            }
            Stream _stream = _res.GetResponseStream();
            HtmlDocument _doc = new HtmlDocument();
            _doc.Load(_stream);
            return _doc;
        }

        /*============== HtmlAgilityPack Utility ==============*/
        string get_class_by_id(string id) {
            //return "//*[contains(@class, '" + id + "')]";
            return "[@class='" + id + "']";
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
        string content_name { get;  }                 // 아카콘 이름 (DB)
        string post_url { get;  }            // 아카콘 주소 (DB)
        string upload_user { get; }                  // 업로드 유저 (DB)
        int sell_count { get; }
        public List<string> tags { get; }
        DateTime? upload_time { get; }                          // 업로드 일시 (DB)
        DateTime update_time  { get; }                              // 업데이트 일시 (DB)

        public Arca_Content_Jar(
            string title, string post_url, string upload_user, int sell_count,List<string> tags, DateTime? upload_time
        ) {
            this.content_name = title;
            this.post_url = post_url;
            this.upload_user = upload_user;
            this.sell_count = sell_count;
            this.upload_time = upload_time;
            this.update_time = DateTime.Now;
        }
    }

    // FOR DB 
    public class Arca_Content {
        int data_id = -1;
        string content_name = string.Empty;                 // 해당 아카콘의 글 이름
        string content_url  = string.Empty;                 // 해당 아카콘의 주소
        bool is_video       = false;                        // 이미지 인지
        bool is_convert     = false;                        // mp4 -> gif화 된건지
        string? convert_url = string.Empty;                 // gif 변환된 주소
    }
}
