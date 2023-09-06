using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Template.MetadataSources;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Template.Providers;

public class AudiobookMetadataProvider : IRemoteMetadataProvider<AudioBook, SongInfo>
{
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SongInfo searchInfo, CancellationToken cancellationToken)
    {
        AudioBookMetadata abm = AudibleMetadataFetcher.FetchMetadataByTitle(searchInfo.Album);
        return Task.FromResult<IEnumerable<RemoteSearchResult>>(new List<RemoteSearchResult> {new RemoteSearchResult {Name = searchInfo.Name, ImageUrl = abm.ArtworkUrl, SearchProviderName = Name,}}.AsEnumerable());
    }

    public Task<MetadataResult<AudioBook>> GetMetadata(SongInfo info, CancellationToken cancellationToken)
    {
        AudioBookMetadata abm = AudibleMetadataFetcher.FetchMetadataByTitle(info.Album);
        return Task.FromResult(new MetadataResult<AudioBook>()
        {
            HasMetadata = true,
            Item = new AudioBook()
            {
                Name = info.Album,
                Overview = abm.Overview,
                Artists = new List<string> {abm.Author, abm.NarratedBy},
                Genres = abm.Tags,
                Tags = abm.Tags,
                CommunityRating = abm.CommunityRating,
                SortName = abm.SortTitle,
                AlbumArtists = new List<string> {abm.NarratedBy}
            }
        });
    }

    public string Name { get; }
    private readonly IHttpClientFactory _httpClientFactory;

    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        return _httpClientFactory.CreateClient(NamedClient.Default).GetAsync(new Uri(url), cancellationToken);
    }

    public AudiobookMetadataProvider(IHttpClientFactory httpClientFactory)
    {
       _httpClientFactory = httpClientFactory;
    }

}
