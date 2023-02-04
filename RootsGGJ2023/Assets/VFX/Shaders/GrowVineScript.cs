using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowVineScript : MonoBehaviour
{
    public List<MeshRenderer> gorwVinesMeshes;
    public float timeToGrow = 5;
    public float refreshRate = 0.05f;
    [Range(0,1)]
    public float minGrow = 0.2f;
    [Range(0,1)]
    public float maxGrow = 0.97f;

    private List<Material> growVinesMaterials = new List<Material>();
    private bool fullyGrown;

    void Start()
    {
        for(int i=0; i<growVinesMeshes.Count; i++)
        {
            for(int j=0; j<growVinesMeshes[i].materials.Lenght; j++)
            {
                if(growVinesMeshes[i].materials[j].Hasproperty("Grow_"))
                {
                    growVinesMeshes[i].materials[j].SetFloat("Grow_", minGrow);
                    growVinesMaterials.Add(gorwVinesMeshes[i].materials[j]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i=0;i<growVinesMaterials.Count; i++)
            {
                StartCoroutine(GrowVines(growVinesMaterials[i]));
            }
        }
    }

    IEnumerator GrowVines (Material mat)
    {
        float growValue = mat.GetFloat("Grow_");

        if(!fullyGrown)
        {
            while(growValue < maxGrow)
            {
                growValue += 1 / (timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                growValue -= 1 / (timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (growValue >= maxGrow)
            fullyGrown = true;
        else
            fullyGrown = false;
    }
}
