using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcacon_Parser;

string user_name = Environment.GetEnvironmentVariable ( "USER" );
string home_path = Path.Combine ( "/Users", user_name );
string json_file = File.ReadAllText ( home_path + "/test.json" );

var _ARCA = new Arcacon_Manager ( );
//_ARCA._test ( );
var dt = _ARCA._get_post_data ( 31162 );
Console.WriteLine ( dt["INFO"].ToString ( ) );
foreach(var i in dt["CONTENTS"] ) {
    Console.WriteLine ( i.ToString ( ) );
}


string title_code = ((Arca_Content_Jar)dt["INFO"]).code.ToString();
// save data
foreach (Arca_Content i in dt["CONTENTS"] ) {
    _ARCA._download_file (i.content_url, title_code, i.content_id.ToString(), i.isVideo);
}


//JObject j = JObject.Parse ( json_file );
//var _db = new MYSQL_MANAGER (
//    j.Value<string> ( "HOST" ),
//    j.Value<int> ( "PORT" ),
//    j.Value<string> ( "DB" ),
//    j.Value<string> ( "ID" ),
//    j.Value<string> ( "PW" )
//);

Console.Read ( );
