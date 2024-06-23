using closirissystem.Models;
using File = closirissystem.Models.File;

namespace closirissystem
{
    public class Singleton
    {
        private static Singleton? _instance;
        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }
        }

        public string? Token { get; set; } 
        public UserEdit? InfoUser {get; set; }

        public List<File> filesFolder {get; set;}

        public string? FileName {get; set;}

        public int? IdFileUpload {get; set;}

        public string? Preview {get; set;}
    }
}
