using closirissystem.Models;

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
    }
}