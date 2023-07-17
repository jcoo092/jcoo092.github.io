using System.Globalization;

// Ensure consistent date handling
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-NZ");

return await Bootstrapper
    .Factory
    .CreateWeb(args)
    .DeployToGitHubPages(
        "jcoo092",
        "jcoo092.github.io",
        Config.FromSetting<string>("GITHUB_TOKEN")
    )
    .RunAsync();