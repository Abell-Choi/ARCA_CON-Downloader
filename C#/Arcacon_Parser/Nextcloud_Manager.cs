using System;
using System.IO;
using System.Net;

namespace Arcacon_Parser {
    public class Nextcloud_Manager {
        string cloud_url = "";
        string share_path = string.Empty;
        string user_name = string.Empty;
        string user_passwd = string.Empty;

        public Nextcloud_Manager ( string cloud_url, string share_path, string user_name, string user_passwd ) {
            this.cloud_url = cloud_url;
            this.share_path = share_path;
            this.user_name = user_name;
            this.user_passwd = user_passwd;
        }

        private bool _is_auth_enabled ( ) {

            return true;
        }

        public bool create_directory ( string dir_name ) {
            bool result = false;
            if ( !dir_name.StartsWith( "/" ) ) {
                dir_name = "/" + dir_name;
            }
            try {
                HttpWebRequest folderRequest = ( HttpWebRequest ) WebRequest.Create( cloud_url + share_path + dir_name );
                folderRequest.Method = "MKCOL";
                folderRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String( System.Text.Encoding.GetEncoding( "ISO-8859-1" ).GetBytes( username + ":" + user_passwd ) );
                folderRequest.Headers["Prefer"] = "return=minimal";
                folderRequest.ContentLength = 0;

                using ( HttpWebResponse folderResponse = ( HttpWebResponse ) folderRequest.GetResponse( ) ) {
                    if ( folderResponse.StatusCode == HttpStatusCode.Created ) {
                        result = true;
                    }
                }
            } catch ( Exception ex ) {
                Console.WriteLine( "EX >> " + ex.Message );
            }

            return result;
        }


        public bool _upload_file ( string dir_name, string file_name, string local_file_path ) {
            bool result = false;

            create_directory(dir_name);

            if (!dir_name.StartsWith("/")){dir_name = "/" +dir_name;}
            if (!dir_name.EndsWith("/")){dir_name += "/";}
            string f_url = this.cloud_url +this.share_path +file_name;
            string file_content_type = "";
            if ( file_name.EndsWith( ".mp4" ) ) {
                file_content_type = $"video/mp4";
            } else {
                file_content_type = $"image/{file_name.Split(".").Last()}"; 
            }
            try {
                string fileUrl = cloud_url +share_path +dir_name +file_name;
                Console.WriteLine(fileUrl);
                HttpWebRequest fileRequest = ( HttpWebRequest ) WebRequest.Create( fileUrl );
                fileRequest.Method = "PUT";
                fileRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String( System.Text.Encoding.GetEncoding( "ISO-8859-1" ).GetBytes( username + ":" + user_passwd ) );
                fileRequest.Headers["Prefer"] = "return=minimal";
                fileRequest.ContentType = file_content_type;

                using ( FileStream fileStream = File.OpenRead( local_file_path ) ) {
                    fileRequest.ContentLength = fileStream.Length;
                    using ( Stream requestStream = fileRequest.GetRequestStream( ) ) {
                        fileStream.CopyTo( requestStream );
                    }
                }

                using ( HttpWebResponse fileResponse = ( HttpWebResponse ) fileRequest.GetResponse( ) ) {
                    if ( fileResponse.StatusCode == HttpStatusCode.Created ) {
                        result = true;
                    }
                }
            } catch ( Exception ex ) {
                Console.WriteLine( ex.Message );
            }

            return result;
        }
    }
}

