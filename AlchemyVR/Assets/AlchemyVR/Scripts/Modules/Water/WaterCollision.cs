using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace UT.AlchemyVR
{
    public class WaterCollision : MonoBehaviour
    {
        [SerializeField]
        private GameObject vfxPrefab;

        private List<WaterCollisionInfo> _waterCollisionInfos;

        public void Initialize()
        {
            _waterCollisionInfos = new List<WaterCollisionInfo>(4);
        }
        private void OnTriggerEnter(Collider other)
        {
            var otherTr = other.gameObject.transform;
            VFXInfo vfxInfo = CreateVFX(otherTr.position);
            var waterCollisionInfo = new WaterCollisionInfo(other.GetHashCode(), otherTr, vfxInfo, transform.position);
            _waterCollisionInfos.Add(waterCollisionInfo);
        }
        private void OnTriggerExit(Collider other)
        {
            var matchedWaterCollisionInfo = _waterCollisionInfos.Find(info => other.GetHashCode() == info.HashCode);
            if (matchedWaterCollisionInfo != null)
            {
                matchedWaterCollisionInfo.Info.DestroyAfter(2500);
                matchedWaterCollisionInfo.Info = null;
                _waterCollisionInfos.Remove(matchedWaterCollisionInfo);
            }
        }
        private void FixedUpdate()
        {
            UpdateWaterCollisionInfo();
        }
        private void UpdateWaterCollisionInfo()
        {
            if (_waterCollisionInfos != null)
            {
                foreach (WaterCollisionInfo info in _waterCollisionInfos)
                {
                    info.UpdatePosition();
                }
            }
        }

        // TODO: Create pool of VFX
        private VFXInfo CreateVFX(Vector3 pos)
        {
            var vfxObj = Instantiate(vfxPrefab, pos, Quaternion.identity);
            var vfx = vfxObj.GetComponent<VisualEffect>();
            var vfxInfo = new VFXInfo(vfx, vfxObj);
            return vfxInfo;
        }
    }
}