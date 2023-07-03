using System;
using UnityEngine;
using RandomUnity = UnityEngine.Random;

namespace MapGen.Random
{
    public class RandomManager : MonoBehaviour
    {
        public static RandomManager Instance { get; private set; }
        
        [SerializeField] private RandomSettings randomSettings;


        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            RandomUnity.InitState(randomSettings.GetSeed());  
        }
    }
}

