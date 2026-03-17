#nullable enable

namespace Shadop.Archmage
{
    /// <summary>
    /// Indicates whether a value represents its zero/default state.
    /// </summary>
    public interface IZero
    {
        bool IsZero { get; }
    }
}
