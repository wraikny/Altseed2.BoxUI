[![](https://github.com/wraikny/Altseed2.BoxUI/workflows/CI/badge.svg)](https://github.com/wraikny/Altseed2.BoxUI/actions?workflow=CI)
[![Nuget](https://img.shields.io/nuget/v/Altseed2.BoxUI?style=plastic)](https://www.nuget.org/packages/Altseed2.BoxUI/)

# Altseed2.BoxUI

Altseed2.BoxUI は 宣言的な GUI レイアウトを簡単かつ軽量に行うための [Altseed2](https://github.com/altseed/Altseed2-csharp) 拡張ライブラリです。

[Install from NuGet](https://www.nuget.org/packages/Altseed2.BoxUI)

```sh
dotnet add package Altseed2.BoxUI
```

以前解説記事を書きました。

[Altseed2で軽量な宣言的UIを支援するライブラリAltseed2.BoxUIの紹介 - AmusementCreators](https://www.amusement-creators.info/articles/advent_calendar/2020/25/)

## Setup

```sh
$ dotnet tool restore
```

## Build

```sh
$ dotnet build
```

## Update Nuget Package

```sh
$ dotnet pack src/Altseed2.BoxUI -c RELEASE
```

Then, drug & drop `src/Release/Altseed2.BoxUI.*.nupkg` to the Nuget website.

