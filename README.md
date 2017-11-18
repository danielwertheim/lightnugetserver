# LightNuGetServer
Uses [Topshelf](https://github.com/topshelf/topshelf) and [NuGet.Server](https://github.com/NuGet/NuGet.Server) to provide a self-hosted NuGet Server with support for multiple configurable feeds.

## Sample settings

```json
{
  "packagesDirPath": "Packages",
  "feeds": [
    {
      "name": "Default",
      "relativeUrl": "nuget",
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
      "name": "Public",
      "relativeUrl": "nuget/public",
      "requiresApiKey": false,
      "nuGetServerSettings": {
        "ignoreSymbolsPackages": true
      }
    }
  ]
}
```

## Upload & Delete packages
Against the configured "Default" feed which is secured by an API-key:

```
nuget push -source http://localhost:5000/nuget .\Topshelf.4.0.3.nupkg -apikey foobar
nuget delete -source http://localhost:5000/nuget Topshelf 4.0.3 -apikey foobar
```

Against the configured "Public" feed which is not secured:

```
nuget push -source http://localhost:5000/nuget/public .\Topshelf.4.0.3.nupkg
nuget delete -source http://localhost:5000/nuget/public Topshelf 4.0.3 -apikey foobar
```