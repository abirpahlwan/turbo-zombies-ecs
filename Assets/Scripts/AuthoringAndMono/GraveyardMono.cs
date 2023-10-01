using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GraveyardMono : MonoBehaviour
{
    public float2 FieldDimensions;
    public int NumberOfTombstones;
    public GameObject TombstonePrefab;
    public uint RandomSeed;
    
    public GameObject ZombiePrefab;
    public float ZombieSpawnRate;
}

public class GraveyardBaker : Baker<GraveyardMono>
{
    public override void Bake(GraveyardMono authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
            
        AddComponent(entity, new GraveyardProperties {
            FieldDimensions = authoring.FieldDimensions,
            NumberOfTombstones = authoring.NumberOfTombstones,
            TombstonePrefab = GetEntity(authoring.TombstonePrefab, TransformUsageFlags.Dynamic),
            ZombiePrefab = GetEntity(authoring.ZombiePrefab),
            ZombieSpawnRate = authoring.ZombieSpawnRate
        });

        AddComponent(entity, new GraveyardRandom {
            Value = Random.CreateFromIndex(authoring.RandomSeed)
        });

        AddComponent<ZombieSpawnPoints>(entity);
        AddComponent<ZombieSpawnTimer>(entity);

        /*AddComponent(new GraveyardProperties {
            FieldDimentions = authoring.FieldDimentions,
            NumberOfTombstones = authoring.NumberOfTombstones,
            TombstonePrefab = GetEntity(authoring.TombstonePrefab)
        });*/
    }
}
