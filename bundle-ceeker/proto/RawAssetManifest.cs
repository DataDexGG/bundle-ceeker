namespace BundleCeeker.proto; 

[ProtoBuf.ProtoContract]
public class RawAssetManifest : ProtoBuf.IExtensible {
    
    private ProtoBuf.IExtension __pbn__extensionData;
    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [ProtoBuf.ProtoMember(1, Name = @"version")]
    public int Version { get; set; }

    [ProtoBuf.ProtoMember(2, Name = @"records")]
    public List<RawAssetManifestRecord> Records { get; } = new();

    [ProtoBuf.ProtoMember(3, Name = @"platform")]
    [System.ComponentModel.DefaultValue(null)]
    public string Platform { get; set; }

    [ProtoBuf.ProtoMember(4, Name = @"tex_format")]
    [System.ComponentModel.DefaultValue(null)]
    public string TexFormat { get; set; }

    [ProtoBuf.ProtoMember(5, Name = @"environment")]
    [System.ComponentModel.DefaultValue(null)]
    public string Environment { get; set; }

    [ProtoBuf.ProtoMember(6, Name = @"timestamp")]
    public ulong Timestamp { get; set; }

    [ProtoBuf.ProtoMember(7, Name = @"revision")]
    public ulong Revision { get; set; }
}