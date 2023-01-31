using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace UT.AlchemyVR
{
    public class VFXInfo
    {
        private readonly VisualEffect _vfx;
        public GameObject VFXObj { get; set; }

        public VFXInfo(VisualEffect vfx, GameObject vfxObj)
        {
            _vfx = vfx;
            VFXObj = vfxObj;
        }

        public void UpdatePosition(Vector3 position)
        {
            _vfx.SetInt("SpawnRate", 25);
            VFXObj.transform.position = position;
        }

        public void StopSpawn()
        {
            _vfx.SetInt("SpawnRate", 0);
        }

        public IEnumerator DestroyAfter(float time)
        {
            StopSpawn();
            yield return new WaitForSeconds(time);
            Object.Destroy(VFXObj);
        }
    }
}