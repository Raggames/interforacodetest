using Atomix.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pf_SomeObject1;
    public GameObject pf_SomeObject2;
    public GameObject pf_SomeObject3;

    private DatabaseObjectDataHandler _dataHandler;

    // Start is called before the first frame update
    void Start()
    {
        _dataHandler = gameObject.AddComponent<DatabaseObjectDataHandler>();

        _dataHandler.LoadAll(
            (objectDatas) =>
            { 
                for(int i = 0;i < objectDatas.Count;i++)
                {
                    GameObject instantiated = null;
                    switch (objectDatas[i].Type)
                    {
                        case "SomeObject1":
                            instantiated = Instantiate(pf_SomeObject1, this.transform);
                            break;
                        case "SomeObject2":
                            instantiated = Instantiate(pf_SomeObject2, this.transform);
                            break;
                        case "SomeObject3":
                            instantiated = Instantiate(pf_SomeObject3, this.transform);
                            break;
                    }

                    instantiated.name = objectDatas[i].Name;
                    instantiated.transform.position = new Vector3(objectDatas[i].PositionX, objectDatas[i].PositionY, objectDatas[i].PositionZ);

                }
            },
            (error) => Debug.LogError($"Something went wrong. {error}"));

    }

    public void InitializeDatas()
    {

    }

}
