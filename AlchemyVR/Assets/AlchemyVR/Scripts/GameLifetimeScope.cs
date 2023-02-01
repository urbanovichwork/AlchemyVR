using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UT.AlchemyVR
{
    public class GameLifetimeScope : SceneLifetimeScope
    {
        [SerializeField]
        private Cauldron cauldron;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(cauldron).As<IInitializable>();
        }
    }
}