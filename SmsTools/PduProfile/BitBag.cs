using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    internal class BitBag
    {
        private ulong[] _segment = new ulong[18];
        private int _bitIndex = 0;
        private int _segmentIndex = 0;
        private int _segmentPosition = 0;
        private int _left = 0;

        internal BitBag() { }

        internal BitBag(byte[] packed)
        {
            int tail = packed.Length % 8 > 0 ? 8 - (packed.Length % 8) : 0;

            var bytes = new List<byte>(packed.Length + tail);
            bytes.AddRange(packed);
            bytes.AddRange(new byte[tail]);
            var bytesarr = bytes.ToArray();

            for (int b = 0; b < bytesarr.Length >> 3; b++)
            {
                _segment[b] = BitConverter.ToUInt64(bytesarr, b << 3);
            }

            _bitIndex = (int)((packed.Length << 3) / 7) * 7;
            index();
        }

        internal void Pack(byte septet)
        {
            _segment[_segmentIndex] |= (ulong)septet << _segmentPosition;

            if (_left > 0)
            {
                _segment[_segmentIndex + 1] |= (ulong)septet >> (7 - _left);
            }

            _bitIndex += 7;
            index();
        }

        internal byte[] Unpack()
        {
            var unpacked = new byte[_bitIndex / 7];

            int bitIndex = 0;
            int segmentIndex = 0;
            int segmentPosition = 0;
            int left = 0;
            int u = 0;

            while (bitIndex < _bitIndex)
            {
                unpacked[u] = (byte)((_segment[segmentIndex] >> segmentPosition) & 0x7f);

                if (left > 0)
                {
                    unpacked[u] |= (byte)((_segment[segmentIndex + 1] & (0x7fuL >> (7 - left))) << (7 - left));
                }

                ++u;
                bitIndex += 7;
                segmentIndex = bitIndex >> 6;
                segmentPosition = bitIndex % 64;
                left = (bitIndex + 7) - (64 * (segmentIndex + 1));
            }

            return unpacked;
        }

        internal byte[] ToOctets()
        {
            int bytecount = (_bitIndex >> 3) + (_bitIndex % 8 > 0 ? 1 : 0);
            var bytes = new byte[bytecount];

            for (int b = 0; b < bytecount; b++)
            {
                int segment = b >> 3;
                int segposition = (b % 8) << 3;

                bytes[b] = (byte)((_segment[segment] & (0xffuL << segposition)) >> segposition);
            }

            return bytes;
        }

        internal void Clear()
        {
            for (int s = 0; s < _segment.Length; _segment[s++] = 0uL) { }

            _bitIndex = _segmentIndex = _segmentPosition = _left = 0;
        }

        private void index()
        {
            _segmentIndex = _bitIndex >> 6;
            _segmentPosition = _bitIndex % 64;
            _left = (_bitIndex + 7) - (64 * (_segmentIndex + 1));
        }
    }
}
