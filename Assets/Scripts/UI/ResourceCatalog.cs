using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChineseZombieHunter
{
    [CreateAssetMenu(menuName = "JJ Games/Resource Catalog")]
    public class ResourceCatalog : ScriptableObject
    {
        [SerializeField] private List<ResourceEntry> resources = new List<ResourceEntry>();

        public IReadOnlyList<ResourceEntry> Resources => resources;
    }

    [Serializable]
    public class ResourceEntry
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private string downloadUrl;
        [SerializeField] private string buttonLabel = "Download";
        [SerializeField] private Sprite icon;

        public string Title => title;
        public string Description => description;
        public string DownloadUrl => downloadUrl;
        public string ButtonLabel => buttonLabel;
        public Sprite Icon => icon;
    }
}
