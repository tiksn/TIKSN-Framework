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

            CreateChunks(ref charArrayRepresentation, ref byteArrayRepresentation);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            if (byteArrayRepresentation is null)
            {
                throw new ArgumentNullException(nameof(byteArrayRepresentation));
            }

            char[] charArrayRepresentation = CreateCharsArray();

            CreateChunks(ref charArrayRepresentation, ref byteArrayRepresentation);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            char[] charArrayRepresentation = CreateCharsArray();
            byte[] byteArrayRepresentation = CreateByteArray();

            var chars = charArrayRepresentation.AsSpan();
            var bytes = byteArrayRepresentation.AsSpan();

            chars[0] = 'c';

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

            WriteBase36(timestamp, chars.Slice(1, 8), bytes.Slice(0, 6));
            WriteBase36(counter, chars.Slice(1 + 8, 4), bytes.Slice(6, 3));
            WriteBase36(pid, chars.Slice(1 + 8 + 4, 2), bytes.Slice(6 + 3, 2));
            WriteBase36(hostnameHash, chars.Slice(1 + 8 + 4 + 2, 2), bytes.Slice(6 + 3 + 2, 2));
            WriteBase36(randomNumber1, chars.Slice(1 + 8 + 4 + 2 + 2, 4), bytes.Slice(6 + 3 + 2 + 2, 4));
            WriteBase36(randomNumber2, chars.Slice(1 + 8 + 4 + 2 + 2 + 4, 4), bytes.Slice(6 + 3 + 2 + 2 + 4, 4));

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        private byte[] CreateByteArray()
        {
            return new byte[ByteArraySize];
        }

        private char[] CreateCharsArray()
        {
            return new char[CharsArraySize];
        }

        private void CreateChunks(
            ref char[] charArrayRepresentation,
            ref byte[] byteArrayRepresentation)
        {
            if (charArrayRepresentation is null)
            {
                charArrayRepresentation = CreateCharsArray();
            }

            if (byteArrayRepresentation is null)
            {
                byteArrayRepresentation = CreateByteArray();
            }

            var chars = charArrayRepresentation.AsSpan();
            var bytes = byteArrayRepresentation.AsSpan();
        }

        private string GetHostname()
        {
            const int padding = 2;

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