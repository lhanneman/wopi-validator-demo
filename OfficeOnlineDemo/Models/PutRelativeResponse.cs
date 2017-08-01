namespace OfficeOnlineDemo.Models
{
    public class PutRelativeResponse
    {
        public string Name { get; set; } // name of file, with extension, without a path
        public string Url { get; set;  } // string uri of the file so like site.com/wopi/files/{file_id}?access_token={access_token}

        // optional:
        public string HostViewUrl { get; set; } //https://wopirest.readthedocs.io/en/latest/files/CheckFileInfo.html#term-hostviewurl
        public string HostEditUrl { get; set; } //https://wopirest.readthedocs.io/en/latest/files/CheckFileInfo.html#term-hostediturl

    }
}