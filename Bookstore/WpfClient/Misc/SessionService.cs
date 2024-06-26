using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Misc
{
    public class SessionService
    {
        private static SessionService instance;

        public Session Session { get; private set; }
        public string Token { get; set; }

        private SessionService() 
        {
            Session = new Session();
            Token = string.Empty;
        }

        public static SessionService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SessionService();
                }
                return instance;
            }
        }
    }
}
