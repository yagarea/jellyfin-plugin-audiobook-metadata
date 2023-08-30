using Jellyfin.Plugin.Template.MetadataSources;

namespace Jellyfin.Plugin.Template.Tests;

using Xunit;

/// <summary>
/// Tests for the AudibleMetadataFetcher class.
/// </summary>
public class AudibleMetadataFetcherTests
{
    /// <summary>
    /// Test that the title is fetched correctly.
    /// </summary>
    [Fact]
    public void TestHarryPotterTitle()
    {
        AudioBookMetadata hp = AudibleMetadataFetcher.FetchMetadataById("B017V4IM1G");
        Assert.Equal("Harry Potter and the Sorcerer's Stone, Book 1", hp.Title);
    }
}
