using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Integration.Correlation;

public class CuidCorrelationService : ICorrelationService
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";
    private const int ByteArraySize = 6 + 3 + 2 + 2 + 4 + 4;
    private const int CharsArraySize = 1 + 8 + 4 + 2 + 2 + 4 + 4;
    private const string Digits = "0123456789";
    private const int DuetteUpperBoundary = 36 * 36;
    private const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
    private const int QuartetteUpperBoundary = 36 * 36 * 36 * 36;
    private const int Radix = 36;
    private const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly IReadOnlyDictionary<char, int> CodeMap = CreateCodeMap();
    private readonly object locker;
    private readonly Random random;
    private readonly TimeProvider timeProvider;
    private int counter;
    private string hostname;

    public CuidCorrelationService(TimeProvider timeProvider, Random random)
    {
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.counter = 0;
        this.locker = new object();
        this.random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public CorrelationId Create(string stringRepresentation)
    {
        ArgumentNullException.ThrowIfNull(stringRepresentation);

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

        var binaryRepresentation = Seq(byteArrayRepresentation);

        return new CorrelationId(new string(charArrayRepresentation), binaryRepresentation);
    }

    public CorrelationId Create(Seq<byte> binaryRepresentation)
    {
        var charArrayRepresentation = CreateCharsArray();
        var byteArrayRepresentation = binaryRepresentation.ToArray();

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

        return new CorrelationId(new string(charArrayRepresentation), binaryRepresentation);
    }

    public CorrelationId Generate()
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

        var timestamp = this.timeProvider.GetUtcNow().ToUnixTimeMilliseconds();

        int oldCounter;
        lock (this.locker)
        {
            oldCounter = this.counter;
            this.counter = this.counter == QuartetteUpperBoundary ? 0 : this.counter + 1;
        }

        var pid = Environment.ProcessId % DuetteUpperBoundary;
        var theHostname = this.GetHostname();
        var hostnameHash = theHostname.Split().Aggregate(theHostname.Length + 36, (prev, c) => prev + c[0]) %
                           DuetteUpperBoundary;
        var randomNumber1 = this.random.Next() % QuartetteUpperBoundary;
        var randomNumber2 = this.random.Next() % QuartetteUpperBoundary;

        WriteBase36(timestamp, timestampChars, timestampBytes);
        WriteBase36(oldCounter, counterChars, counterBytes);
        WriteBase36(pid, pidChars, pidBytes);
        WriteBase36(hostnameHash, hostnameChars, hostnameBytes);
        WriteBase36(randomNumber1, randomNumber1Chars, randomNumber1Bytes);
        WriteBase36(randomNumber2, randomNumber2Chars, randomNumber2Bytes);

        var binaryRepresentation = Seq(byteArrayRepresentation);

        return new CorrelationId(new string(charArrayRepresentation), binaryRepresentation);
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
        else if (byteArrayRepresentation.Length != ByteArraySize)
        {
            throw new ArgumentOutOfRangeException(nameof(byteArrayRepresentation),
                "CUID byte array representation contains invalid number of characters.");
        }

        var chars = charArrayRepresentation.AsSpan();
        var bytes = byteArrayRepresentation.AsSpan();

        timestampChars = chars.Slice(1, 8);
        timestampBytes = bytes[..6];

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

    private static Dictionary<char, int> CreateCodeMap()
    {
        var codeMap = new Dictionary<char, int>();

        _ = Digits
            .ToCharArray()
            .Do(x => codeMap.Add(x, x - '0'))
            .ToArray();

        _ = LowercaseLetters
            .ToCharArray()
            .Do(x => codeMap.Add(x, x - 'a' + 10))
            .ToArray();

        _ = UppercaseLetters
            .ToCharArray()
            .Do(x => codeMap.Add(x, x - 'A' + 10))
            .ToArray();
        return codeMap;
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

        valueBytes[^bytes.Length..].CopyTo(bytes);
    }

    private static void WriteBase36(long value, Span<char> chars, Span<byte> bytes)
    {
        WriteBase36(value, bytes);
        WriteBase36(value, chars);
    }

    private string GetHostname()
    {
        if (this.hostname == null)
        {
            lock (this.locker)
            {
                if (this.hostname == null)
                {
                    try
                    {
                        this.hostname = Environment.MachineName;
                    }
                    catch
                    {
#pragma warning disable CA5394 // Do not use insecure randomness
                        this.hostname = this.random.Next().ToString(CultureInfo.InvariantCulture);
#pragma warning restore CA5394 // Do not use insecure randomness
                    }
                }
            }
        }

        return this.hostname;
    }
}
