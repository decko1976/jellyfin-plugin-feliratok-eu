<h1 align="center">Jellyfin feliratok.eu Plugin</h1>


## About

This is a plugin allows you to download subtitles from [feliratok.eu Subtitles](https://feliratok.eu) for your media. Actually only movies supported, series would be supported later.

## Installation

[See the official documentation for install instructions](https://jellyfin.org/docs/general/server/plugins/index.html#installing).

## Build

1. To build this plugin you will need [.Net 8.x](https://dotnet.microsoft.com/download/dotnet/8.0).

2. Build plugin with following command
  ```
  dotnet publish --configuration Release --output bin
  ```

3. Place the dll-file and bin/HtmlAgilityPack.dll in the `plugins/feliratok` folder (you might need to create the folders) of your JF install

## Releasing

To release the plugin we recommend [JPRM](https://github.com/oddstr13/jellyfin-plugin-repository-manager) that will build and package the plugin.
For additional context and for how to add the packaged plugin zip to a plugin manifest see the [JPRM documentation](https://github.com/oddstr13/jellyfin-plugin-repository-manager) for more info.

## Contributing

We welcome all contributions and pull requests! If you have a larger feature in mind please open an issue so we can discuss the implementation before you start.
In general refer to our [contributing guidelines](https://github.com/jellyfin/.github/blob/master/CONTRIBUTING.md) for further information.

## Licence

This plugins code and packages are distributed under the GPLv3 License. See [LICENSE](./LICENSE) for more information.
