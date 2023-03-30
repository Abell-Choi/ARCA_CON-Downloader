using System;
namespace Arcacon_Parser
{
	public class Nextcloud_Manager {
		string cloud_url = string.Empty;				// URL
		string cloud_folder_path = string.Empty;		// MAIN PATH

		string id = string.Empty;						// ID
		string pw = string.Empty;                       // PW


		private bool _login(string user_id, string user_pw) {
			return true;
		}
		public void mkdir(string path = "/arcacon_temp") { }
		public void download_file(string file, string download_path = "./") { }
		public void update_file(string filepath) {
			filepath = cloud_folder_path + filepath;
			filepath = filepath.Contains("//") ? filepath.Replace("//", "/") : filepath;
		}
		public void delete_file(string file_path) { }
		public void is_exist_file(string file_path) { }
		public void is_exist_dir(string path) { }

	}
}

