using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public readonly partial struct GraveyardAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<LocalTransform> transformAspect;
    private readonly RefRO<GraveyardProperties> graveyardProperties;
    private readonly RefRW<GraveyardRandom> graveyardRandom;

    public int NumberOfTombstonesToSpawn => graveyardProperties.ValueRO.NumberOfTombstones;
    public Entity TombstonePrefab => graveyardProperties.ValueRO.TombstonePrefab;

    public LocalTransform GetRandomTombstoneTransform()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(transformAspect.ValueRO.Position, randomPosition) < BRAIN_SAFETY_RADIUS_SQ);
        
        return randomPosition;
    }
    
    private float3 MinCorner => transformAspect.ValueRO.Position - HalfDimensions;
    private float3 MaxCorner => transformAspect.ValueRO.Position + HalfDimensions;
    private float3 HalfDimensions => new()
    {
        x = graveyardProperties.ValueRO.FieldDimensions.x / 2,
        y = 0f,
        z = graveyardProperties.ValueRO.FieldDimensions.y / 2
    };
    
    private const float BRAIN_SAFETY_RADIUS_SQ = 100;
    
    private quaternion GetRandomRotation()
    {
        var x = quaternion.RotateX(graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
        var y = quaternion.RotateY(graveyardRandom.ValueRW.Value.NextFloat(-1f, 1f));
        var z = quaternion.RotateZ(graveyardRandom.ValueRW.Value.NextFloat(-0.45f, 0.45f));
        
        var randomRotation = math.mul(z, math.mul(y, x));
        return randomRotation;
    }

    private float GetRandomScale(float min) => graveyardRandom.ValueRW.Value.NextFloat(min, 1f);
    
}
