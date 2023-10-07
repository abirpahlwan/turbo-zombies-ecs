using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ZombieRiseAspect : IAspect
{
    public readonly Entity Entity;
    
    private readonly RefRW<LocalTransform> transformAspect;
    private readonly RefRO<ZombieRiseRate> zombieRiseRate;

    public void Rise(float deltaTime)
    {
        transformAspect.ValueRW.Position += math.up() * zombieRiseRate.ValueRO.Value * deltaTime;
    }
    
    public bool IsAboveGround => transformAspect.ValueRO.Position.y >= 0f;

    public void SetAtGroundLevel()
    {
        var position = transformAspect.ValueRO.Position;
        position.y = 0f;
        transformAspect.ValueRW.Position = position;
    }
}
