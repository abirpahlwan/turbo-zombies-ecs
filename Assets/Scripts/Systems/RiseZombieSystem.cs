using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct RiseZombieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new RiseZombieJob
        {
            DeltaTime = deltaTime,
            ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged) // .AsParallelWriter()
        }.Run();
        // .ScheduleParallel();
    }
}

[BurstCompile]
public partial struct RiseZombieJob : IJobEntity
{
    public float DeltaTime;
    // public EntityCommandBuffer.ParallelWriter ecb;
    public EntityCommandBuffer ecb;

    [BurstCompile]
    // private void Execute(ZombieRiseAspect zombie, [EntityIndexInQuery]int sortKey)
    private void Execute(ZombieRiseAspect zombie)
    {
        zombie.Rise(DeltaTime);
        if (!zombie.IsAboveGround) return;
        
        zombie.SetAtGroundLevel();
        // ecb.RemoveComponent<ZombieRiseRate>(sortKey, zombie.Entity);
        ecb.RemoveComponent<ZombieRiseRate>(zombie.Entity);
    }
}
