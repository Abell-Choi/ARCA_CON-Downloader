using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcacon_Parser;
using System.Threading;

string user_name = Environment.GetEnvironmentVariable( "USER" );
string home_path = Path.Combine( "/Users", user_name );
string json_file = File.ReadAllText( home_path + "/test.json" );

JObject jobject = JObject.Parse( json_file );


var _ARCA = new Arcacon_Manager( );

Console.Write("INPUT DOWNLOAD POST URL >>>>> ");
string _read = Console.ReadLine();
if (_read.Contains("?")){_read = _read.Split("?")[0];}

int url_post_num = int.Parse(_read.Split("/").Last());
var _post_data = _ARCA._get_post_data(url_post_num);
Arca_Content_Jar _title = _post_data["INFO"];
List<Arca_Content> _contents = _post_data["CONTENTS"];

///// download & upload file
//foreach(Arca_Content _content in _contents ) {
//    _ARCA._download_file( _content );
//}


Nextcloud_Manager _get_connection ( ) {
    var NEXT_CLOUD_MANAGER = new Nextcloud_Manager(
        jobject.Value<string>( "CLOUD_HOST" ),
        jobject.Value<string>( "SHARE_PATH" ),
        jobject.Value<string>( "ID" ),
        jobject.Value<string>( "PW" )
    );

    return NEXT_CLOUD_MANAGER;

}

List<Thread> _t_list = new ();
foreach ( var i in new DirectoryInfo( "./download" ).GetDirectories( ) ) {
    string name = i.Name;
    _get_connection().create_directory( name );
    int stack = 0;
    foreach ( var j in i.GetFiles( ) ) {
        while ( _t_list.Count >= 10 ) {
            List<Thread> _t_refreshed = new();
            foreach(var _ind_th in _t_list ) {
                if (_ind_th.IsAlive){_t_refreshed.Add(_ind_th);}
            }

            _t_list = _t_refreshed;
        }
        Thread _t = new Thread(new ThreadStart(delegate(){
            _get_connection( )._upload_file( name, j.Name, j.FullName );
        } ));
        _t_list.Add(_t);
        _t.Start();
    }
}

while (_t_list.Count != 0 ) {
    List<Thread> _t_refreshed = new( );
    foreach ( var _ind_th in _t_list ) {
        if ( _ind_th.IsAlive ) {
            _t_refreshed.Add( _ind_th );
        }
    }
    _t_list = _t_refreshed;
}

// update db
var _db = new MYSQL_MANAGER(
    jobject.Value<string>( "HOST" ),
    jobject.Value<int>( "PORT" ),
    jobject.Value<string>( "DB" ),
    jobject.Value<string>( "ID" ),
    jobject.Value<string>( "PW" )
);

_db.add_content_title(_title);
foreach(string i in _title.tags ) {
    _db.add_content_tag_tb(i);
}

foreach(Arca_Content _content in _contents ) {
    _db.add_arca_content_tb(_content);
}

Console.Read( );
