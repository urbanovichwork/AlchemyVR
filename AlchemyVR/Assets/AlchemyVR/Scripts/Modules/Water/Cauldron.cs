using UnityEngine;
using VContainer.Unity;

namespace UT.AlchemyVR
{
    public class Cauldron : MonoBehaviour, IInitializable
    {
        [SerializeField]
        private WaterCollision waterCollision;
        public void Initialize()
        {
            waterCollision.Initialize();
        }
    }
}