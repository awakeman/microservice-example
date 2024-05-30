using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using Xunit;

namespace TestUtils;

[ExcludeFromCodeCoverage]
public class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base( [ new InlineDataAttribute(values), new AutoMoqDataAttribute() ])
    {
    }
}