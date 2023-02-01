using VContainer.Unity;

namespace UT.AlchemyVR
{
    public class SceneLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();

            if (Parent == null && autoRun == false && !IsRoot)
            {
                IsRoot = true;
                Build();
            }
        }
    }
}