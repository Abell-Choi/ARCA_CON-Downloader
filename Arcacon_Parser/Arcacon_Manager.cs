using System;
namespace Arcacon_Parser {
    public class Arcacon_Manager {
        string _url = "arca.live/e/";


        void _search_by_title(string title, bool is_rank=false) { }
        void _search_by_nickname(string title) { }
        void _search_by_tag(string tag) { }

        void _get_post_data(string post_url) { }
        void _get_all_content_urls(string post_url) { }

        void _download_image(string url) { }
        void _download_video(string url) { }
        void _convert_gif(string url) { }
    }

    public class Arca_Content_Jar {
        string content_name = string.Empty;                 // 아카콘 이름 (DB)
        string content_post_url = string.Empty;             // 아카콘 주소 (DB)
        List<Arca_Content> content_list = new() ;           // 파일 내용들 (DB)
        string upload_user = string.Empty;                  // 업로드 유저 (DB)
        DateTime upload_time;                               // 업로드 일시 (DB)
        DateTime update_time;                               // 업데이트 일시 (DB)
    }

    // FOR DB 
    public class Arca_Content {
        int data_id             = -1;
        string content_name     = string.Empty;                 // 해당 아카콘의 글 이름
        string content_url      = string.Empty;                 // 해당 아카콘의 주소
        bool is_video           = false;                        // 이미지 인지
        bool is_convert         = false;                        // mp4 -> gif화 된건지
        string? convert_url     = string.Empty;                 // gif 변환된 주소
    }
}