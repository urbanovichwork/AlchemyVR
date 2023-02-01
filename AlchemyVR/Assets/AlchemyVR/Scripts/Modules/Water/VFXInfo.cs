using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace UT.AlchemyVR
{
    public class VFXInfo
    {
        private const string SpawnRate = "SpawnRate";
        private const int DefaultSpawnRateValue = 25;

        private readonly VisualEffect _vfx;
        public GameObject VFXObj { get; }

        public VFXInfo(VisualEffect vfx, GameObject vfxObj)
        {
            _vfx = vfx;
            VFXObj = vfxObj;
        }
        public void UpdatePosition(Vector3 position)
        {
            _vfx.SetInt(SpawnRate, DefaultSpawnRateValue);
            VFXObj.transform.position = position;
        }
        public void StopSpawn()
        {
            _vfx.SetInt(SpawnRate, 0);
        }
        public async void DestroyAfter(int time)
        {
            StopSpawn();
            await UniTask.Delay(time);
            Object.Destroy(VFXObj);
        }
    }
}