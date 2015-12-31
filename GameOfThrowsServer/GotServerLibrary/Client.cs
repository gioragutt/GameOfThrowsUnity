using System;
using System.Net;
using GotLib;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace GotServerLibrary
{
    // Structure to store the client information
    public class Client
    {
        public EndPoint endPoint;
        public Guid id;
        public PlayerData data;

        public string PlayerDataToString()
        {
            return data.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", endPoint, data);
        }

        #region Generated

        private bool Equals(Client other)
        {
            return Equals(endPoint, other.endPoint) && id.Equals(other.id) && Equals(data, other.data);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = endPoint?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ id.GetHashCode();
                hashCode = (hashCode * 397) ^ (data?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Client)obj);
        }

        #endregion
    }
}
