using UnityEngine;

namespace UT.AlchemyVR
{
    public class WaterCollision : MonoBehaviour
    {
        [SerializeField]
        private GameObject vfxPrefab;

        private GameObject vfx;
        private Transform inter;

        private void OnTriggerEnter(Collider other)
        {
            inter = other.gameObject.transform;
            vfx = Instantiate(vfxPrefab, inter.position, Quaternion.identity);
        }
        private void OnTriggerExit(Collider other)
        {
            inter = null;
            Destroy(vfx);
            vfx = null;
        }
        private void FixedUpdate()
        {
            if (inter != null)
            {
                var position = new Vector3(inter.position.x, transform.position.y, inter.position.z);
                vfx.transform.position = position;
            }
        }
    }
}