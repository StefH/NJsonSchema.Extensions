rem https://github.com/StefH/GitHubReleaseNotes

rem --skip-empty-releases

SET version=0.0.1

GitHubReleaseNotes --output ReleaseNotes.md --exclude-labels question invalid doc --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --exclude-labels question invalid doc --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%