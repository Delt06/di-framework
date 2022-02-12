using DELTation.DIFramework;
using DELTation.DIFramework.Containers;
using Demo.Scripts.Bullets;
using UnityEngine;
using ILogger = Demo.Scripts.Bullets.ILogger;

namespace Demo.Scripts.Dependencies
{
    public sealed class CompositionRoot : DependencyContainerBase
    {
        [SerializeField] private Transform _bulletsRoot;

        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            builder
                .RegisterFromResources<BulletFactoryConfig>("Bullet Factory Config").AsInternal()
                .Register<UnityLogger>().AsInternal()
                .RegisterFromMethod((BulletFactoryConfig config, ILogger logger) =>
                    CreateBulletFactory(config, logger)
                )
                ;
        }

        private BulletFactory CreateBulletFactory(BulletFactoryConfig bulletFactoryConfig, ILogger logger) =>
            new BulletFactory(_bulletsRoot, bulletFactoryConfig.BulletPrefab,
                bulletFactoryConfig.PowerfulBulletSpeedMultiplier, logger
            );
    }
}