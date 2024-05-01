using BundleCeeker.proto;
using ProtoBuf;

namespace ConsoleApp1;

public class ManifestHelper {
    public RawAssetManifest rawAssetManifest { get; set; }

    public List<string> audioFiles { get; set; }
    public List<string> prefixes { get; set; }
    public List<string> resources { get; set; }

    public List<RawAssetManifestRecord> BundleFiles { get; set; }
    public List<RawAssetManifestRecord> AudioFiles { get; set; }

    public ManifestHelper() {
        this.audioFiles = new List<string>();
        this.prefixes = new List<string>();
        this.resources = new List<string>();

        this.BundleFiles = new List<RawAssetManifestRecord>();
        this.AudioFiles = new List<RawAssetManifestRecord>();
    }

    public void ReadFromFile(string filepath) {
        using (FileStream inFile = File.OpenRead(filepath)) {
            rawAssetManifest = Serializer.Deserialize<RawAssetManifest>(inFile);
        }

        BundleFiles = this.rawAssetManifest.Records.Where(record => record.packageType == 0).ToList();
        AudioFiles = this.rawAssetManifest.Records
            .Where(record => record.packageType == 1 && record.Name != "soundbanksinfo").ToList();

        //backwards compatibility
        var audioFileNames = AudioFiles.Select(audio => audio.Name).ToList();
        this.audioFiles.AddRange(audioFileNames);

        var bundleFileNames = BundleFiles.Select(audio => audio.Name).ToList();
        this.resources.AddRange(bundleFileNames);

        this.audioFiles = audioFiles.Distinct().ToList();
        this.resources = resources.Distinct().ToList();

        foreach (var resource in this.resources) {
            var resourcePrefix = resource.Split('_');
            if (resourcePrefix.Length > 1) {
                this.prefixes.Add(resourcePrefix.First());
            }
        }

        this.prefixes = prefixes.Distinct().ToList();

        audioFiles.Sort();
        prefixes.Sort();
        resources.Sort();
    }
}