using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace TestUtils;

[ExcludeFromCodeCoverage]
public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() => {
            var f = new Fixture().Customize(new AutoMoqCustomization());
            f.Behaviors.Add(new OmitOnRecursionBehavior());
            return f;
        })
    {
    }
}
