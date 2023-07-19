using Atomix;
using Atomix.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SynchronizedObjectBehaviour pf_SomeObject1;
    public SynchronizedObjectBehaviour pf_SomeObject2;
    public SynchronizedObjectBehaviour pf_SomeObject3;

    private DatabaseObjectDataHandler _dataHandler;
    private List<SynchronizedObjectBehaviour> _objects = new List<SynchronizedObjectBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        _dataHandler = gameObject.AddComponent<DatabaseObjectDataHandler>();

        // On charge les données dans la base
        // J'ai utilisé mon serveur perso pour créér une nouvelle base avec une table, et trois scripts PHP pour gérer le READ/UPDATE des méthodes CRUD
        _dataHandler.LoadAll(
            (objectDatas) =>
            { 
                // On instancie
                for(int i = 0;i < objectDatas.Count;i++)
                {
                    SynchronizedObjectBehaviour instantiated = null;
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

                    // Initialisation de l'objet monob avec les données du backend
                    instantiated.Init(objectDatas[i]);
                    _objects.Add(instantiated);
                }
            },
            (error) => Debug.LogError($"Something went wrong. {error}"));

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 300, 30), "Déplacer les objets créés"))
        {
            for(int i = 0; i < _objects.Count;i++)
            {
                // On modifie la position aléatoirement
                _objects[i].transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
                // On met à jour l'objet de donnée
                _objects[i].DatabaseObjectData.PositionX = _objects[i].transform.position.x;
                _objects[i].DatabaseObjectData.PositionY = _objects[i].transform.position.y;
                _objects[i].DatabaseObjectData.PositionZ = _objects[i].transform.position.z;

                // On commit la modification au backend afin de pouvoir recharger les nouvelles données aux prochain lancement
                _dataHandler.CommitUpdate(_objects[i].DatabaseObjectData, (result) => Debug.Log("Object updated ?" + result)); 
            }
        }
    }

}
