using UnityEngine;

namespace UT.AlchemyVR
{
    [ExecuteInEditMode]
    public class Liquid : MonoBehaviour
    {
        public enum UpdateMode
        {
            Normal,
            UnscaledTime
        }

        private static readonly int _wobbleX = Shader.PropertyToID("_WobbleX");
        private static readonly int _wobbleZ = Shader.PropertyToID("_WobbleZ");
        private static readonly int _fillAmount = Shader.PropertyToID("_FillAmount");

        [SerializeField]
        private UpdateMode updateMode;

        [SerializeField]
        private float maxWobble = 0.03f;
        [SerializeField]
        private float wobbleSpeedMove = 1f;
        [SerializeField]
        private float fillAmount = 0.5f;
        [SerializeField]
        private float recovery = 1f;
        [SerializeField]
        private float thickness = 1f;
        [SerializeField]
        [Range(0, 1)]
        private float compensateShapeAmount;
        [SerializeField]
        Mesh mesh;
        [SerializeField]
        Renderer rend;

        private Vector3 _pos;
        private Vector3 _lastPos;
        private Vector3 _velocity;
        private Quaternion _lastRot;
        private Vector3 _angularVelocity;
        private float _wobbleAmountX;
        private float _wobbleAmountZ;
        private float _wobbleAmountToAddX;
        private float _wobbleAmountToAddZ;
        private float _pulse;
        private float _sineWave;
        private float _time = 0.5f;
        private Vector3 _comp;
        private MaterialPropertyBlock _propertyBlock;

        private void Start()
        {
            InitLiquid();
        }
        private void OnValidate()
        {
            InitLiquid();
        }
        private void InitLiquid()
        {
            _propertyBlock = new MaterialPropertyBlock();
            GetMeshAndRend();
        }
        private void GetMeshAndRend()
        {
            if (mesh == null)
            {
                mesh = GetComponent<MeshFilter>().sharedMesh;
            }
            if (rend == null)
            {
                rend = GetComponent<Renderer>();
            }
        }
        private void Update()
        {
            float deltaTime = 0;
            switch (updateMode)
            {
                case UpdateMode.Normal:
                    deltaTime = Time.deltaTime;
                    break;
                case UpdateMode.UnscaledTime:
                    deltaTime = Time.unscaledDeltaTime;
                    break;
            }
            _time += deltaTime;

            if (deltaTime != 0)
            {
                // decrease wobble over time
                _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, (deltaTime * recovery));
                _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, (deltaTime * recovery));

                // make a sine wave of the decreasing wobble
                _pulse = 2 * Mathf.PI * wobbleSpeedMove;
                _sineWave = Mathf.Lerp(_sineWave, Mathf.Sin(_pulse * _time), deltaTime * Mathf.Clamp(_velocity.magnitude + _angularVelocity.magnitude, thickness, 10));

                _wobbleAmountX = _wobbleAmountToAddX * _sineWave;
                _wobbleAmountZ = _wobbleAmountToAddZ * _sineWave;

                // velocity
                _velocity = (_lastPos - transform.position) / deltaTime;
                _angularVelocity = GetAngularVelocity(_lastRot, transform.rotation);

                // add clamped velocity to wobble
                _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_velocity.y * 0.2f) + _angularVelocity.z + _angularVelocity.y) * maxWobble, -maxWobble, maxWobble);
                _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_velocity.y * 0.2f) + _angularVelocity.x + _angularVelocity.y) * maxWobble, -maxWobble, maxWobble);
            }

            // send it to the shader
            rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_wobbleX, _wobbleAmountX);
            _propertyBlock.SetFloat(_wobbleZ, _wobbleAmountZ);
            rend.SetPropertyBlock(_propertyBlock);

            // set fill amount
            UpdatePos(deltaTime);

            // keep last position
            var curTransform = transform;
            _lastPos = curTransform.position;
            _lastRot = curTransform.rotation;
        }

        private void UpdatePos(float deltaTime)
        {
            Vector3 worldPos = transform.TransformPoint(new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z));
            if (compensateShapeAmount > 0)
            {
                // only lerp if not paused/normal update
                if (deltaTime != 0)
                {
                    _comp = Vector3.Lerp(_comp, (worldPos - new Vector3(0, GetLowestPoint(), 0)), deltaTime * 10);
                }
                else
                {
                    _comp = (worldPos - new Vector3(0, GetLowestPoint(), 0));
                }

                _pos = worldPos - transform.position - new Vector3(0, fillAmount - (_comp.y * compensateShapeAmount), 0);
            }
            else
            {
                _pos = worldPos - transform.position - new Vector3(0, fillAmount, 0);
            }
            
            rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetVector(_fillAmount, _pos);
            rend.SetPropertyBlock(_propertyBlock);
        }

        //https://forum.unity.com/threads/manually-calculate-angular-velocity-of-gameobject.289462/#post-4302796
        private Vector3 GetAngularVelocity(Quaternion foreLastFrameRotation, Quaternion lastFrameRotation)
        {
            var q = lastFrameRotation * Quaternion.Inverse(foreLastFrameRotation);

            // no rotation?
            // You may want to increase this closer to 1 if you want to handle very small rotations.
            // Beware, if it is too close to one your answer will be Nan
            if (Mathf.Abs(q.w) > 1023.5f / 1024.0f)
                return Vector3.zero;
            float gain;

            // handle negatives, we could just flip it but this is faster
            if (q.w < 0.0f)
            {
                var angle = Mathf.Acos(-q.w);
                gain = -2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }
            else
            {
                var angle = Mathf.Acos(q.w);
                gain = 2.0f * angle / (Mathf.Sin(angle) * Time.deltaTime);
            }
            Vector3 angularVelocity = new Vector3(q.x * gain, q.y * gain, q.z * gain);

            if (float.IsNaN(angularVelocity.z))
            {
                angularVelocity = Vector3.zero;
            }
            return angularVelocity;
        }

        private float GetLowestPoint()
        {
            float lowestY = float.MaxValue;
            Vector3 lowestVert = Vector3.zero;
            Vector3[] vertices = mesh.vertices;

            foreach (Vector3 t in vertices)
            {
                Vector3 position = transform.TransformPoint(t);

                if (position.y < lowestY)
                {
                    lowestY = position.y;
                    lowestVert = position;
                }
            }
            return lowestVert.y;
        }
    }
}