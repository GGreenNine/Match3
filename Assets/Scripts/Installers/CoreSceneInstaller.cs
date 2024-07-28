using Data;
using Factories;
using Game;
using GameState;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class CoreSceneInstaller : MonoInstaller
    {
        [SerializeField] private TileControl _tileControlPrefab;
        [SerializeField] private CoreSettings _coreSettings;
        
        public override void InstallBindings()
        {
            Container.BindIFactory<TileControl>().FromMethod(_ => Instantiate(_tileControlPrefab));
            Container.Bind<CoreSettings>().FromScriptableObject(_coreSettings).AsSingle();
            Container.BindInterfacesAndSelfTo<GridFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateController>().AsSingle();
            Container.BindInterfacesTo<GameScoreController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameGlobalTimer>().AsSingle();
        }
    }
}