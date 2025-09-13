using ConsoleAppFramework;
using LocalRepositoryListing;

[assembly: ZLinq.ZLinqDropIn("ZLinq", ZLinq.DropInGenerateTypes.Collection)]

var app = ConsoleApp.Create();
app.Add<LocalRepositoryListingCommand>();

await app.RunAsync(args);
