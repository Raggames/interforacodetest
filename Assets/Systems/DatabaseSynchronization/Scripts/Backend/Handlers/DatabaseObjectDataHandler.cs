using Atomix.Backend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix.Backend
{
    public class DatabaseObjectDataHandler : BackendDataHandler<DatabaseObjectData>
    {
        public override void LoadAll(Action<List<DatabaseObjectData>> onDoneCallback, Action<string> onFailedCallback)
        {
            StartCoroutine(GetRequest("LoadAll", null, (jsonResult) => {

                DatabaseObjectDataList databaseObjectDataList = JsonConvert.DeserializeObject<DatabaseObjectDataList>(jsonResult);
                onDoneCallback.Invoke(databaseObjectDataList.list);
                Datas = databaseObjectDataList.list;
            }, 
            onFailedCallback));
        }
    }
}
