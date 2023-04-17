using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcacon_Parser;

string user_name = Environment.GetEnvironmentVariable ( "USER" );
string home_path = Path.Combine ( "/Users", user_name );
string json_file = File.ReadAllText ( home_path + "/test.json" );

JObject j = JObject.Parse ( json_file );
var _db = new MYSQL_MANAGER (
    j.Value<string> ( "HOST" ),
    j.Value<int> ( "PORT" ),
    j.Value<string> ( "DB" ),
    j.Value<string> ( "ID" ),
    j.Value<string> ( "PW" )
);

Console.WriteLine ( JsonConvert.SerializeObject( _db.get_content_id_in_title_id ( -1 ), Formatting.Indented) );
Console.WriteLine ( _db.get_content_info ( -1 ).ToString ( ) );
Console.WriteLine ( "\n" );
foreach (var i in _db.get_content_info ( "_TEST" ) ) {
    Console.WriteLine ( i.ToString ( ) );
}
Console.Read ( );


var jars = new Arca_Content_Jar ( 0, "TEST00", "TEST", "TEST", 100, new List<string> ( ) { "ajsdklfjas","test","TTT" }, DateTime.Now );
Console.WriteLine ( _db.add_content_title ( jars ) );
Console.Read ( );

Console.WriteLine ( _db.del_content_title ( 0 ) );
