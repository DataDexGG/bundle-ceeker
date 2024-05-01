namespace BundleCeeker.proto; 

[ProtoBuf.ProtoContract]
public class RawAssetManifestRecord : ProtoBuf.IExtensible {
    
    private ProtoBuf.IExtension __pbn__extensionData;
    ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [ProtoBuf.ProtoMember(1, Name = @"name")]
    [System.ComponentModel.DefaultValue(null)]
    public string Name { get; set; }

    [ProtoBuf.ProtoMember(2, Name = @"version")]
    public ulong Version { get; set; }

    [ProtoBuf.ProtoMember(3, Name = @"size")]
    public int Size { get; set; }

    [ProtoBuf.ProtoMember(4, Name = @"uncompressed_size")]
    public int UncompressedSize { get; set; }

    [ProtoBuf.ProtoMember(5, Name = @"shared")]
    public bool Shared { get; set; }

    [ProtoBuf.ProtoMember(6, Name = @"rank")]
    public int Rank { get; set; }

    [ProtoBuf.ProtoMember(7)]
    public int packageType { get; set; }

    [ProtoBuf.ProtoMember(8, Name = @"entries")]
    public List<RawAssetManifestRecordEntry> Entries { get; } = new();

    [ProtoBuf.ProtoMember(9, Name = @"dependencies")]
    public List<string> Dependencies { get; } = new();

    [ProtoBuf.ProtoMember(10, Name = @"crc")]
    public uint Crc { get; set; }
}