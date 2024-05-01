namespace BundleCeeker.proto; 

[ProtoBuf.ProtoContract()]
public class RawAssetManifestRecordEntry : ProtoBuf.IExtensible {
    
    private ProtoBuf.IExtension __pbn__extensionData;
    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [ProtoBuf.ProtoMember(1, Name = @"asset_name")]
    [System.ComponentModel.DefaultValue(null)]
    public string AssetName { get; set; }

    [ProtoBuf.ProtoMember(2, Name = @"runtime_size")]
    public int RuntimeSize { get; set; }

    [ProtoBuf.ProtoMember(3, Name = @"clone_runtime_size")]
    public int CloneRuntimeSize { get; set; }
}