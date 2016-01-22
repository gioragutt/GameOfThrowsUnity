using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GotServerLibrary
{
    public class ClientList : List<ConnectedClient>
    {
        public bool ContainsClient(EndPoint endPoint)
        {
            return GetClient(endPoint) != null;
        }

        public ConnectedClient GetClient(EndPoint endPoint)
        {
            return this.FirstOrDefault(c => c.ClientEndpoint.ToString() == endPoint.ToString());
        }

        public void DisconnectAtIndex(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index of player must be within range of the list");

            this[index].KickPlayer();
        }

        public void DisconnectAll()
        {
            int initialCount = Count;
            for (int i = 0; i < initialCount; i++)
            {
                this.First().KickPlayer();
            }
        }
    }
}
