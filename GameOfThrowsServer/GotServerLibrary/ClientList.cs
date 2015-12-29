using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GotServerLibrary
{
    public class ClientList : List<Client>
    {
        public bool ContainsClient(EndPoint endPoint)
        {
            return GetClient(endPoint) != null;
        }

        public Client GetClient(EndPoint endPoint)
        {
            return this.FirstOrDefault(c => c.endPoint.ToString() == endPoint.ToString());
        }
    }
}
