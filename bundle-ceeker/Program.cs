using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using BundleCeeker.proto;
using ConsoleApp1;
using Newtonsoft.Json;

namespace BundleCeeker;

internal class Program {

    private static JsonNode? PROGRESS = null;
    private static int lastKnownAssetVersion = -1;
    
    static void Main(string[] args) {
        Console.WriteLine("Hello World!");
        run().Wait();
    }

    private static async Task run() {
        while (true) {
            PROGRESS = GetProgress();
            var assetVersion = GetAssetVersion();
            if (assetVersion == -1) {
                Console.WriteLine("Failed to get asset version.");
                await Task.Delay(TimeSpan.FromSeconds(10));
                continue;
            }
            
            if (lastKnownAssetVersion == assetVersion) {
                Console.WriteLine("No new update was found.");
                await Task.Delay(TimeSpan.FromMinutes(1));
                continue;
            }

            lastKnownAssetVersion = assetVersion;
        
            var list = GetBundleAssetsFromManifest(assetVersion);
        
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText($"./Manifest/{assetVersion}/manifest.json", json);

            DownloadNewAssetsForVersion(assetVersion, list);

            // Wait for some time before polling again (e.g., every 5 minutes)
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }

    private static string GetAssetDownloadUrl(int assetVersion) {
        return $"https://eaassets-a.akamaihd.net/assetssw.capitalgames.com/PROD/{assetVersion}/Android/ETC";
    }

    private static JsonNode? GetProgress() {
        Directory.CreateDirectory("./bundles");
        
        if (!File.Exists("./bundles/progress.json")) {
            var initialDocument = JsonNode.Parse("{}");
            File.WriteAllText("./bundles/progress.json", initialDocument?.ToJsonString());
        }

        var jsonText = File.ReadAllText("./bundles/progress.json");
        var document = JsonNode.Parse(jsonText);
        return document;
    }

    private static void DownloadNewAssetsForVersion(int assetVersion, List<RawAssetManifestRecord> list) {
        var versions = new List<string>();
        var versionsArray = PROGRESS?["versions"]?.AsArray();
        for (var i = 0; i < versionsArray?.Count; i++) {
            var jsonNode = versionsArray[i];
            versions.Add(jsonNode?.ToString());
        }
        if (!versions.Contains(assetVersion.ToString()))
            versions.Add(assetVersion.ToString());

        var currentVersionObject = PROGRESS?[assetVersion.ToString()];
        if (currentVersionObject == null) {
            PROGRESS[assetVersion.ToString()] = new JsonObject();
            PROGRESS[assetVersion.ToString()]["created_at"] = JsonValue.Create(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
        
        var ignoreList = new List<string>();
        var assetList = new List<string>();
        foreach (var version in versions) {
            var jsonArray = PROGRESS?[version]?["assets"]?.AsArray();
            for (var i = 0; i < jsonArray?.Count; i++) {
                var jsonNode = jsonArray[i];
                var assetName = jsonNode?.ToString();
                
                ignoreList.Add(assetName);
                
                if (version == assetVersion.ToString()) {
                    assetList.Add(assetName);
                }
            }
        }
        
        foreach (var bundle in list) {
            if (ignoreList.Contains(bundle.Name) || assetList.Contains(bundle.Name))
                continue;
            
            assetList.Add(bundle.Name);
            DownloadAssetBundle(assetVersion, bundle.Name);
        }

        PROGRESS[$"{assetVersion}"]["assets"] = JsonValue.Create(assetList);
        PROGRESS["versions"] = JsonValue.Create(versions);
        File.WriteAllText(
            "./bundles/progress.json", 
            PROGRESS.ToJsonString(new JsonSerializerOptions { WriteIndented = true })
        );
    }

    private static int GetAssetVersion() {
        try {
            using (var client = new WebClient()) {
                client.DownloadFile(
                    "https://raw.githubusercontent.com/swgoh-utils/gamedata/main/allVersions.json",
                    "./allVersions.json"
                );
            }

            var jsonText = File.ReadAllText("./allVersions.json");

            var jsonDoc = JsonDocument.Parse(jsonText);
            var root = jsonDoc.RootElement;

            var assetVersionElement = root.GetProperty("assetVersion");
            var assetVersion = assetVersionElement.GetInt32();

            return assetVersion;
        }
        catch (Exception e) {
            return -1;
        }
    }

    private static void DownloadAssetBundle(int assetVersion, string assetBundleName) {
        try {
            Console.WriteLine($"Downloading {assetBundleName}");
            Directory.CreateDirectory($"./bundles");
            Directory.CreateDirectory($"./bundles/{assetVersion}");

            var pathToNewFile = $"./bundles/{assetVersion}/{assetBundleName}.bundle";
            using (var client = new WebClient()) {
                client.DownloadFile(
                    $"{GetAssetDownloadUrl(assetVersion)}/{assetBundleName}.bundle", 
                    pathToNewFile
                );
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error downloading file '{assetBundleName}'! you may ignore this.");
            Console.WriteLine(ex);
        }
    }

    public static void DownloadManifest(int assetVersion) {
        Directory.CreateDirectory($"./Manifest/{assetVersion}");

        using (var client = new WebClient()) {
            Console.WriteLine($"Downloading Manifest");
            client.DownloadFile(
                $"{GetAssetDownloadUrl(assetVersion)}/manifest.data",
                $"./Manifest/{assetVersion}/manifest.data"
            );
        }
    }

    public static string GetPathToManifestAndDownloadIfNotExists(int assetVersion) {
        var pathToManifest = $"./Manifest/{assetVersion}/manifest.data";

        if (!File.Exists(pathToManifest)) {
            DownloadManifest(assetVersion);
        }

        return pathToManifest;
    }

    public static List<RawAssetManifestRecord> GetBundleAssetsFromManifest(int assetVersion) {
        var pathToManifest = GetPathToManifestAndDownloadIfNotExists(assetVersion);
        var manifestHelper = new ManifestHelper();
        manifestHelper.ReadFromFile(pathToManifest);
        return manifestHelper.BundleFiles;
    }
}