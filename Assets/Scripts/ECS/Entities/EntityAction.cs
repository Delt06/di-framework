using ECS.Core;
using JetBrains.Annotations;

namespace ECS.Entities
{
	public delegate void EntityAction([NotNull] IEntity entity);
}