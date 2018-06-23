using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public struct JobParallelForTransform : IJobParallelForTransform
{
    public Vector3 destination;
    public float deltaTime;

    public void Execute(int i, TransformAccess ta)
    {
        Vector3 vToDest = destination - ta.position;
        float distToDest = vToDest.magnitude;
        float distThisFrame = (deltaTime / distToDest) * 10;

        if (distToDest > distThisFrame)
            ta.position += (vToDest / distToDest) * distThisFrame;

        ta.rotation *= Quaternion.AngleAxis(deltaTime * 200, Vector3.forward);
    }
}

public class QuadAnimParallelTransJobs : MonoBehaviour
{
    public GameObject prefab;
    private TransformAccessArray transforms;
    private Vector3 destination = new Vector3(40, 0, 5);
    private const int quantity = 1000;
    private JobParallelForTransform job = new JobParallelForTransform();

    private void Start()
    {
        transforms = new TransformAccessArray(quantity);

        for (int i = 0; i < quantity; i++)
        {
            Vector3 pos = new Vector3();
            pos.x = Random.Range(-20, 10);
            pos.y = Random.Range(-12, 12);
            pos.z = Random.Range(0, 40);

            Transform t = Instantiate(prefab).transform;
            t.position = pos;
            transforms.Add(t);
        }

        job.destination = destination;
    }

    private void OnDestroy()
    {
        transforms.Dispose();
    }

    private void Update()
    {
        job.deltaTime = Time.deltaTime;
        JobHandle handle = job.Schedule(transforms);
        handle.Complete();
    }
}
