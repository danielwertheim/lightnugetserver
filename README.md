# LightNuGetServer
Uses [NuGet.Server](https://github.com/NuGet/NuGet.Server) to provide a self-hosted NuGet Server with support for multiple configurable feeds.

## Sample settings
A sample configuring two different feeds. The name of the feed is used to produce a URL friendly slug.

```json
{
  "packagesDirPath": "Packages",
  "feeds": [
    {
      "name": "Default", //produces URL "default"
      "apiKey": "foobar",
      "requiresApiKey": true,
      "nuGetServerSettings": {
        "allowOverrideExistingPackageOnPush": false,
        "ignoreSymbolsPackages": false,
        "enableDelisting": false,
        "enableFrameworkFiltering": false,
        "enableFileSystemMonitoring": true
      }
    },
    {
      "name": "Some Weird Name", //produces URL "some-weird-name"
      "requiresApiKey": false,
      "nuGetServerSettings": {
        "ignoreSymbolsPackages": true
      }
    }
  ]
}
```

## Installing
Since `v0.3.0` TopShelf is no longer used. To install as service you can use `sc.exe`, e.g.

```
sc create LightNuGetServer start= auto binPath= "C:\SomePath\LightNuGetServer.Host.exe" DisplayName= LightNuGetServer
```

### Adding an UrlAcl
In case you get an issue with not being able to start under the account you have assigned to use for your service, you might need to manually add an UrlAcl. This would look something like this (**NOTE!** port should match what is in the `App.config`):

```
netsh http add urlacl url=http://+:5000/ user="NT AUTHORITY\NETWORK SERVICE"
```

## Upload & Delete packages
Against the configured "Default" feed which is secured by an API-key:

```
nuget push .\Topshelf.4.0.3.nupkg -source http://localhost:5000/default -apikey foobar
nuget delete Topshelf 4.0.3 -source http://localhost:5000/default -apikey foobar
```

Against the configured "Public" feed which is not secured:

```
nuget push .\Topshelf.4.0.3.nupkg -source http://localhost:5000/some-weird-name
nuget delete Topshelf 4.0.3 -source http://localhost:5000/some-weird-name
```
