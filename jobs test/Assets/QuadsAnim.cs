using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadsAnim : MonoBehaviour
{
    public GameObject prefab;
    private List<Transform> transforms = new List<Transform>();
    private Vector3 destination = new Vector3(40, 0, 5);
    private const int quantity = 1000;

    private void Start ()
    {
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
    }

    private void Update ()
    {
        for (int i = 0; i < quantity; i++)
        {
            Transform t = transforms[i];
            Vector3 vToDest = destination - t.position;
            float distToDest = vToDest.magnitude;
            float distThisFrame = (Time.deltaTime / distToDest) * 10;

            if (distToDest > distThisFrame)
                t.position += (vToDest / distToDest) * distThisFrame;

            t.Rotate(Vector3.forward, Time.deltaTime * 200);
        }
	}
}
