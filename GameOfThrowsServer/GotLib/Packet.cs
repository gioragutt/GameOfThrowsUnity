using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace GotLib
{
    // ----------------
    // Packet Structure
    // ----------------

    // Description   -> | dataIdentifier | name length | message length |     name    |     message    |
    // Size in bytes -> |        4       |      4      |        4       | name length | message length |

    public enum DataIdentifier
    {
        Message,
        LogIn,
        LogOut,
        Null,
        Error
    }

    public class Packet
    {
        #region Public Members

        public DataIdentifier DataIdentifier { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        #endregion

        #region Constructors

        public Packet()
        {
            DataIdentifier = DataIdentifier.Null;
            Message = null;
            Name = null;
        }

        public static Packet PacketFromBytes(byte[] dataStream)
        {
            const int DATA_IDENTIFIER_INDEX = 0;
            const int NAME_LENGTH_INDEX = DATA_IDENTIFIER_INDEX + sizeof(int);
            const int MESSAGE_LENGTH_INDEX = NAME_LENGTH_INDEX + sizeof(int);

            int nameLength = BitConverter.ToInt32(dataStream, NAME_LENGTH_INDEX);
            int msgLength = BitConverter.ToInt32(dataStream, MESSAGE_LENGTH_INDEX);

            return new Packet
            {
                DataIdentifier = (DataIdentifier)BitConverter.ToInt32(dataStream, DATA_IDENTIFIER_INDEX),
                Name = nameLength > 0 ? Encoding.UTF8.GetString(dataStream, 12, nameLength) : null,
                Message = msgLength > 0 ? Encoding.UTF8.GetString(dataStream, 12 + nameLength, msgLength) : null
            };
        }

        #endregion

        #region Other Methods

        // Converts the packet into a byte array for sending/receiving 
        public byte[] ToByteArray()
        {
            var dataStream = new List<byte>();

            // Add the dataIdentifier
            dataStream.AddRange(BitConverter.GetBytes((int)DataIdentifier));

            // Add the name length
            dataStream.AddRange(BitConverter.GetBytes(Name?.Length ?? 0));

            // Add the message length
            dataStream.AddRange(BitConverter.GetBytes(Message?.Length ?? 0));

            // Add the name
            if (Name != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(Name));

            // Add the message
            if (Message != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(Message));

            return dataStream.ToArray();
        }

        #endregion

        #region Operator Overloading Methods

        public static bool operator ==(Packet one, Packet two)
        {
            if (Equals(one, null) != Equals(two, null))
                return false;
            if (Equals(one, null))
                return true;

            return one.DataIdentifier == two.DataIdentifier &&
                   one.Name == two.Name &&
                   one.Message == two.Message;
        }

        public static bool operator !=(Packet one, Packet two)
        {
            return !(one == two);
        }

        #endregion

        #region Generated

        protected bool Equals(Packet other)
        {
            return DataIdentifier == other.DataIdentifier && string.Equals(Name, other.Name) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Packet)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)DataIdentifier;
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Message?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion
    }
}
