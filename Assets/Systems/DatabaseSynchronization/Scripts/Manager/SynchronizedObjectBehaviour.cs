using Atomix.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Atomix
{
    public class SynchronizedObjectBehaviour : MonoBehaviour
    {
        public DatabaseObjectData DatabaseObjectData { get; private set; }

        public void Init(DatabaseObjectData data)
        {
            DatabaseObjectData = data;

            name = data.Name;
            transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
        }
    }
}
