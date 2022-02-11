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

        protected override void ComposeDependencies(ContainerBuilder builder)
        {
            // Compose your dependencies here:
            // builder.Register(new T());
            // builder.Register<T>();

            builder
                .RegisterFromResources<BulletFactoryConfig>("Bullet Factory Config")
                .Register<UnityLogger>()
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