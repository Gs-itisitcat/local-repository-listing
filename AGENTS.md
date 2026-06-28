# Repository Guidelines

## Project Structure & Module Organization

This repository contains a small .NET command-line tool. The solution file is `local-repository-listing.sln`; the application project lives in `local-repository-listing/`. Entry point code is in `Program.cs`, command handling is in `LocalRepositoryListingCommand.cs`, repository discovery code is under `Searcher/`, and output/fuzzy-finder integration is under `ResultLister/`. The shell helper function distributed with releases is `src/lepos.sh`. GitHub Actions release automation is in `.github/workflows/dotnet-publish.yml`.

## Build, Test, and Development Commands

- `dotnet restore local-repository-listing/local-repository-listing.csproj` restores NuGet dependencies.
- `dotnet build local-repository-listing.sln` builds the solution for the configured `net10.0` target.
- `dotnet run --project local-repository-listing/local-repository-listing.csproj -- --list-only --root <path>` runs `lepol` locally and prints discovered repositories without invoking `fzf`.
- `dotnet publish local-repository-listing/local-repository-listing.csproj --configuration Release --runtime win-x64 --self-contained true -p:PublishAot=true` creates a Native AOT release build. Use `linux-x64` for Linux.

The runtime tool depends on `fzf` unless `--list-only` is used.

## Coding Style & Naming Conventions

Use C# with nullable reference types and implicit usings enabled. Keep namespaces under `LocalRepositoryListing`, with subnamespaces matching folders such as `LocalRepositoryListing.Searcher`. Use PascalCase for public types and methods, camelCase for locals and parameters, and interface names with the `I` prefix (`ISearcher`, `IResultLister`). Follow the existing four-space indentation style. Prefer async APIs and cancellation-token propagation for long-running search or process work.

## Testing Guidelines

There is currently no test project in the repository. For changes, at minimum run `dotnet build local-repository-listing.sln` and manually exercise list-only behavior against a small directory tree, for example:

```bash
dotnet run --project local-repository-listing/local-repository-listing.csproj -- --list-only --root .
```

If adding tests, create a dedicated test project such as `local-repository-listing.Tests/`, use descriptive names like `Lepol_ListOnly_PrintsRepositories`, and cover search filtering, non-recursive mode, and error handling.

## Commit & Pull Request Guidelines

Recent commits use short imperative or descriptive subjects such as `Update README` and `support for zsh`; keep subjects concise and focused. Pull requests should describe the behavior change, list manual verification commands, note platform impact when touching publish or shell behavior, and link related issues. Include terminal output or screenshots only when they clarify CLI or `fzf` behavior.

## Release & Configuration Notes

Version releases are driven by the `<Version>` value in `local-repository-listing/local-repository-listing.csproj`. The publish workflow builds Native AOT archives for Windows and Linux and includes `src/lepos.sh`, so keep release-facing script changes backward compatible.
