using System;

namespace Jellyfin.Plugin.Template;

/// <summary>
/// Audio book metadata storage object.
/// </summary>
public class AudioBookMetadata {
    private string? Title { get; }

    private string SortTitle { get; }

    private string CommunityRating { get; }

    private string Overview { get; }

    private string NarratedBy { get; }

    private string Author { get; }

    private string Publisher { get; }

    private string[] Tags { get; }

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
    public AudioBookMetadata(
        string title = "",
        string sortTitle = "",
        string communityRating = "",
        string overview = "",
        string narratedBy = "",
        string author = "",
        string publisher = "",
        string[]? tags = null)
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
