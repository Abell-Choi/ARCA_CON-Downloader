using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcacon_Parser;


string user_name = Environment.GetEnvironmentVariable( "USER" );
string home_path = Path.Combine( "/Users", user_name );
string json_file = File.ReadAllText( home_path + "/test.json" );

JObject jobject = JObject.Parse( json_file );


var _ARCA = new Arcacon_Manager( );

int url_post_num = 31415;
var _post_data = _ARCA._get_post_data(url_post_num);
Arca_Content_Jar _title = _post_data["INFO"];
List<Arca_Content> _contents = _post_data["CONTENTS"];

/// download & upload file
foreach(Arca_Content _content in _contents ) {
    _ARCA._download_file( _content );
}

var NEXT_CLOUD_MANAGER = new Nextcloud_Manager(
    jobject.Value<string>( "CLOUD_HOST" ),
    jobject.Value<string>( "SHARE_PATH" ),
    jobject.Value<string>( "ID" ),
    jobject.Value<string>( "PW" )
);

foreach ( var i in new DirectoryInfo( "./download" ).GetDirectories( ) ) {
    string name = i.Name;
    NEXT_CLOUD_MANAGER.create_directory( name );
    foreach ( var j in i.GetFiles( ) ) {
        Console.WriteLine(i.FullName);
        NEXT_CLOUD_MANAGER._upload_file( name, j.Name, j.FullName );
    }
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
