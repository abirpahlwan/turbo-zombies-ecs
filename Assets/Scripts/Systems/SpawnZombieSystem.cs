using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct SpawnZombieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        
        new SpawnZombieJob
        {
            DeltaTime = deltaTime,
            ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
        }.Run();
    }
}

[BurstCompile]
public partial struct SpawnZombieJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer ecb;
    
    [BurstCompile]
    private void Execute(GraveyardAspect graveyard)
    {
        graveyard.ZombieSpawnTimer -= DeltaTime;
        if (!graveyard.TimeToSpawnZombie) return;
        if (!graveyard.ZombieSpawnPointInitialized()) return;

        graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
        var newZombie = ecb.Instantiate(graveyard.ZombiePrefab);

        var newZombieTransform = graveyard.GetZombieSpawnPoint();
        ecb.SetComponent(newZombie, newZombieTransform);
    }
}
