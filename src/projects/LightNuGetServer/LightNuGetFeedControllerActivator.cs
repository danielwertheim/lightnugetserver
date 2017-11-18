using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using EnsureThat;
using LightNuGetServer.Controllers;

namespace LightNuGetServer
{
    public class LightNuGetFeedControllerActivator : IHttpControllerActivator
    {
        private static readonly Type NuGetFeedControllerType = typeof(LightNuGetFeedController);

        private readonly IHttpControllerActivator _defaultControllerActivator;
        private readonly IReadOnlyDictionary<string, LightNuGetFeed> _feeds;
        private readonly LightNuGetFeed _singleFeed;

        public LightNuGetFeedControllerActivator(IDictionary<string, LightNuGetFeed> feeds)
        {
            Ensure.Collection.HasItems(feeds, nameof(feeds));

            _defaultControllerActivator = new DefaultHttpControllerActivator();
            _feeds = new ReadOnlyDictionary<string, LightNuGetFeed>(feeds);

            if (_feeds.Count == 1)
                _singleFeed = _feeds.Values.Single();
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

            if (_feeds.TryGetValue(request.RequestUri.AbsolutePath, out var feed))
                return feed;

            return _feeds.Values.FirstOrDefault(f => request.RequestUri.AbsolutePath.StartsWith(f.Key));
        }
    }
}