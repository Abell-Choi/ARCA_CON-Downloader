using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySqlConnector;

namespace Arcacon_Parser {
    public class MYSQL_MANAGER {
        MySqlConnection _conn = null;
        string _DB_HOST;
        int _DB_PORT = 3306;
        string _DB_NAME;
        string _ID;
        string _PW;

        public MYSQL_MANAGER ( string HOST, int PORT, string DB, string ID, string PW ) {
            this._DB_HOST = HOST;
            this._DB_PORT = PORT;
            this._DB_NAME = DB;
            this._ID = ID;
            this._PW = PW;

            Console.WriteLine ( HOST );
            Console.WriteLine ( PORT.ToString ( ) );
            Console.WriteLine ( DB );
            Console.WriteLine ( ID );
            Console.WriteLine ( PW );


            string code = $"server={HOST};port={PORT};database={DB};uid={ID};password={PW}";
            this._conn = new MySqlConnection ( code );
            _conn.Open ( );
            _conn.Close ( );
        }



        // ARCA_CONTENT_TB
        // CONTENT_TITLE, CONTENT_ID_PK, CONTENT_URL, IS_VIDEO

        /// <summary> 타이틀 명으로 아카콘 아이디 리스트 싹 긁어오기 </summary>
        public List<string> get_content_ids_in_title ( string title ) {
            string SQL = "SELECT CONTENT_ID_PK FROM ARCA_CONTENT_TB WHERE CONTENT_TITLE = @TITLE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TITLE", title );
            var _reader = _cmd.ExecuteReader ( );

            List<string> _CONTENT_IDs = new ( );
            while ( _reader.Read ( ) ) {
                _CONTENT_IDs.Add ( (string) _reader["CONTENT_ID_PK"] );
            }
            _conn.Close ( );
            return _CONTENT_IDs;
        }




        // CONTENT_TAGS_TB
        // TAG_CODE_PK, TAG_NAME
        public List<string> get_tag_lists ( ) {
            string SQL = "SELECT TAG_NAME FROM CONTENT_TAGS_TB";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            var _reader = _cmd.ExecuteReader ( );

            List<string> _TAGs = new ( );
            while ( _reader.Read ( ) ) {
                _TAGs.Add ( (string)_reader["TAG_NAME"] );
            }
            _conn.Close ( );
            return _TAGs;
        }

        public int get_tag_code ( string tag ) {
            string SQL = "SELECT TAG_CODE_PK FROM CONTENT_TAGS_TB WHERE TAG_NAME = @TAG_NAME";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TAG_NAME", tag );
            var _reader = _cmd.ExecuteReader ( );
            if ( !_reader.Read ( ) ) { _conn.Close ( ); return -1; }

            int _result_code = (int) _reader["TAG_CODE_PK"];
            _conn.Close ( );
            return _result_code;
        }

        /// <summary> code -> 태그 </summary>
        public string get_tag_code_to_name(int tag_code ) {
            string SQL = "SELECT TAG_NAME FROM CONTENT_TAGS_TB WHERE TAG_CODE_PK=@TAG_CODE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TAG_CODE", tag_code );
            var _reader = _cmd.ExecuteReader ( );
            if ( !_reader.Read ( ) ) { _conn.Close ( ); return ""; }

            string _result_tag = ( string ) _reader["TAG_NAME"];
            _conn.Close ( );
            return _result_tag;
        }


        // CONTENT_TITLE_TB
        // CONTENT_TITLE_PK, POST_URL, UPLOAD_USER, SELL_COUNT, TAG_LISTS, UPLOAD_TIME, UPDATE_TIME
        ///<summary> 저장되어있는 아카콘 타이틀 가져오기 </summary>
        public List<string> get_title_lists ( ) {
            string SQL = "SELECT TITLE_PK FROM CONTENT_TITLE_TB";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            var _reader = _cmd.ExecuteReader ( );

            List<string> _TITLE_PKs = new ( );
            while ( _reader.Read ( ) ) {
                _TITLE_PKs.Add ( ( string ) _reader["TITLE_PK"] );
            }
            _conn.Close ( );
            return _TITLE_PKs;
        }

        /// <summary> 아카콘의 정보를 가져옵니다. </summary>
        public Arca_Content_Jar get_content_info ( string TITLE ) {
            string SQL = "SElECT * FROM CONTENT_TITLE_TB WHERE TITLE_PK=@TITLE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TITLE", TITLE );
            var _reader = _cmd.ExecuteReader ( );

            if ( !_reader.Read() ) { _conn.Close ( ); return null; }

            Arca_Content_Jar _jar = new (
                ( string ) _reader["TITLE_PK"],
                ( string ) _reader["POST_URL"],
                ( string ) _reader["UPLOAD_USER"],
                ( int ) _reader["SELL_COUNT"],
                JArray.Parse ( ( string ) _reader["TAG_LISTS"] ).ToObject<List<string>> ( ),
                ( DateTime ) _reader["UPLOAD_TIME"]) ;
            _conn.Close ( );
            return _jar;
        }
    }
}
