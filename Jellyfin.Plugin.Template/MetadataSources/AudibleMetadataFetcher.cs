using System;
using HtmlAgilityPack;

namespace Jellyfin.Plugin.Template;

public class AudibleMetadataFetcher {

    private const string BaseBookURL = "https://www.audible.com/pd/";
    private const string searchURL = "https://www.audible.com/search?keywords=";

    private static HtmlDocument getAudiobookPageHTML(string audiobookID) {
        string bookUrl = BaseBookURL + audiobookID;
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(bookUrl);
        if (web.StatusCode == System.Net.HttpStatusCode.OK) return doc;
        else if (web.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception("Could not find audiobook");
        else throw new Exception("Could not connect to Audible.com");
    }

    private string? GetTitle(HtmlDocument doc) {
        return doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[1]/h1")[0].InnerText;
    }

    private string? GetSortTitle(HtmlDocument doc) {
        return GetTitle(doc);
    }

    private string? GetAuthor(HtmlDocument doc) {
        HtmlNodeCollection authors = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[3]/a");
        string author = "";
        foreach (HtmlNode node in authors) author += " " + node.InnerText;
        return author.Trim();
    }

    private string? GetPublisher(HtmlDocument doc) {
        return doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[30]/div/div/div[2]/span")[0].InnerText;
    }

    private string? GetNarrator(HtmlDocument doc) {
        HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li");
        string narrators = "";
        foreach (HtmlNode node in linkNodes) narrators += " " + node.InnerText;
        return narrators.Trim();
    }

    private string? GetOverview(HtmlDocument doc) {
        HtmlNodeCollection summury = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[10]/div/div/div[1]/span");
        string overview = "";
        foreach (HtmlNode node in summury) overview += "\n\n" + node.InnerText;
        return overview.Trim();
    }

    private int? GetCommunityRating(HtmlDocument doc) {
        string rating = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[52]/div/div[2]/div[1]/span/ul/li[1]/span[3]")[0].InnerText;
        rating = rating.Replace(" out of 5 stars", "");
        return (int) (float.Parse(rating) * 2);
    }

    private string[]? GetTags(HtmlDocument doc) {
        // TODO: Not consistent across books
        HtmlNodeCollection tags = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li/a");
        string[] tagArray = new string[tags.Count];
        for (int i = 0; i < tags.Count; i++) tagArray[i] = tags[i].InnerText;
        return tagArray;
    }

    /// <summary>
    /// Fetch metadata for an audiobook from audible
    /// </summary>
    /// <param name="audiobookID">ID from url of book page</param>
    /// <returns>AudiobookMetadata object</returns>
    public AudioBookMetadata FetchMetadataByID(string audiobookID)
    {
        HtmlDocument doc = getAudiobookPageHTML(audiobookID);
        return new AudioBookMetadata(
            title: GetTitle(doc) ?? "",
            sortTitle: GetSortTitle(doc) ?? "",
            communityRating: GetCommunityRating(doc).ToString() ?? "",
            overview: GetOverview(doc) ?? "",
            narratedBy: GetNarrator(doc) ?? "",
            publisher: GetPublisher(doc) ?? "",
            author: GetAuthor(doc) ?? "",
            tags: GetTags(doc) ?? Array.Empty<string>()
        );

    }

    private String getIDByTitle(String title)
    {
        string bookUrl = searchURL + title.Replace(" ", "+");
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(bookUrl);
        if (web.StatusCode == System.Net.HttpStatusCode.OK) {
            String bookLink = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[5]/div[5]/div/div[2]/div[4]/div/div/div/span[2]/ul/li[1]/div/div[1]/div/div[2]/div/div/span/ul/li[1]/h3/a")[0].Attributes["href"].Value;
            bookLink = bookLink.Split("?")[0];
            return bookLink.Split("/")[bookLink.Split("/").Length - 1];
        } else throw new Exception("Could not connect to Audible.com");
    }


    public AudioBookMetadata FetchMetadataByTitle(string title) {
        return FetchMetadataByID(getIDByTitle(title));
    }
}
