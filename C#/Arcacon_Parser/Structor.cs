using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arcacon_Parser
{
    /// <summary>  </summary>
    public class Arca_Content_Jar {
        public int code { get; }
        public string title { get; }                 // 아카콘 이름 (DB)
        public string post_url { get; }            // 아카콘 주소 (DB)
        public string upload_user { get; }                  // 업로드 유저 (DB)
        public int sell_count { get; }
        public List<string> tags { get; }
        public DateTime? upload_time { get; }                          // 업로드 일시 (DB)
        public DateTime update_time { get; }                              // 업데이트 일시 (DB)

        public Arca_Content_Jar ( int content_title_code,
            string title, string post_url, string upload_user, int sell_count, List<string> tags, DateTime? upload_time, DateTime? update_time=null
        ) {
            this.code = content_title_code;
            this.title = title;
            this.post_url = post_url;
            this.upload_user = upload_user;
            this.sell_count = sell_count;
            this.tags = tags;
            this.upload_time = upload_time;
            if (update_time == null ) { update_time = DateTime.Now; }
            this.update_time = (DateTime) update_time;
           }

        public string _get_tag_lists ( ) {
            return JsonConvert.SerializeObject ( tags );
        }

        public override string ToString()
        {
            Dictionary<string, dynamic> d = new ( );
            d.Add ( "CODE", code );
            d.Add ( "CONTENT_TITLE", title );
            d.Add ( "POST_URL", post_url );
            d.Add ( "UPLOAD_USER", upload_user );
            d.Add ( "SELL_COUNT", sell_count );
            d.Add ( "TAGS", tags );
            d.Add ( "UPLOAD_TIME", upload_time );
            d.Add ( "UPDATE_TIME", update_time );

            return JsonConvert.SerializeObject(d, Formatting.Indented);
        }
    }

    // FOR DB 
    public class Arca_Content {

        public int content_title { get; }
        public string content_post_url { get; }
        public int content_id { get; }
        public string content_url { get; }
        public bool isVideo { get; }

        public Arca_Content(
            int content_post_id, string content_post_url, int content_id, string content_url, bool isVideo
            ) {
            this.content_title = content_post_id;
            this.content_post_url = content_post_url;
            this.content_id = content_id;
            this.content_url = content_url;
            this.isVideo = isVideo;
        }

        public override string ToString()
        {
            var d = new Dictionary<string, dynamic> ( );
            d.Add ( "CONTENT_TITLE", content_title );
            d.Add ( "CONTENT_POST_URL", content_post_url );
            d.Add ( "CONTENT_ID", content_id );
            d.Add ( "CONTENT_URL", content_url );
            d.Add ( "IS_VIDEO", isVideo );

            return JsonConvert.SerializeObject ( d , Formatting.Indented);
        }
    }
}

