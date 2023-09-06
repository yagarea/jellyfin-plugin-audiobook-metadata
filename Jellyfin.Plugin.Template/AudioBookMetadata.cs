using System;

namespace Jellyfin.Plugin.Template;

/// <summary>
/// Audio book metadata storage object.
/// </summary>
public class AudioBookMetadata
{
    /// <summary>
    /// Gets the audiobook title.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets Jellyfin specific sort title.
    /// </summary>
    public string SortTitle { get; init; }

    /// <summary>
    /// Gets the audiobook community rating.
    /// </summary>
    public string CommunityRating { get; init; }

    /// <summary>
    /// Gets the audiobook overview.
    /// </summary>
    public string Overview { get; init; }

    /// <summary>
    /// Gets the audiobook narrator.
    /// </summary>
    public string NarratedBy { get; init; }

    /// <summary>
    /// Gets the audiobook author.
    /// </summary>
    public string Author { get; init; }

    /// <summary>
    /// Gets the audiobook publisher.
    /// </summary>
    public string Publisher { get; init; }

    /// <summary>
    /// Gets array of audiobook tags.
    /// </summary>
    public string[] Tags { get; init; }

    public string ArtworkUrl { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioBookMetadata"/> class.
    /// Class containing metadata for an audiobook.
    /// </summary>
    /// <param name="title">Audiobook title.</param>
    /// <param name="sortTitle">Audiobook sort title.</param>
    /// <param name="communityRating">Audiobook community rating 0-10.</param>
    /// <param name="overview">Audiobook overview.</param>
    /// <param name="narratedBy">Audiobook narrator.</param>
    /// <param name="author">Audiobook author.</param>
    /// <param name="publisher">Audiobook publisher.</param>
    /// <param name="tags">Audiobook tags.</param>
    /// <param name="artworkUrl">Audiobook artwork url.</param>
    public AudioBookMetadata(
        string title = "",
        string sortTitle = "",
        string communityRating = "",
        string overview = "",
        string narratedBy = "",
        string author = "",
        string publisher = "",
        string[]? tags = null,
        string artworkUrl = "")
    {
        this.Title = title;
        this.SortTitle = sortTitle;
        this.CommunityRating = communityRating;
        this.Overview = overview;
        this.NarratedBy = narratedBy;
        this.NarratedBy = narratedBy;
        this.Author = author;
        this.Publisher = publisher;
        this.Tags = tags ?? Array.Empty<string>();
        this.ArtworkUrl = artworkUrl;
    }

    /// <summary>
    /// Print the metadata contents to the console.
    /// </summary>
    public void DebugPrint()
    {
        Console.WriteLine($"Title: {Title}");
        Console.WriteLine($"Sort title: {SortTitle}");
        Console.WriteLine($"Community rating: {CommunityRating}");
        Console.WriteLine($"Overview: {Overview}");
        Console.WriteLine($"Narrated by: {NarratedBy}");
        Console.WriteLine($"Author: {Author}");
        Console.WriteLine($"Publisher: {Publisher}");
        Console.WriteLine($"Tags: {Tags}");
    }
}
