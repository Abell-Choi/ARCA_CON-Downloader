using System;
namespace Arcacon_Parser
{
    /// <summary>  </summary>
    public class Arca_Content_Jar {
        string content_name { get; }                 // 아카콘 이름 (DB)
        string post_url { get; }            // 아카콘 주소 (DB)
        string upload_user { get; }                  // 업로드 유저 (DB)
        int sell_count { get; }
        public List<string> tags { get; }
        DateTime? upload_time { get; }                          // 업로드 일시 (DB)
        DateTime update_time { get; }                              // 업데이트 일시 (DB)

        public Arca_Content_Jar(
            string title, string post_url, string upload_user, int sell_count, List<string> tags, DateTime? upload_time
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
        public int content_post_id { get; }
        public string content_post_url { get; }
        public int content_id { get; }
        public string content_url { get; }
        public bool isVideo { get; }

        public Arca_Content(
            int content_post_id, string content_post_url, int content_id, string content_url, bool isVideo
            ) {
            this.content_post_id = content_post_id;
            this.content_post_url = content_post_url;
            this.content_id = content_id;
            this.content_url = content_url;
            this.isVideo = isVideo;
        }

        public override string ToString()
        {
            new Dictionary<string, dynamic>();

                return "";
        }
    }
}

