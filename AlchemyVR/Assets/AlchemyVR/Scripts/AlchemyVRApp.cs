using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UT.UnityFoundation;

namespace UT.AlchemyVR
{
    public class AlchemyVRApp : BaseApp
    {
        [SerializeField]
        private AssetReference gameScene;

        protected override void StartGame()
        {
            PlayGame().Forget();
        }

        private async UniTaskVoid PlayGame()
        {
            GameToken token = await PlayModule<GameToken>(gameScene);
            switch (token.Result)
            {
                case GameToken.Kind.Restart:
                    PlayGame().Forget();
                    break;
            }
        }
    }

}