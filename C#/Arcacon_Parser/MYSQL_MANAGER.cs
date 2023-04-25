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
        // TITLE_CODE, CONTENT_ID_PK, CONTENT_URL, IS_VIDEO

        /// <summary> 타이틀 명으로 아카콘 아이디 리스트 싹 긁어오기 </summary>
        public List<int> get_content_id_in_title_id ( int title_code ) {
            string SQL = "SELECT CONTENT_ID_PK FROM ARCA_CONTENT_TB WHERE TITLE_CODE = @TITLE_CODE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TITLE_CODE", title_code );
            var _reader = _cmd.ExecuteReader ( );

            List<int> _CONTENT_IDs = new ( );
            while ( _reader.Read ( ) ) {
                _CONTENT_IDs.Add ( (int) _reader["CONTENT_ID_PK"] );
            }
            _conn.Close ( );
            return _CONTENT_IDs;
        }

        /// <summary> CONTENT_TB 추가(업데이트) </summary>
        public int add_arca_content_tb(Arca_Content _content ) {
            string SQL = "INSERT INTO `ARCA_CONTENT_TB` " +
                "(`TITLE_CODE`, `CONTENT_ID_PK`, `CONTENT_URL`, `IS_VIDEO`)" +
                "VALUES (@TITLE_CODE, @CONTENT_ID_PK, @CONTENT_URL, @IS_VIDEO) " +
                "ON DUPLICATE KEY UPDATE " +
                "`TITLE_CODE` = @TITLE_CODE," +
                "`CONTENT_ID_PK` = @CONTENT_ID_PK," +
                "`CONTENT_URL` = @CONTENT_URL," +
                "`IS_VIDEO` = @IS_VIDEO";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, _conn );
            _cmd.Parameters.AddWithValue ( "@TITLE_CODE", _content.content_post_id );
            _cmd.Parameters.AddWithValue ( "@CONTENT_ID_PK", _content.content_id );
            _cmd.Parameters.AddWithValue ( "@CONTENT_URL", _content.content_url );
            _cmd.Parameters.AddWithValue ( "@IS_VIDEO", _content.isVideo );

            int _res = -1;
            try { _res = _cmd.ExecuteNonQuery ( ); } catch { }
            _conn.Close ( );
            return _res;
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


        /// <summary> CONTENT_TAGS_TB 추가(업데이트) </summary>
        public int add_content_tag_tb( string TAG_NAME ) {
            if (this.get_tag_code(TAG_NAME) != -1 ) { return -1; }
            string SQL = "INSERT INTO `CONTENT_TAGS_TB`(`TAG_CODE_PK`, `TAG_NAME`) " +
                "VALUES (NULL, @TAG_NAME) ON DUPLICATE KEY UPDATE " +
                "`TAG_NAME` = @TAG_NAME";

            _conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, _conn );
            _cmd.Parameters.AddWithValue ( "@TAG_NAME", TAG_NAME );
            int _res = -1;
            try { _res = _cmd.ExecuteNonQuery ( ); } catch { }
           _conn.Close ( );

            return _res;
        }

        /// <summary> CONTENT_TAGS_TB 제거</summary>
        public int del_content_tag_tb(int TAG_CODE_PK ) {
            string SQL = "DELETE FROM `CONTENT_TAGS_TB` WHERE `CONTENT_TAGS_TB`.`TAG_CODE_PK` = @TAG_CODE_PK";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, _conn );
            _cmd.Parameters.AddWithValue ( SQL, TAG_CODE_PK );
            int _res = -1;
            try { _res = _cmd.ExecuteNonQuery ( ); } catch { }

            this._conn.Close ( );
            return _res;
        }


        // CONTENT_TITLE_TB
        // CONTENT_TITLE_PK, POST_URL, UPLOAD_USER, SELL_COUNT, TAG_LISTS, UPLOAD_TIME, UPDATE_TIME
        ///<summary> 저장되어있는 아카콘 타이틀 가져오기 </summary>
        public List<string> get_title_lists ( ) {
            string SQL = "SELECT CODE_PK FROM CONTENT_TITLE_TB";

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


        /// <summary> 타이틀 코드 -> 아카콘 정보 반환 </summary>
        public Arca_Content_Jar get_content_info ( int TITLE_CODE ) {
            string SQL = "SELECT * FROM CONTENT_TITLE_TB WHERE CODE_PK=@TITLE_CODE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TITLE_CODE", TITLE_CODE );
            var _reader = _cmd.ExecuteReader ( );


            if ( !_reader.Read ( ) ) { _conn.Close ( ); return null; }

            Arca_Content_Jar _jar = new (
                ( int ) _reader["CODE_PK"],
                ( string ) _reader["TITLE"],
                ( string ) _reader["POST_URL"],
                ( string ) _reader["UPLOAD_USER"],
                ( int ) _reader["SELL_COUNT"],
                JArray.Parse ( ( string ) _reader["TAG_LISTS"] ).ToObject<List<string>> ( ),
                ( DateTime ) _reader["UPLOAD_TIME"] );
            _conn.Close ( );
            return _jar;
        }

        /// <summary> 아아콘명 -> 아카콘 정보 (들). </summary>
        public List<Arca_Content_Jar> get_content_info ( string TITLE ) {
            string SQL = "SElECT * FROM CONTENT_TITLE_TB WHERE TITLE=@TITLE";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@TITLE", TITLE );
            var _reader = _cmd.ExecuteReader ( );

            List<Arca_Content_Jar> _jars = new ( );
            while ( _reader.Read ( ) ) {
                Arca_Content_Jar _jar = new (
                    ( int ) _reader["CODE_PK"],
                    ( string ) _reader["TITLE"],
                    ( string ) _reader["POST_URL"],
                    ( string ) _reader["UPLOAD_USER"],
                    ( int ) _reader["SELL_COUNT"],
                    JArray.Parse ( ( string ) _reader["TAG_LISTS"] ).ToObject<List<string>> ( ),
                    ( DateTime ) _reader["UPLOAD_TIME"] );
                _jars.Add ( _jar );
            }
            _conn.Close ( );
            return _jars;
        }
        /// <summary> CONTENT_TITLE_TB의 데이터를 추가 (업데이트) 합니다. </summary>
        public int add_content_title(Arca_Content_Jar _jar ) {
            string SQL = "INSERT INTO `CONTENT_TITLE_TB` " +
                "(`CODE_PK`, `TITLE`, `POST_URL`, `UPLOAD_USER`, `SELL_COUNT`, `TAG_LISTS`," +
                "`UPLOAD_TIME`, `UPDATE_TIME`) " +
                "VALUES (@CODE_PK, @TITLE, @POST_URL, @UPLOAD_USER," +
                "@SELL_COUNT, @TAG_LISTS, @UPLOAD_TIME, @UPDATE_TIME) " +
                "ON DUPLICATE KEY UPDATE " +
                "`CODE_PK` = @CODE_PK, `TITLE` = @TITLE, `POST_URL` = @POST_URL," +
                "`SELL_COUNT` = @SELL_COUNT, `TAG_LISTS` = @TAG_LISTS," +
                "`UPLOAD_TIME` = @UPLOAD_TIME, `UPDATE_TIME` = @UPDATE_TIME";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "@CODE_PK", _jar.code );
            _cmd.Parameters.AddWithValue ( "@TITLE", _jar.title );
            _cmd.Parameters.AddWithValue ( "@POST_URL", _jar.post_url );
            _cmd.Parameters.AddWithValue ( "@UPLOAD_USER", _jar.upload_user );
            _cmd.Parameters.AddWithValue ( "@SELL_COUNT", _jar.sell_count );
            _cmd.Parameters.AddWithValue ( "@TAG_LISTS", _jar._get_tag_lists() );
            _cmd.Parameters.AddWithValue ( "@UPLOAD_TIME", _jar.upload_time );
            _cmd.Parameters.AddWithValue ( "@UPDATE_TIME", _jar.update_time );
            int _res = -1;
            try {
                _res = _cmd.ExecuteNonQuery ( );
            } catch { }
            this._conn.Close ( );
            return _res;
        }


        /// <summary> CONTENT_TITLE_TB 제거 </summary>
        public int del_content_title(int CODE_PK ) {
            string SQL = "DELETE FROM `CONTENT_TITLE_TB` WHERE `CONTENT_TITLE_TB`.`CODE_PK` = @CODE_PK";

            this._conn.Open ( );
            var _cmd = new MySqlCommand ( SQL, this._conn );
            _cmd.Parameters.AddWithValue ( "CODE_PK", CODE_PK );

            int _res = -1;
            try { _res = _cmd.ExecuteNonQuery ( ); } catch { }
            this._conn.Close ( );
            return _res;
        }
    }
}
