using UnityEngine;
using UT.UnityFoundation;
using VContainer;
using VContainer.Unity;

namespace UT.AlchemyVR
{
    public class AlchemyVRInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField]
        private AlchemyVRApp app;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponent(app).As<BaseApp>();
        }
    }
}