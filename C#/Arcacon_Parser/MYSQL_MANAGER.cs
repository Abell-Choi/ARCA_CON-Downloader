using System;


namespace Arcacon_Parser {
	public class MYSQL_MANAGER {
        string _DB_HOST;
        int _DB_PORT = 3306;
        string _DB_NAME;
        string _ID;
        string _PW;

        public MYSQL_MANAGER ( string HOST, int PORT, string DB, string ID, string PW ){
            this._DB_HOST = HOST;
            this._DB_PORT = PORT;
            this._DB_NAME = DB;
            this._ID = ID;
            this._PW = PW;
		}
	}
}