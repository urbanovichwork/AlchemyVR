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

        private void Awake()
        {
            _waterCollisionInfos = new List<WaterCollisionInfo>();
        }
        private void OnTriggerEnter(Collider other)
        {
            var inter = other.gameObject.transform;
            var vfxObj = Instantiate(vfxPrefab, inter.position, Quaternion.identity);
            var vfx = vfxObj.GetComponent<VisualEffect>();
            var vfxInfo = new VFXInfo(vfx, vfxObj);
            var info = new WaterCollisionInfo(other.GetHashCode(), inter, vfxInfo, transform.position);
            _waterCollisionInfos.Add(info);
        }
        private void OnTriggerExit(Collider other)
        {
            var matchInfo = _waterCollisionInfos.Find(info => other.GetHashCode() == info.HashCode);
            if (matchInfo != null)
            {
                StartCoroutine(matchInfo.Info.DestroyAfter(2.5f));
                matchInfo.Info = null;
                _waterCollisionInfos.Remove(matchInfo);
            }
        }
        private void FixedUpdate()
        {
            foreach (WaterCollisionInfo info in _waterCollisionInfos)
            {
                info.UpdatePosition();
            }
        }
    }
}