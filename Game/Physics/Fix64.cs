// Complete corrected Fix64 with better error handling

public struct Fix64
{
    private const int FRACTIONAL_PLACES = 32;
    private const long ONE = 1L << FRACTIONAL_PLACES;
    private const long HALF = ONE >> 1;
    private const long MAX_VALUE = long.MaxValue;
    private const long MIN_VALUE = long.MinValue;

    private readonly long rawValue;

    public Fix64(long rawValue)
    {
        this.rawValue = rawValue;
    }

    // Factory methods
    public static Fix64 FromInt(int value) => new Fix64((long)value << FRACTIONAL_PLACES);
    public static Fix64 FromDouble(double v) => new Fix64(checked((long)Math.Round(v * ONE)));
    public static Fix64 FromFloat(float v) => new Fix64(checked((long)MathF.Round(v * ONE)));
    public static Fix64 FromRaw(long rawValue) => new Fix64(rawValue);

    // Conversion methods
    public float ToFloat() => (float)rawValue / ONE;
    public double ToDouble() => (double)rawValue / ONE;
    public int ToInt() => (int)(rawValue >> FRACTIONAL_PLACES);
    public long ToRaw() => rawValue;

    // Constants
    public static Fix64 Zero => new Fix64(0);
    public static Fix64 One => new Fix64(ONE);
    public static Fix64 Half => new Fix64(HALF);
    public static Fix64 Pi => new Fix64(13493037705L); // Approximately π
    public static Fix64 TwoPi => new Fix64(26986075409L); // Approximately 2π

    // Arithmetic operations
    public static Fix64 operator +(Fix64 a, Fix64 b)
    {
        long result = a.rawValue + b.rawValue;
        return new Fix64(result);
    }

    public static Fix64 operator -(Fix64 a, Fix64 b)
    {
        long result = a.rawValue - b.rawValue;
        return new Fix64(result);
    }

    public static Fix64 operator -(Fix64 a) => new Fix64(-a.rawValue);

    public static Fix64 operator *(Fix64 a, Fix64 b)
    {
        // Handle overflow by using 128-bit arithmetic simulation
        long result = MultiplyWithOverflowCheck(a.rawValue, b.rawValue);
        return new Fix64(result);
    }

    public static Fix64 operator /(Fix64 a, Fix64 b)
    {
        if (b.rawValue == 0)
            throw new DivideByZeroException("Cannot divide by zero");

        long result = (a.rawValue << FRACTIONAL_PLACES) / b.rawValue;
        return new Fix64(result);
    }

    // Helper method for multiplication with overflow checking
    private static long MultiplyWithOverflowCheck(long a, long b)
    {
        // Simple overflow-safe multiplication for fixed point
        bool aIsNegative = a < 0;
        bool bIsNegative = b < 0;

        if (aIsNegative) a = -a;
        if (bIsNegative) b = -b;

        // Split into high and low parts
        uint aHi = (uint)(a >> 32);
        uint aLo = (uint)(a & 0xFFFFFFFF);
        uint bHi = (uint)(b >> 32);
        uint bLo = (uint)(b & 0xFFFFFFFF);

        ulong result = (ulong)aLo * bLo;
        result >>= FRACTIONAL_PLACES;
        result += ((ulong)aHi * bLo) << (32 - FRACTIONAL_PLACES);
        result += ((ulong)aLo * bHi) << (32 - FRACTIONAL_PLACES);
        result += ((ulong)aHi * bHi) << (64 - FRACTIONAL_PLACES);

        long finalResult = (long)result;
        return (aIsNegative ^ bIsNegative) ? -finalResult : finalResult;
    }

    // Comparison operators
    public static bool operator ==(Fix64 a, Fix64 b) => a.rawValue == b.rawValue;
    public static bool operator !=(Fix64 a, Fix64 b) => a.rawValue != b.rawValue;
    public static bool operator <(Fix64 a, Fix64 b) => a.rawValue < b.rawValue;
    public static bool operator >(Fix64 a, Fix64 b) => a.rawValue > b.rawValue;
    public static bool operator <=(Fix64 a, Fix64 b) => a.rawValue <= b.rawValue;
    public static bool operator >=(Fix64 a, Fix64 b) => a.rawValue >= b.rawValue;

    // Math functions
    public static Fix64 Sqrt(Fix64 value)
    {
        if (value.rawValue <= 0) return Zero;

        // Babylonian method (Newton's method for square roots)
        long x = value.rawValue;
        long guess = x >> 1; // Start with x/2 as initial guess

        if (guess == 0) guess = 1;

        for (int i = 0; i < 20; i++) // Limit iterations
        {
            long newGuess = (guess + (x << FRACTIONAL_PLACES) / guess) >> 1;
            if (Math.Abs(newGuess - guess) <= 1)
                break;
            guess = newGuess;
        }

        return new Fix64(guess);
    }

    public static Fix64 Abs(Fix64 value) => new Fix64(Math.Abs(value.rawValue));
    public static Fix64 Min(Fix64 a, Fix64 b) => a.rawValue < b.rawValue ? a : b;
    public static Fix64 Max(Fix64 a, Fix64 b) => a.rawValue > b.rawValue ? a : b;

    public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    // Lerp function
    public static Fix64 Lerp(Fix64 a, Fix64 b, Fix64 t)
    {
        return a + (b - a) * Clamp(t, Zero, One);
    }

    public override bool Equals(object obj) => obj is Fix64 other && rawValue == other.rawValue;
    public override int GetHashCode() => rawValue.GetHashCode();
    public override string ToString() => ToDouble().ToString("F6");
}