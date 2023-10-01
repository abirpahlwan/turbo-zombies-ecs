using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GraveyardProperties>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
        var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tombstoneOffset = new float3(0f, -2f, 1f);
        
        // create BlobBulder object
        var builder = new BlobBuilder(Allocator.Temp);
        // use builder to construct a root array based on the type
        ref var spawnPoints = ref builder.ConstructRoot<ZombieSpawnPointsBlob>();
        // allocate memory based on length
        var arrayBuilder = builder.Allocate(ref spawnPoints.BlobPointsArray, graveyard.NumberOfTombstonesToSpawn);

        for (int i = 0; i < graveyard.NumberOfTombstonesToSpawn; i++)
        {
            var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);
            var newTombstoneTransform = graveyard.GetRandomTombstoneTransform();
            ecb.SetComponent(newTombstone, newTombstoneTransform);

            var newZombieSpawnPoint = newTombstoneTransform.Position + tombstoneOffset;
            arrayBuilder[i] = newZombieSpawnPoint;
        }

        // get the reference of array we created
        var blobAssetReference = builder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
        // pass the reference to component class, and set it to graveyard entity 
        ecb.SetComponent(graveyardEntity, new ZombieSpawnPoints{ PointsBlobReference = blobAssetReference });
        builder.Dispose();
        
        ecb.Playback(state.EntityManager);
    }
}
