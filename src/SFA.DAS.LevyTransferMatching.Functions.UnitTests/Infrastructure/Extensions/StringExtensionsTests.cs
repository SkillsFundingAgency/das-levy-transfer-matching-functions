using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Extensions;

namespace SFA.DAS.LevyTransferMatching.Functions.UnitTests.Infrastructure.Extensions;

[TestFixture]
public class StringExtensionsTests
{
    [TestCase("hello", "hello")]
    [TestCase("Hello", "hello")]
    [TestCase("HelloWorld", "hello_world")]
    [TestCase("HelloWorldHereIAm", "hello_world_here_i_am")]
    public void ToUnderscoreCaseProducesExpectedResults(string source, string expected)
    {
        var result = source.ToUnderscoreCase();
        Assert.That(result, Is.EqualTo(expected));
    }
}