using System;
using System.Net;
using System.IO.Compression;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arcacon_Parser {
    public class Arcacon_Manager {
        string _url = "https://arca.live/e/";
        string _login_userid = string.Empty;
        string _login_userpw = string.Empty;
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
                    {"post_url", post_url.Split("?")[0] }
                });
            }

            // get last page num
            var last_li = _doc_node.SelectNodes("//a" + get_class_by_id("page-link")).Last();
            if (last_li == null) { return _retunner_data; }                                     // 없는 코드 처리 
            _retunner_data.Add(new Dictionary<string, dynamic>() {
                { "LAST_PAGE", int.Parse(last_li.Attributes["href"].Value.Split("p=")[1])}
            });

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
            string url = this._url + post_code.ToString();
            var _data = this._get_web_data(url);
            var node = _data.DocumentNode;
            var head_nodes = _data.DocumentNode.SelectSingleNode("//div" +this.get_class_by_id("article-head"));
            var body_nodes = _data.DocumentNode.SelectSingleNode("//div" +this.get_class_by_id("emoticons-wrapper"));


            //content parsing 
            var articles = head_nodes.SelectSingleNode("./div" + get_class_by_id("info-row clearfix"));
            var tags     = body_nodes.SelectSingleNode("./div" + get_class_by_id("emoticon-tags"));

            string title_stirng = head_nodes.SelectSingleNode("./div" + get_class_by_id("title-row") +"/div" +get_class_by_id("title")).InnerText ;
            string uploader     = articles.SelectSingleNode("./div" +get_class_by_id("member-info")).InnerText;
            int selling_count   = int.Parse(articles.SelectSingleNode("./div" + get_class_by_id("article-info")).SelectSingleNode("./span" + get_class_by_id("body")).InnerText);
            string update_date  = articles.SelectSingleNode("./div" + get_class_by_id("article-info")).SelectSingleNode(".//time").InnerText;
            List<string> _tags  = new();

            foreach(HtmlNode _node in tags.ChildNodes) {
                if (_node.Name != "a") { continue; }
                _tags.Add(_node.InnerText);
            }

            Dictionary<string, dynamic> _retunner_type = new();
            var _content_info = new Arca_Content_Jar(
                title_stirng, this._url +post_code.ToString(), uploader, selling_count, _tags, DateTime.Parse(update_date));
            _retunner_type.Add("INFO", _content_info);

            List<Arca_Content> _contents = new();
            var _emote_lists = body_nodes.ChildNodes;
            foreach(HtmlNode _node in _emote_lists) {
                if (_node.ParentNode != body_nodes) { continue; }   // 친부모 아니면 제거
                if (_node == _emote_lists.Last()) { continue; }     // 태그 제거


                bool isVideo = false;
                if (_node.Attributes["data-src"].Value.Split('.').Last() == "mp4") {
                    isVideo = true;
                }
                var _content_obj = new Arca_Content(
                    post_code, url, int.Parse(_node.Attributes["data-id"].Value),
                    "https:" +_node.Attributes["data-src"], isVideo
                ) ;

                _contents.Add(_content_obj);
            }

            _retunner_type.Add("CONTENTS", _contents);
            return _retunner_type;
        }

        /// <summary> 단순 이미지 다운로드 </summary>
        public bool _download_file(string url, string dir_name, string file_name) {
            string path = "./" + dir_name;
            DirectoryInfo _dir = new DirectoryInfo(path);
            _reset_directory(path);

            WebClient _wc = new WebClient();
            try { _wc.DownloadFile(url, "./" +dir_name +"/" +file_name+url.Split('.').Last()); }
            catch(Exception e) { return false; }
            return true;
        }


        /// <summary> 디렉터리 초기화용 </summary>
        private void _reset_directory(string dir_path) {
            DirectoryInfo _dir = new DirectoryInfo(dir_path);
            if (!_dir.Exists) { return; }
            foreach (var file in _dir.GetFiles("*", SearchOption.AllDirectories)) {
                file.Delete();
            }

            _dir.Delete(true);
            _dir.Create();
        }


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
    
}
