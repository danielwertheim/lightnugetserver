using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using EnsureThat;
using LightNuGetServer.Controllers;
using LightNuGetServer.Internals;

namespace LightNuGetServer
{
    public class LightNuGetFeedControllerActivator : IHttpControllerActivator
    {
        private static readonly Type NuGetFeedControllerType = typeof(LightNuGetFeedController);

        private readonly IHttpControllerActivator _defaultControllerActivator;
        private readonly IReadOnlyDictionary<string, LightNuGetFeed> _feedsBySlug;
        private readonly LightNuGetFeed _singleFeed;

        public LightNuGetFeedControllerActivator(LightNuGetFeed[] feeds)
        {
            Ensure.Collection.HasItems(feeds, nameof(feeds));

            _defaultControllerActivator = new DefaultHttpControllerActivator();
            _feedsBySlug = new ReadOnlyDictionary<string, LightNuGetFeed>(feeds.ToDictionary(f => $"/{f.Name.Slugify()}/"));

            if (_feedsBySlug.Count == 1)
                _singleFeed = _feedsBySlug.Values.Single();
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if (controllerType == NuGetFeedControllerType)
            {
                var feed = GetMatchingFeed(request);

                return feed != null
                    ? new LightNuGetFeedController(feed)
                    : null;
            }

            return _defaultControllerActivator.Create(request, controllerDescriptor, controllerType);
        }

        private LightNuGetFeed GetMatchingFeed(HttpRequestMessage request)
        {
            if (_singleFeed != null)
                return _singleFeed;

            if (_feedsBySlug.TryGetValue(request.RequestUri.AbsolutePath, out var feed))
                return feed;

            return _feedsBySlug.Values.FirstOrDefault(f => request.RequestUri.AbsolutePath.StartsWith(f.Slug));
        }
    }
}