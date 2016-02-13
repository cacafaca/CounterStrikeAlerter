using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.ViewModel
{
    public class Response
    {
        public Response(byte[] response)
        {
            Value = response;
            Position = 0;
        }

        private byte[] Value;
        private int Position;

        public void SkipHeader()
        {
            if (Position == 0)
                Position = 4;
        }

        public byte GetNextByte()
        {
            if (Value != null)
            {
                if (Value.Length > Position)
                {
                    SkipHeader();
                    return Value[Position++];
                }
                throw new Exception("Array overflow.");
            }
            throw new ArgumentNullException(nameof(Value));
        }

        public string GetNextString()
        {
            SkipHeader();
            if (Value != null && Value.Length > Position)
            {
                int endPos = Array.IndexOf(Value, (byte)0x0, Position);
                if (endPos != -1)
                {
                    int length = endPos - Position;
                    byte[] result = new byte[length];
                    Array.Copy(Value, Position, result, 0, length);
                    Position = endPos + 1;
                    return Encoding.Default.GetString(result);
                }
                else
                    return string.Empty;
            }
            throw new Exception("Array overflow.");
        }

        public short GetNextShort()
        {
            return BitConverter.ToInt16(new byte[2] { Value[Position++], Value[Position++] }, 0);
        }

        public int GetNextInt()
        {
            return BitConverter.ToInt32(new byte[sizeof(int)] { Value[Position++], Value[Position++], Value[Position++], Value[Position++] }, 0);
        }

        public float GetNextFloat()
        {
            float value;
            int size = sizeof(System.Single);
            byte[] b = new byte[size];
            Array.Copy(Value, Position, b, 0, size);
            Position += size;
            value = BitConverter.ToSingle(b, 0);
            return value;
        }

        public byte Current
        {
            get
            {
                if (Position < Value.Length)
                    return Value[Position];
                else
                    throw new ArgumentOutOfRangeException("Position");

            }
        }

        public byte[] GetChallenge()
        {
            if (Value != null)
            {
                const int challegeSize = 4;
                byte[] result = new byte[challegeSize];
                Array.Copy(Value, 5, result, 0, challegeSize);
                return result;
            }
            else
                return null;
        }

        public override string ToString()
        {
            if (Value != null)
                return CSA.Common.Util.ByteArrayToString(Value);
            else
                return string.Empty;
        }
    }
}
