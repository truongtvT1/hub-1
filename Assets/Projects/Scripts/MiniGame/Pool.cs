using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Pool : MonoBehaviour
    {
        public GameObject thing;
        private List<GameObject> things = new List<GameObject>();

        public GameObject nextThing
        {
            get
            {  
                if (things.Count < 1)
                {
                    GameObject newClone = (GameObject) Instantiate(thing, transform, true);  
                    newClone.SetActive(false);  
                    things.Add(newClone);  
                    PoolMember poolMember = newClone.AddComponent<PoolMember>();  
                    poolMember.pool = this;  
                }  
                GameObject clone = things[0];  
                things.RemoveAt(0);  
                clone.SetActive(true);  
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