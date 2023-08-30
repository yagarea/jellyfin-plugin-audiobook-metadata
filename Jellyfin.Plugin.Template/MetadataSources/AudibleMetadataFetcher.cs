using System;
using System.Globalization;
using HtmlAgilityPack;

namespace Jellyfin.Plugin.Template.MetadataSources;

/// <summary>
/// Class containing methods to fetch metadata from audible.com.
/// </summary>
public class AudibleMetadataFetcher
{
    private const string BaseBookUrl = "https://www.audible.com/pd/";
    private const string SearchUrl = "https://www.audible.com/search?keywords=";

    private static HtmlDocument GetAudiobookPageHtml(string audiobookId)
    {
        string bookUrl = AudibleMetadataFetcher.BaseBookUrl + audiobookId;
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(bookUrl);
        if (web.StatusCode == System.Net.HttpStatusCode.OK) { return doc; }
        else if (web.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new Exception("Could not find audiobook");
        }
        else
        {
            throw new Exception("Could not connect to Audible.com");
        }
    }

    private static string? GetTitle(HtmlDocument doc)
    {
        return doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[1]/h1")[0].InnerText;
    }

    private static string? GetSortTitle(HtmlDocument doc)
    {
        return GetTitle(doc);
    }

    private static string GetAuthor(HtmlDocument doc)
    {
        HtmlNodeCollection authors = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[3]/a");
        string author = string.Empty;
        foreach (HtmlNode node in authors) author += " " + node.InnerText;
        return author.Trim();
    }

    private static string? GetPublisher(HtmlDocument doc)
    {
        return doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[30]/div/div/div[2]/span")[0].InnerText;
    }

    private static string GetNarrator(HtmlDocument doc)
    {
        HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li");
        string narrators = string.Empty;
        foreach (HtmlNode node in linkNodes) narrators += " " + node.InnerText;
        return narrators.Trim();
    }

    private static string GetOverview(HtmlDocument doc)
    {
        HtmlNodeCollection summary = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[10]/div/div/div[1]/span");
        string overview = string.Empty;
        foreach (HtmlNode node in summary) overview += "\n\n" + node.InnerText;
        return overview.Trim();
    }

    private static int? GetCommunityRating(HtmlDocument doc)
    {
        string rating = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[52]/div/div[2]/div[1]/span/ul/li[1]/span[3]")[0].InnerText;
        rating = rating.Replace(" out of 5 stars", String.Empty);
        return (int)(float.Parse(rating, NumberStyles.Float) * 2); // converting stars to x/10 score
    }

    private static string[] GetTags(HtmlDocument doc)
    {
        // TODO: Not consistent across books
        HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li/a");
        string[] tagArray = new string[tags.Count];
        for (int i = 0; i < tags.Count; i++) { tagArray[i] = tags[i].InnerText; }

        return tagArray;
    }

    /// <summary>
    /// Fetch metadata for an audiobook from audible.
    /// </summary>
    /// <param name="audiobookId">ID from url of book page.</param>
    /// <returns>AudiobookMetadata object.</returns>
    public static AudioBookMetadata FetchMetadataById(string audiobookId)
    {
        HtmlDocument doc = GetAudiobookPageHtml(audiobookId);
        return new AudioBookMetadata(
            title: GetTitle(doc) ?? string.Empty,
            sortTitle: GetSortTitle(doc) ?? string.Empty,
            communityRating: GetCommunityRating(doc).ToString() ?? string.Empty,
            overview: GetOverview(doc),
            narratedBy: GetNarrator(doc),
            publisher: GetPublisher(doc) ?? string.Empty,
            author: GetAuthor(doc),
            tags: GetTags(doc)
        );
    }

    private static String GetIdByTitle(String title)
    {
        string bookUrl = SearchUrl + title.Replace(" ", "+");
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(bookUrl);
        if (web.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string bookLink = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[5]/div[5]/div/div[2]/div[4]/div/div/div/span[2]/ul/li[1]/div/div[1]/div/div[2]/div/div/span/ul/li[1]/h3/a")[0].Attributes["href"].Value;
            bookLink = bookLink.Split("?")[0];
            return bookLink.Split("/")[bookLink.Split("/").Length - 1];
        }
        else
        {
            throw new Exception("Could not connect to Audible.com");
        }
    }

    /// <summary>
    /// Fetch metadata for an audiobook from audible found by title.
    /// </summary>
    /// <param name="title">Title of audiobook.</param>
    /// <returns>AudiobookMetaData object containing metadata of book with that title.</returns>
    public static AudioBookMetadata FetchMetadataByTitle(string title)
    {
        return FetchMetadataById(GetIdByTitle(title));
    }
}
