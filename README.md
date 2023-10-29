# Ogu.Otp

[![.NET](https://github.com/ogulcanturan/Ogu.Otp/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/ogulcanturan/Ogu.Otp/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/Ogu.Otp.svg?color=1ecf18)](https://nuget.org/packages/Ogu.Otp)
[![Nuget](https://img.shields.io/nuget/dt/Ogu.Otp.svg?logo=nuget)](https://nuget.org/packages/Ogu.Otp)

## Introduction

This library offers support for both Time-Based One-Time Password (`TOTP`) and HMAC-based one-time password (`HOTP`) protocols, enabling secure and reliable authentication for various applications. With seamless integration for `Google`, `Microsoft`, and other `RFC 6238-compliant` authentication systems


## Features

- TOTP (MD5, SHA1, SHA256, SHA384, SHA512)
- HOTP (MD5, SHA1, SHA256, SHA384, SHA512)
- Includes Base32Helper class

## Installation

You can install the library via NuGet Package Manager:

```bash
dotnet add package Ogu.Otp
```
## Usage

**sample console app:** [Otp.Console](https://github.com/ogulcanturan/Ogu.Otp/blob/master/samples/Otp.Console/Program.cs/)

**sample web app:** [Otp.Api](https://github.com/ogulcanturan/Ogu.Otp/blob/master/samples/Otp.Api/)

## Credits


This library makes use of certain codes and resources from [AspNetCore](https://github.com/dotnet/aspnetcore), [Base32.cs](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/Base32.cs), [Rfc6238AuthenticationService.cs](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/Rfc6238AuthenticationService.cs)