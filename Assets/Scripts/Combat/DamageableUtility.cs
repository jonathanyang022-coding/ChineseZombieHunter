using UnityEngine;

namespace ChineseZombieHunter
{
    public static class DamageableUtility
    {
        public static IDamageable FindInParents(Component component)
        {
            if (component == null)
            {
                return null;
            }

            Transform current = component.transform;
            while (current != null)
            {
                MonoBehaviour[] behaviours = current.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour behaviour in behaviours)
                {
                    if (behaviour is IDamageable damageable)
                    {
                        return damageable;
                    }
                }

                current = current.parent;
            }

            return null;
        }
    }
}
