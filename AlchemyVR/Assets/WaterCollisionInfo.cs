using UnityEngine;

namespace UT.AlchemyVR
{

    public class WaterCollisionInfo
    {
        private readonly Transform _follow;
        private readonly Vector3 _originPos;
        private Vector3 oldPos;
        public int HashCode { get; }
        public VFXInfo Info { get; set; }

        public WaterCollisionInfo(int hashCode, Transform follow, VFXInfo vfxInfo, Vector3 originPos)
        {
            HashCode = hashCode;
            _follow = follow;
            Info = vfxInfo;
            _originPos = originPos;

            oldPos = _originPos;
        }
        public void UpdatePosition()
        {
            var followPos = _follow.position;
            if (Vector3.Distance(oldPos, followPos) > 0.1f)
            {
                var newPos = new Vector3(followPos.x, _originPos.y, followPos.z);
                Info.UpdatePosition(newPos);
                oldPos = followPos;
            }
            else
            {
                Info.StopSpawn();
            }

        }
    }
}