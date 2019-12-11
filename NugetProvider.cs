using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grab
{
    class VersionInfo { public string[] Versions { get; set; } }
    class NuGetProvider : IProvider
    {
        HttpClient Client;

        public NuGetProvider()
        {
            Client = new HttpClient { BaseAddress = new Uri(Constants.BASE_ADDRESS) };
        }

        public IEnumerable<Task<(string, bool, string)>> DownloadPackages(string[] packages)
        {
            var JsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, AllowTrailingCommas = true };

            return packages.Select(async package =>
            {
                var Splits = package.Split('@');
                string Name = Splits[0], Version = Splits.Length == 2 ? Splits[1] : null;

                var VersionResponse = await Client.GetAsync($"{Name}/index.json");

                if (!VersionResponse.IsSuccessStatusCode)
                    return (Name, false, Constants.PACKAGE_NOT_FOUND);

                var VersionsInfo = JsonSerializer.Deserialize<VersionInfo>(await VersionResponse.Content.ReadAsStringAsync(), JsonSerializerOptions);

                if (!string.IsNullOrWhiteSpace(Version) && !VersionsInfo.Versions.Any(v => v == Version))
                    return ($"{Name} @ {Version}", false, Constants.VERSION_NOT_FOUND);

                Version = Version ?? VersionsInfo.Versions.LastOrDefault();

                var PackageResponse = await Client.GetAsync($"{Constants.BASE_ADDRESS}{Name}/{Version}/{Name}.{Version}.nupkg");

                if (!PackageResponse.IsSuccessStatusCode)
                    return ($"{Name} @ {Version}", false, Constants.FAILED_TO_DOWNLOAD);

                var ExtractionPath = $"packages/{Name}/{Version}";
                Directory.CreateDirectory(ExtractionPath);
                var FilePath = Path.Combine(ExtractionPath, $"{Name}.nupkg");

                await File.WriteAllBytesAsync(FilePath, await PackageResponse.Content.ReadAsByteArrayAsync());
                ZipFile.ExtractToDirectory(FilePath, ExtractionPath, true);
                return ($"{Name} @ {Version}", true, Constants.DOWNLOADED);
            });
        }
    }
}