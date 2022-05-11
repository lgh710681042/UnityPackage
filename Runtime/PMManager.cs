
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace MyCommandLine
{
    public class PMManager
    {
        private static PMManager m_instance = null;
        
        public static PMManager GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new PMManager();
            }
            
            return m_instance;
        }

        public void LoadRes(string AddressNameStr)
        {
            Addressables.LoadAssetAsync<GameObject>(AddressNameStr).Completed += OnAssetObjLoaded;
        }

        void OnAssetObjLoaded(AsyncOperationHandle<GameObject> asyncOperationHandle)
        {
            GameObject assetObj = asyncOperationHandle.Result;
            Vector3 pos = new Vector3(0, 0, 0);

            if (assetObj != null)
            {
                GameObject.Instantiate(assetObj, pos, Quaternion.identity);
            }
        }
    }
}
