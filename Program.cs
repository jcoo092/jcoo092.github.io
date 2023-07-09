return await Bootstrapper
    .Factory
    .CreateWeb(args)
    .DeployToGitHubPages(
        "jcoo092",
        "jcoo092.github.io",
        Config.FromSetting<string>("GITHUB_TOKEN")
    )
    .RunAsync();