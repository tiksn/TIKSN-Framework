using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TIKSN.Time;

namespace TIKSN.Integration.Correlation
{
    public class CuidCorrelationService : ICorrelationService
    {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
        private const int ByteArraySize = 6 + 3 + 2 + 2 + 4 + 4;
        private const int CharsArraySize = 1 + 8 + 4 + 2 + 2 + 4 + 4;
        private const int QuartetteUpperBoundary = 36 * 36 * 36 * 36;
        private const int Radix = 36;

        private readonly object _locker;
        private readonly Random _random;
        private readonly ITimeProvider _timeProvider;
        private int _counter;
        private string _hostname;

        public CuidCorrelationService(ITimeProvider timeProvider, Random random)
        {
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _counter = 0;
            _locker = new object();
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            if (stringRepresentation is null)
            {
                throw new ArgumentNullException(nameof(stringRepresentation));
            }

            char[] charArrayRepresentation = stringRepresentation.ToCharArray();
            byte[] byteArrayRepresentation = CreateByteArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out Span<char> timestampChars,
                out Span<byte> timestampBytes,
                out Span<char> counterChars,
                out Span<byte> counterBytes,
                out Span<char> pidChars,
                out Span<byte> pidBytes,
                out Span<char> hostnameChars,
                out Span<byte> hostnameBytes,
                out Span<char> randomNumber1Chars,
                out Span<byte> randomNumber1Bytes);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            if (byteArrayRepresentation is null)
            {
                throw new ArgumentNullException(nameof(byteArrayRepresentation));
            }

            char[] charArrayRepresentation = CreateCharsArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out Span<char> timestampChars,
                out Span<byte> timestampBytes,
                out Span<char> counterChars,
                out Span<byte> counterBytes,
                out Span<char> pidChars,
                out Span<byte> pidBytes,
                out Span<char> hostnameChars,
                out Span<byte> hostnameBytes,
                out Span<char> randomNumber1Chars,
                out Span<byte> randomNumber1Bytes);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            char[] charArrayRepresentation = CreateCharsArray();
            byte[] byteArrayRepresentation = CreateByteArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out Span<char> timestampChars,
                out Span<byte> timestampBytes,
                out Span<char> counterChars,
                out Span<byte> counterBytes,
                out Span<char> pidChars,
                out Span<byte> pidBytes,
                out Span<char> hostnameChars,
                out Span<byte> hostnameBytes,
                out Span<char> randomNumber1Chars,
                out Span<byte> randomNumber1Bytes);

            var chars = charArrayRepresentation.AsSpan();
            var bytes = byteArrayRepresentation.AsSpan();

            var timestamp = _timeProvider.GetCurrentTime().ToUnixTimeMilliseconds();

            int counter;

            lock (_locker)
            {
                counter = _counter;
                _counter = (_counter == QuartetteUpperBoundary) ? 0 : _counter + 1;
            }

            var pid = Process.GetCurrentProcess().Id;
            var hostname = GetHostname();
            var hostnameHash = hostname.Split().Aggregate(hostname.Length + 36, (prev, c) => prev + c[0]);
            var randomNumber1 = _random.Next() % QuartetteUpperBoundary;
            var randomNumber2 = _random.Next() % QuartetteUpperBoundary;

            WriteBase36(timestamp, timestampChars, timestampBytes);
            WriteBase36(counter, counterChars, counterBytes);
            WriteBase36(pid, pidChars, pidBytes);
            WriteBase36(hostnameHash, hostnameChars, hostnameBytes);
            WriteBase36(randomNumber1, randomNumber1Chars, randomNumber1Bytes);
            WriteBase36(randomNumber2, chars.Slice(1 + 8 + 4 + 2 + 2 + 4, 4), bytes.Slice(6 + 3 + 2 + 2 + 4, 4));

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        private byte[] CreateByteArray()
        {
            return new byte[ByteArraySize];
        }

        private char[] CreateCharsArray()
        {
            var chars = new char[CharsArraySize];
            chars[0] = 'c';
            return chars;
        }

        private void CreateChunks(
            ref char[] charArrayRepresentation,
            ref byte[] byteArrayRepresentation,
            out Span<char> timestampChars,
            out Span<byte> timestampBytes,
            out Span<char> counterChars,
            out Span<byte> counterBytes,
            out Span<char> pidChars,
            out Span<byte> pidBytes,
            out Span<char> hostnameChars,
            out Span<byte> hostnameBytes,
            out Span<char> randomNumber1Chars,
            out Span<byte> randomNumber1Bytes)
        {
            if (charArrayRepresentation is null)
            {
                charArrayRepresentation = CreateCharsArray();
            }
            else
            {
                if (charArrayRepresentation.Length != CharsArraySize)
                {
                    throw new ArgumentOutOfRangeException(nameof(charArrayRepresentation), "CUID string representation contains invalid number of characters.");
                }

                if (charArrayRepresentation[0] != 'c')
                {
                    throw new ArgumentException("CUID string representation should start with 'c' character.", nameof(charArrayRepresentation));
                }
            }

            if (byteArrayRepresentation is null)
            {
                byteArrayRepresentation = CreateByteArray();
            }
            else
            {
                if (byteArrayRepresentation.Length != ByteArraySize)
                {
                    throw new ArgumentOutOfRangeException(nameof(byteArrayRepresentation), "CUID byte array representation contains invalid number of characters.");
                }
            }

            var chars = charArrayRepresentation.AsSpan();
            var bytes = byteArrayRepresentation.AsSpan();

            timestampChars = chars.Slice(1, 8);
            timestampBytes = bytes.Slice(0, 6);

            counterChars = chars.Slice(1 + 8, 4);
            counterBytes = bytes.Slice(6, 3);

            pidChars = chars.Slice(1 + 8 + 4, 2);
            pidBytes = bytes.Slice(6 + 3, 2);

            hostnameChars = chars.Slice(1 + 8 + 4 + 2, 2);
            hostnameBytes = bytes.Slice(6 + 3 + 2, 2);

            randomNumber1Chars = chars.Slice(1 + 8 + 4 + 2 + 2, 4);
            randomNumber1Bytes = bytes.Slice(6 + 3 + 2 + 2, 4);
        }

        private string GetHostname()
        {
            if (_hostname == null)
            {
                lock (_locker)
                {
                    if (_hostname == null)
                    {
                        try
                        {
                            _hostname = Environment.MachineName;
                        }
                        catch
                        {
                            _hostname = _random.Next().ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            return _hostname;
        }

        private void WriteBase36(long value, Span<char> chars, Span<byte> bytes)
        {
            var valueBytes = BitConverter.GetBytes(value).AsSpan();
            if (BitConverter.IsLittleEndian)
            {
                valueBytes.Reverse();
            }

            valueBytes.Slice(valueBytes.Length - bytes.Length).CopyTo(bytes);

            for (int i = chars.Length - 1; i >= 0; i--)
            {
                chars[i] = Alphabet[(int)(value % Radix)];
                value /= Radix;
            }
        }
    }
}