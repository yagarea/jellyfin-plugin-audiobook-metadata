using System;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
using NUnit.Framework;

namespace Jellyfin.Plugin.Template.MetadataSources;

/// <summary>
/// Class containing methods to fetch metadata from audible.com.
/// </summary>
public class AudibleMetadataFetcher
{
    private const string BaseBookUrl = "https://www.audible.com/pd/";
    private const string SearchUrl = "https://www.audible.com/search?keywords=";


    private const string TitleXPath = "/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[1]/h1";
    private const string AuthorXPath = "/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[3]/a";
    private const string PublisherXpath = "/html/body/div[1]/div[8]/div[10]/div/div/div[2]/span";
    private const string NarratorsXPath = "/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[2]/span/ul/li[4]/a";
    private const string OverviewXPath = "/html/body/div[1]/div[8]/div[10]/div/div/div[1]/span";
    private const string CommunityRatingXPath = "/html/body/div[1]/div[8]/div[52]/div/div[2]/div[1]/span/ul/li[1]/span[3]";
    private const string TagGroupXPath = "/html/body/div[1]/div[8]/div[14]/div/div/div/div/span";
    private const string ArtworkXPath = "/html/body/div[1]/div[8]/div[2]/div/div[3]/div/div/div/div[1]/div/div[1]/img";

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
        return doc.DocumentNode.SelectNodes(TitleXPath)[0].InnerText;
    }

    private static string? GetSortTitle(HtmlDocument doc)
    {
        return GetTitle(doc);
    }

    private static string GetAuthor(HtmlDocument doc)
    {
        HtmlNodeCollection authors = doc.DocumentNode.SelectNodes(AuthorXPath);
        string author = authors.Aggregate(string.Empty, (current, node) => current + (" " + node.InnerText));
        return author.Trim();
    }

    private static string? GetPublisher(HtmlDocument doc)
    {
        String publisher = doc.DocumentNode.SelectNodes(PublisherXpath)[0].InnerText;
        return publisher.Trim();
    }

    private static string GetNarrator(HtmlDocument doc)
    {
        HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes(NarratorsXPath);
        string narrators = string.Empty;
        foreach (HtmlNode node in linkNodes) narrators += " " + node.InnerText;
        return narrators.Trim();
    }

    private static string GetOverview(HtmlDocument doc)
    {
        HtmlNodeCollection summary = doc.DocumentNode.SelectNodes(OverviewXPath);
        string overview = string.Empty;
        foreach (HtmlNode node in summary) overview += "\n\n" + node.InnerText;
        return overview.Trim();
    }

    private static int GetCommunityRating(HtmlDocument doc)
    {
        string rating = doc.DocumentNode.SelectNodes(CommunityRatingXPath)[0].InnerText.Trim();
        rating = rating.Replace(" out of 5 stars", String.Empty);
        string ratingValue = rating.Substring(0, 3);
        return (int)(double.Parse(ratingValue, CultureInfo.InvariantCulture) * 2);
    }

    private static string[] GetTags(HtmlDocument doc)
    {
        HtmlNodeCollection rawTags = doc.DocumentNode.SelectNodes(TagGroupXPath);
        string[] tagArray = new string[rawTags.Count];
        for (int i = 0; i < rawTags.Count; i++) {
            HtmlNodeCollection tag = rawTags[i].SelectNodes("span/a/span/span");
            tagArray[i] = tag[0].InnerText.Trim();
        }
        return tagArray;
    }

    private static string GetArtworkUrl(HtmlDocument doc) {
        return doc.DocumentNode.SelectNodes(ArtworkXPath)[0].Attributes["src"].Value;
    }




    /// <summary>
    /// Fetch metadata for an audiobook from audible.
    /// </summary>
    /// <param name="audiobookId">ID from url of book page.</paraB017V4IM1Gm>
    /// <returns>AudiobookMetadata object.</returns>
    public static AudioBookMetadata FetchMetadataById(string audiobookId)
    {
        HtmlDocument doc = GetAudiobookPageHtml(audiobookId);
        return new AudioBookMetadata(
            title: GetTitle(doc) ?? string.Empty,
            sortTitle: GetSortTitle(doc) ?? string.Empty,
            communityRating: GetCommunityRating(doc).ToString(CultureInfo.InvariantCulture),
            overview: GetOverview(doc),
            narratedBy: GetNarrator(doc),
            publisher: GetPublisher(doc) ?? string.Empty,
            author: GetAuthor(doc),
            tags: GetTags(doc)
        );
    }


    private static String GetIdByTitle(String title)
    {
        string bookUrl = SearchUrl + title.Replace(" ", "+", System.StringComparison.Ordinal);
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(bookUrl);
        if (web.StatusCode == System.Net.HttpStatusCode.OK)
        {
            string bookLink = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[5]/div[5]/div/div[2]/div[4]/div/div/div/span[2]/ul/li[1]/div/div[1]/div/div[2]/div/div/span/ul/li[1]/h3/a")[0].Attributes["href"].Value;
            bookLink = bookLink.Split("?")[0];
            return bookLink.Split("/")[bookLink.Split("/").Length - 1];
        }

        throw new Exception("Could not connect to Audible.com");
    }

    /// <summary>
    /// Fetch metadata for an audiobook from audible found by title.
    /// </summary>
    /// <param name="title">Title of audiobook.</param>
    /// <returns>AudiobookMetaData object containing metadata of book with that title.</returns>
    public static AudioBookMetadata FetchMetadataByTitle(string title) {
        return FetchMetadataById(GetIdByTitle(title));
    }


    /// <summary>
    /// Test on Harry Potter
    /// </summary>
    [Test]
    public void TestHarryPotter() {
        HtmlDocument doc = GetAudiobookPageHtml("B017V4IM1G");
        Assert.AreEqual("Harry Potter and the Sorcerer's Stone, Book 1", GetTitle(doc));
        Assert.AreEqual("Harry Potter and the Sorcerer's Stone, Book 1", GetSortTitle(doc));
        Assert.AreEqual("J.K. Rowling", GetAuthor(doc));
        Assert.AreEqual("\u00a91997 J.K. Rowling (P)1999 Listening Library, an imprint of Penguin Random House Audio Publishing Group", GetPublisher(doc));
        Assert.AreEqual("Jim Dale", GetNarrator(doc));
        Assert.AreEqual("Harry Potter and the Sorcerer’s Stone will be streaming in Audible Plus through August 4th, 2023. Turning the envelope over, his hand trembling, Harry saw a purple wax seal bearing a coat of arms; a lion, an eagle, a badger and a snake surrounding a large letter 'H'. Harry Potter has never even heard of Hogwarts when the letters start dropping on the doormat at number four, Privet Drive. Addressed in green ink on yellowish parchment with a purple seal, they are swiftly confiscated by his grisly aunt and uncle. Then, on Harry's eleventh birthday, a great beetle-eyed giant of a man called Rubeus Hagrid bursts in with some astonishing news: Harry Potter is a wizard, and he has a place at Hogwarts School of Witchcraft and Wizardry. An incredible adventure is about to begin! Having become classics of our time, the Harry Potter stories never fail to bring comfort and escapism. With their message of hope, belonging and the enduring power of truth and love, the story of the Boy Who Lived continues to delight generations of new listeners.", GetOverview(doc));
        Assert.AreEqual(9, GetCommunityRating(doc));
        Assert.AreEqual(new string[] {"Growing Up & Facts of Life", "Family Life", "Fantasy & Magic", "Fantasy"}, GetTags(doc));
        Assert.AreEqual("https://m.media-amazon.com/images/I/51xJbFMRsxL._SL500_.jpg", GetArtworkUrl(doc));
    }

}
