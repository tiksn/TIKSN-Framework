using System;
using System.Collections.Generic;
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
        private const int DuetteUpperBoundary = 36 * 36;
        private const int QuartetteUpperBoundary = 36 * 36 * 36 * 36;
        private const int Radix = 36;
        private static readonly IReadOnlyDictionary<char, int> CodeMap;
        private readonly object _locker;
        private readonly Random _random;
        private readonly ITimeProvider _timeProvider;
        private int _counter;
        private string _hostname;

        static CuidCorrelationService()
        {
            var codeMap = new Dictionary<char, int>();

            _ = "0123456789"
                .ToCharArray()
                .Do(x => codeMap.Add(x, x - '0'))
                .ToArray();

            _ = "abcdefghijklmnopqrstuvwxyz"
                .ToCharArray()
                .Do(x => codeMap.Add(x, x - 'a' + 10))
                .ToArray();

            _ = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                .ToCharArray()
                .Do(x => codeMap.Add(x, x - 'A' + 10))
                .ToArray();

            CodeMap = codeMap;
        }

        public CuidCorrelationService(ITimeProvider timeProvider, Random random)
        {
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this._counter = 0;
            this._locker = new object();
            this._random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public CorrelationID Create(string stringRepresentation)
        {
            if (stringRepresentation is null)
            {
                throw new ArgumentNullException(nameof(stringRepresentation));
            }

            var charArrayRepresentation = stringRepresentation.ToCharArray();
            var byteArrayRepresentation = CreateByteArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out var timestampChars,
                out var timestampBytes,
                out var counterChars,
                out var counterBytes,
                out var pidChars,
                out var pidBytes,
                out var hostnameChars,
                out var hostnameBytes,
                out var randomNumber1Chars,
                out var randomNumber1Bytes,
                out var randomNumber2Chars,
                out var randomNumber2Bytes);

            ConvertCharsToBytes(timestampChars, timestampBytes);
            ConvertCharsToBytes(counterChars, counterBytes);
            ConvertCharsToBytes(pidChars, pidBytes);
            ConvertCharsToBytes(hostnameChars, hostnameBytes);
            ConvertCharsToBytes(randomNumber1Chars, randomNumber1Bytes);
            ConvertCharsToBytes(randomNumber2Chars, randomNumber2Bytes);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Create(byte[] byteArrayRepresentation)
        {
            if (byteArrayRepresentation is null)
            {
                throw new ArgumentNullException(nameof(byteArrayRepresentation));
            }

            var charArrayRepresentation = CreateCharsArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out var timestampChars,
                out var timestampBytes,
                out var counterChars,
                out var counterBytes,
                out var pidChars,
                out var pidBytes,
                out var hostnameChars,
                out var hostnameBytes,
                out var randomNumber1Chars,
                out var randomNumber1Bytes,
                out var randomNumber2Chars,
                out var randomNumber2Bytes);

            ConvertBytesToChars(timestampChars, timestampBytes);
            ConvertBytesToChars(counterChars, counterBytes);
            ConvertBytesToChars(pidChars, pidBytes);
            ConvertBytesToChars(hostnameChars, hostnameBytes);
            ConvertBytesToChars(randomNumber1Chars, randomNumber1Bytes);
            ConvertBytesToChars(randomNumber2Chars, randomNumber2Bytes);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        public CorrelationID Generate()
        {
            var charArrayRepresentation = CreateCharsArray();
            var byteArrayRepresentation = CreateByteArray();

            CreateChunks(
                ref charArrayRepresentation,
                ref byteArrayRepresentation,
                out var timestampChars,
                out var timestampBytes,
                out var counterChars,
                out var counterBytes,
                out var pidChars,
                out var pidBytes,
                out var hostnameChars,
                out var hostnameBytes,
                out var randomNumber1Chars,
                out var randomNumber1Bytes,
                out var randomNumber2Chars,
                out var randomNumber2Bytes);

            var chars = charArrayRepresentation.AsSpan();
            var bytes = byteArrayRepresentation.AsSpan();

            var timestamp = this._timeProvider.GetCurrentTime().ToUnixTimeMilliseconds();

            int counter;

            lock (this._locker)
            {
                counter = this._counter;
                this._counter = this._counter == QuartetteUpperBoundary ? 0 : this._counter + 1;
            }

            var pid = Process.GetCurrentProcess().Id % DuetteUpperBoundary;
            var hostname = this.GetHostname();
            var hostnameHash = hostname.Split().Aggregate(hostname.Length + 36, (prev, c) => prev + c[0]) %
                               DuetteUpperBoundary;
            var randomNumber1 = this._random.Next() % QuartetteUpperBoundary;
            var randomNumber2 = this._random.Next() % QuartetteUpperBoundary;

            WriteBase36(timestamp, timestampChars, timestampBytes);
            WriteBase36(counter, counterChars, counterBytes);
            WriteBase36(pid, pidChars, pidBytes);
            WriteBase36(hostnameHash, hostnameChars, hostnameBytes);
            WriteBase36(randomNumber1, randomNumber1Chars, randomNumber1Bytes);
            WriteBase36(randomNumber2, randomNumber2Chars, randomNumber2Bytes);

            return new CorrelationID(new string(charArrayRepresentation), byteArrayRepresentation);
        }

        private static void WriteBase36(long value, Span<char> chars)
        {
            for (var i = chars.Length - 1; i >= 0; i--)
            {
                chars[i] = Alphabet[(int)(value % Radix)];
                value /= Radix;
            }
        }

        private static void WriteBase36(long value, Span<byte> bytes)
        {
            var valueBytes = BitConverter.GetBytes(value).AsSpan();
            if (BitConverter.IsLittleEndian)
            {
                valueBytes.Reverse();
            }

            valueBytes.Slice(valueBytes.Length - bytes.Length).CopyTo(bytes);
        }

        private static void WriteBase36(long value, Span<char> chars, Span<byte> bytes)
        {
            WriteBase36(value, bytes);
            WriteBase36(value, chars);
        }

        private static void ConvertBytesToChars(Span<char> chars, Span<byte> bytes)
        {
            var value = 0L;

            for (var i = 0; i < bytes.Length; i++)
            {
                value *= 256;
                value += bytes[i];
            }

            WriteBase36(value, chars);
        }

        private static void ConvertCharsToBytes(Span<char> chars, Span<byte> bytes)
        {
            var value = 0L;

            for (var i = 0; i < chars.Length; i++)
            {
                var code = CodeMap[chars[i]];
                value *= Radix;
                value += code;
            }

            WriteBase36(value, bytes);
        }

        private static byte[] CreateByteArray() => new byte[ByteArraySize];

        private static char[] CreateCharsArray()
        {
            var chars = new char[CharsArraySize];
            chars[0] = 'c';
            return chars;
        }

        private static void CreateChunks(
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
            out Span<byte> randomNumber1Bytes,
            out Span<char> randomNumber2Chars,
            out Span<byte> randomNumber2Bytes)
        {
            if (charArrayRepresentation is null)
            {
                charArrayRepresentation = CreateCharsArray();
            }
            else
            {
                if (charArrayRepresentation.Length != CharsArraySize)
                {
                    throw new ArgumentOutOfRangeException(nameof(charArrayRepresentation),
                        "CUID string representation contains invalid number of characters.");
                }

                if (charArrayRepresentation[0] != 'c')
                {
                    throw new ArgumentException("CUID string representation should start with 'c' character.",
                        nameof(charArrayRepresentation));
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
                    throw new ArgumentOutOfRangeException(nameof(byteArrayRepresentation),
                        "CUID byte array representation contains invalid number of characters.");
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

            randomNumber2Chars = chars.Slice(1 + 8 + 4 + 2 + 2 + 4, 4);
            randomNumber2Bytes = bytes.Slice(6 + 3 + 2 + 2 + 4, 4);
        }

        private string GetHostname()
        {
            if (this._hostname == null)
            {
                lock (this._locker)
                {
                    if (this._hostname == null)
                    {
                        try
                        {
                            this._hostname = Environment.MachineName;
                        }
                        catch
                        {
                            this._hostname = this._random.Next().ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            return this._hostname;
        }
    }
}
