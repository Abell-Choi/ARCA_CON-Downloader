using System;
using System.Net;
using System.IO.Compression;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

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
        public Dictionary<string, dynamic> _get_post_data ( int post_code ) {
            // 테스트용 접속
            this._get_main_site( );

            Thread.Sleep( 2000 );
            string url = this._url + post_code.ToString( );
            var _data = this._get_web_data( url );
            var node = _data.DocumentNode;
            var head_nodes = _data.DocumentNode.SelectSingleNode( "//div" + this.get_class_by_id( "article-head" ) );
            var body_nodes = _data.DocumentNode.SelectSingleNode( "//div" + this.get_class_by_id( "emoticons-wrapper" ) );


            //content parsing 
            var articles = head_nodes.SelectSingleNode( "./div" + get_class_by_id( "info-row clearfix" ) );
            var tags = body_nodes.SelectSingleNode( "./div" + get_class_by_id( "emoticon-tags" ) );


            // 타이틀, 업로더, 판매수, 등록일, 태그들
            string title_stirng = head_nodes.SelectSingleNode( "./div" + get_class_by_id( "title-row" ) + "/div" + get_class_by_id( "title" ) ).InnerText;
            string uploader = articles.SelectSingleNode( "./div" + get_class_by_id( "member-info" ) ).InnerText;
            int selling_count = int.Parse( articles.SelectSingleNode( "./div" + get_class_by_id( "article-info" ) ).SelectSingleNode( "./span" + get_class_by_id( "body" ) ).InnerText );
            string update_date = articles.SelectSingleNode( "./div" + get_class_by_id( "article-info" ) ).SelectSingleNode( ".//time" ).InnerText;
            List<string> _tags = new( );

            // 태그 긁어오기
            if ( tags != null ) {
                foreach ( HtmlNode _node in tags.ChildNodes ) {
                    if ( _node.Name != "a" ) {
                        continue;
                    }
                    _tags.Add( _node.InnerText );
                }
            }


            Dictionary<string, dynamic> _retunner_type = new( );
            var _content_info = new Arca_Content_Jar(
                post_code,
                title_stirng,
                this._url + post_code.ToString( ),
                uploader,
                selling_count,
                _tags,
                DateTime.Parse( update_date )
            );
            _retunner_type.Add( "INFO", _content_info );

            List<Arca_Content> _contents = _get_content_in_wrapper(_data, post_code);
            _retunner_type.Add( "CONTENTS", _contents );
            return _retunner_type;
        }

        /// <summary> 아카콘만 가져옴 </summary>
        private List<Arca_Content> _get_content_in_wrapper ( HtmlDocument _post_docs, int post_id ) {
            if ( _post_docs == null ) { return null; }
            HtmlNode _node = _post_docs.DocumentNode;
            var _wrapper_node = _node.SelectSingleNode( "//div" +get_class_by_id("emoticons-wrapper"));
            if ( _wrapper_node == null ) { return null; }
            List<HtmlNode> _content_nodes = new (){ };
            // content 분류
            foreach(HtmlNode node in _wrapper_node.ChildNodes ) {
                if (node.ParentNode != _wrapper_node){continue;}
                if (!new List<string>(){ "video", "img"}.Contains(node.Name) ){continue;}
                _content_nodes.Add(node);
            }

            List<Arca_Content> _contents = new(){ };
            foreach(HtmlNode node in _content_nodes ) {
                int content_post_id = post_id;
                string content_post_url = $"{_url}{post_id.ToString()}";
                int content_id = int.Parse( node.Attributes["data-id"].Value );
                bool is_video = node.Name == "video"?true:false;
                string content_url = string.Empty;

                if (node.Attributes["data-src"] != null){
                    content_url = "https:" + node.Attributes["data-src"].Value;
                }
                else{content_url = "https:" +node.Attributes["src"].Value;}
                content_url = content_url.Replace("amp;", "");
                _contents.Add( new Arca_Content( content_post_id, content_post_url, content_id, content_url, is_video ));
            }

            return _contents;
        }

        private void _init_download_path ( ) {
            if (!Directory.Exists($"./download")){Directory.CreateDirectory($"./download");}
        }

        private void _init_title_path(int title_code , bool reset_data = false) {
            _init_download_path();
            if ( !Directory.Exists( $"./download/{title_code.ToString( )}" ) ) {
                Directory.CreateDirectory( $"./download/{title_code.ToString( )}" );
                return;
            }
            if ( reset_data ) {
                _reset_directory($"./download/{title_code.ToString()}");
            }
            return;
        }

        public bool _download_file(Arca_Content _content ) {
            _init_title_path(_content.content_post_id);
            string path = new DirectoryInfo($"./download/{_content.content_post_id.ToString()}").FullName;

            string file_name = _content.isVideo ? $"{path}/{_content.content_id.ToString( )}.mp4" :
                $"{path}/{_content.content_id.ToString( )}.png";

            string download_url = _content.content_url;
            WebClient _wc = new WebClient();
            try{_wc.DownloadFile($"{download_url}", $"{file_name}");}
            catch(Exception e){Console.WriteLine(e.ToString()); return false;}

            if ( file_name.Contains( ".mp4" ) ) {
                _convert_video($"{file_name}");
            }

            return true;
        }

        public bool _convert_video(string file_path) {
            string input_path = new FileInfo(file_path).FullName;

            string ffmpeg_args = $"-i {input_path} {input_path.Replace("mp4", "gif")}";
            var ffmpeg_process = new Process{
                StartInfo = new ProcessStartInfo{
                    FileName = "ffmpeg",
                    Arguments = ffmpeg_args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffmpeg_process.Start();
            ffmpeg_process.WaitForExit();

            new FileInfo(file_path).Delete();
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
