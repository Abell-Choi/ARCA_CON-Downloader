using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arcacon_Parser;

string user_name = Environment.GetEnvironmentVariable( "USER" );
string home_path = Path.Combine( "/Users", user_name );
string json_file = File.ReadAllText( home_path + "/test.json" );

var _ARCA = new Arcacon_Manager( );
//_ARCA._test ( );
foreach(int i in new List<int>(){ 31607, 31608, 31407 } ) {
    Thread.Sleep(5000);
    var dt = _ARCA._get_post_data( i );
    Console.WriteLine( dt["INFO"].ToString( ) );
    foreach ( Arca_Content j in dt["CONTENTS"] ) {
        Console.WriteLine( i.ToString( ) );
        _ARCA._download_file( j );
    }
}

//JObject j = JObject.Parse ( json_file );
//var _db = new MYSQL_MANAGER (
//    j.Value<string> ( "HOST" ),
//    j.Value<int> ( "PORT" ),
//    j.Value<string> ( "DB" ),
//    j.Value<string> ( "ID" ),
//    j.Value<string> ( "PW" )
//);

Console.Read( );
