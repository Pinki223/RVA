using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Misc
{
    public class Session
    {
        public IBookstoreService BookstoreService { get; private set; }

        public Session()
        {
            ChannelFactory<IBookstoreService> factory = new ChannelFactory<IBookstoreService>(new NetTcpBinding(), "net.tcp://localhost:9000");
            BookstoreService = factory.CreateChannel();
        }

    }
}
