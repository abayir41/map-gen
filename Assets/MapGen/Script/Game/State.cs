using UnityEngine;

namespace MapGen
{
    public abstract class State : MonoBehaviour
    {
        public abstract void OnStateEnter();
        public abstract void OnStateExit();
    }
}