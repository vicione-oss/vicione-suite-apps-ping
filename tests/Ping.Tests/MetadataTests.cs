using Sdk.Testing;
using Xunit;

namespace Ping.Tests;

public class MetadataTests
{
    [Fact]
    public void Should_be_serializeable()
    {
        MetadataValidator.ValidateMetadata();
    }
}
