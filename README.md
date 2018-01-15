
<h1 align="center">
SoundByte
</h1>

<img src="UWP_AppPreview_v17_12b6.png" alt="SoundByte Screenshot (v17.12 Build 6)">

<h4 align="center">Music client for Windows 10 &amp; Xbox One (with other platforms in development) supporting SoundCloud, YouTube, Fanburst, Local Playback* and Podcasts*.</h4>

<p>*Features still in development.</p>

<p align="center">
    <a href="https://github.com/DominicMaas/SoundByte/issues">
        <img src="https://img.shields.io/github/issues/dominicmaas/soundbyte.svg" alt="SoundByte Issues">
    </a>
    <a href="https://discord.gg/tftSadE">
        <img src="https://img.shields.io/discord/333524708463214594.svg" alt="Chat on Discord">
    </a>
</p>

## Status

|Project|Status|Download (Stable)|Download (Beta)
|--|--|--|--|
|SoundByte Core|![VSTS](https://gridentertainment.visualstudio.com/_apis/public/build/definitions/2ed2cbbe-2068-4924-89a8-1b43989872e6/4/badge) | [![NuGet](https://img.shields.io/nuget/v/SoundByte.Core.svg)](https://www.nuget.org/packages/SoundByte.Core/)| [![NuGet](https://img.shields.io/nuget/vpre/SoundByte.Core.svg)](https://www.nuget.org/packages/SoundByte.Core/)
|SoundByte UWP|![VSTS](https://gridentertainment.visualstudio.com/_apis/public/build/definitions/29470259-4594-4fc0-9b08-99514c07cfd0/2/badge) | [![NuGet](https://img.shields.io/badge/microsoft_%20store-stable-green.svg)](https://www.microsoft.com/store/apps/9nblggh4xbjg)| [![NuGet](https://img.shields.io/badge/microsoft_%20store-beta-orange.svg)](https://twitter.com/soundbyteuwp)

## Introduction
SoundByte is a Windows 10 and Xbox One app (with other platforms in development) that intergrates with the SoundCloud, Fanburst and YouTube APIs allowing a user to listen to these platforms natively. SoundByte will also support local playback and podcasts in the future. Currently SoundByte supports UWP (Windows 10) and is published through the Windows Store for free.

## SoundByte Structure
SoundByte is split into the following projects: `SoundByte.Android`, `SoundByte.MacOS`, `SoundByte.Core`, `SoundByte.iOS` and `SoundByte.UWP`. Each of these projects and containing files are mentioned in more detail below.

|Project Name|Platform|Description|
|--|--|--|
|`SoundByte.Android`|Xamarin Native (Android)|WIP Xamarin Native app for Android phones|
|`SoundByte.MacOS`|Xamarin Native (macOS)|WIP Xamarin Native app for macOS|
|`SoundByte.Core`|.NET Standard v2.0|Core logic used by all projects within SoundByte|
|`SoundByte.iOS`|Xamarin Native (iOS)|WIP Xamarin Native app for iOS|
|`SoundByte.UWP`|UWP 10.0 - Fall Creators Update - see `cu_stable` for Creators Update support|Windows 10 UWP App (Windows 10/Xbox One)|

## Download
SoundByte can be either downloaded from the Windows Store [here](https://www.microsoft.com/store/apps/9nblggh4xbjg). Windows 10 Creators Update or newer is required to run SoundByte. If you would like to run beta version (compiled every Sunday) direct message the SoundByte twitter (@SoundByteUWP).

## Development

Simply clone the repo to get started. SoundByte will download the required information from the SoundByte servers on app startup.
More information about development will be added to the wiki.

## Credits

- **[Dominic Maas](https://twitter.com/dominicjmaas)**  - *App Development*
- **[Dennis Bednarz](https://twitter.com/DennisBednarz)**  - *App UI/UX Design*

See also the list of [contributors](https://github.com/DominicMaas/SoundByte/contributors) who participated in this project.

## License

The project is released under MIT License

MIT License

Copyright 2018 Grid Entertainment

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
