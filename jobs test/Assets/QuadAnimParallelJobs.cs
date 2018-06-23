using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct JobParallelFor : IJobParallelFor
{
    public Vector3 destination;
    public NativeArray<Vector3> positions;
    public NativeArray<Quaternion> rotations;
    public float deltaTime;

    public void Execute(int i)
    {
        Vector3 vToDest = destination - positions[i];
        float distToDest = vToDest.magnitude;
        float distThisFrame = (deltaTime / distToDest) * 10;

        if (distToDest > distThisFrame)
            positions[i] += (vToDest / distToDest) * distThisFrame;

        rotations[i] *= Quaternion.AngleAxis(deltaTime * 200, Vector3.forward);
    }
}

public class QuadAnimParallelJobs : MonoBehaviour
{
    public GameObject prefab;
    private List<Transform> transforms = new List<Transform>();
    private Vector3 destination = new Vector3(40, 0, 5);
    private const int quantity = 1000;
    private NativeArray<Vector3> positions;
    private NativeArray<Quaternion> rotations;
    private JobParallelFor job = new JobParallelFor();

    private void Start()
    {
        positions = new NativeArray<Vector3>(quantity, Allocator.Persistent);
        rotations = new NativeArray<Quaternion>(quantity, Allocator.Persistent);

        for (int i = 0; i < quantity; i++)
        {
            Vector3 pos = new Vector3();
            pos.x = Random.Range(-20, 10);
            pos.y = Random.Range(-12, 12);
            pos.z = Random.Range(0, 40);
            positions[i] = pos;

            rotations[i] = Quaternion.identity;

            Transform t = Instantiate(prefab).transform;
            transforms.Add(t);
        }

        job.positions = positions;
        job.rotations = rotations;
        job.destination = destination;
    }

    private void OnDestroy()
    {
        positions.Dispose();
        rotations.Dispose();
    }

    private void Update()
    {
        job.deltaTime = Time.deltaTime;
        JobHandle handle = job.Schedule(quantity, 1);
        handle.Complete();

        for (int i = 0; i < quantity; i++)
        {
            transforms[i].position = positions[i];
            transforms[i].rotation = rotations[i];
        }
    }
}
