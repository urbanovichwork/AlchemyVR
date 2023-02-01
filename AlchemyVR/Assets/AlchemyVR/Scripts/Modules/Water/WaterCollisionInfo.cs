using UnityEngine;

namespace UT.AlchemyVR
{
    public class WaterCollisionInfo
    {
        private readonly Transform _follow;
        private readonly Vector3 _originPos;
        
        private Vector3 _oldPos;
        private bool _isPosChanged;
        
        public int HashCode { get; }
        public VFXInfo Info { get; set; }

        public WaterCollisionInfo(int hashCode, Transform follow, VFXInfo vfxInfo, Vector3 originPos)
        {
            HashCode = hashCode;
            _follow = follow;
            Info = vfxInfo;
            _originPos = originPos;

            _oldPos = _originPos;
        }
        public void UpdatePosition()
        {
            var followPos = _follow.position;
            if (Vector3.Distance(_oldPos, followPos) > 0.1f)
            {
                var newPos = new Vector3(followPos.x, _originPos.y, followPos.z);
                Info.UpdatePosition(newPos);
                _oldPos = followPos;
                _isPosChanged = true;
            }
            else
            {
                if (_isPosChanged)
                {
                    Info.StopSpawn();
                    _isPosChanged = false;
                }
            }
        }
    }
}