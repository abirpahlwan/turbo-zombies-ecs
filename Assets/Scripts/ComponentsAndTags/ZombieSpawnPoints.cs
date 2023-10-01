using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ZombieSpawnPoints : IComponentData
{
    public BlobAssetReference<ZombieSpawnPointsBlob> PointsBlobReference;
}

public struct ZombieSpawnPointsBlob
{
    public BlobArray<float3> BlobPointsArray;
}
