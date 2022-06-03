using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Pool : MonoBehaviour
    {
        public GameObject thing;
        private List<GameObject> things = new List<GameObject>();

        public void RemoveChild(GameObject go)
        {
            if (things.Contains(go))
            {
                things.Remove(go);
            }
        }
        
        public GameObject nextThing
        {
            get
            {  
                if (things.Count < 1 || things.TrueForAll(_ => _.activeSelf))
                {
                    GameObject newClone = (GameObject) Instantiate(thing, transform, true);
                    newClone.SetActive(false);  
                    things.Add(newClone);  
                    PoolMember poolMember = newClone.AddComponent<PoolMember>();  
                    poolMember.pool = this;
                }

                GameObject clone = null;
                for (int i = 0; i < things.Count; i++)
                {
                    if (!things[i].activeSelf)
                    {
                        clone = things[i];
                        things.RemoveAt(i);  
                        clone.SetActive(true);
                        break;
                    }
                }
                return clone;
            }  
            set
            {  
                value.SetActive(false);  
                things.Add(value);  
            }  
        }
    }
}