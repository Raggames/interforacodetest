

using System;
using System.Collections.Generic;

namespace Atomix.Backend.Models
{
    [Serializable]
    public class DatabaseObjectData : IBackendData
    {
        /// <summary>
        /// Database primary key
        /// </summary>
        public int ID { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public string Data { get; set; }

    }

    public class DatabaseObjectDataList : DatabaseObjectData
    {
        public List<DatabaseObjectData> list;
    }
}
