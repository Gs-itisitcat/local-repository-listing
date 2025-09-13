using ConsoleAppFramework;
using LocalRepositoryListing;

var app = ConsoleApp.Create();
app.Add<LocalRepositoryListingCommand>();

await app.RunAsync(args);
