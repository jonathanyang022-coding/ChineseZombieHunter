using System;
using UnityEngine;
using UnityEngine.UI;

namespace ChineseZombieHunter
{
    public class Stage1Barrel : MonoBehaviour
    {
        [SerializeField] private Transform travelRoot;
        [SerializeField] private GameObject barrelRoot;
        [SerializeField] private GameObject zombieGroupRoot;
        [SerializeField] private Text barrelLabelText;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private Animator animator;
        [SerializeField] private string explodeTrigger = "Explode";
        [SerializeField] private float moveSpeed = 0.7f;

        private bool isActiveEncounter;
        private Vector3 startPosition;

        private void Awake()
        {
            if (travelRoot != null)
            {
                startPosition = travelRoot.position;
            }
        }

        private void Update()
        {
            if (!isActiveEncounter || travelRoot == null)
            {
                return;
            }

            travelRoot.position += Vector3.forward * (moveSpeed * Time.deltaTime);
        }

        public void ResetEncounter(string label)
        {
            isActiveEncounter = true;

            if (travelRoot != null)
            {
                travelRoot.position = startPosition;
            }

            if (barrelRoot != null)
            {
                barrelRoot.SetActive(true);
            }

            if (zombieGroupRoot != null)
            {
                zombieGroupRoot.SetActive(true);
            }

            if (barrelLabelText != null)
            {
                barrelLabelText.text = label;
            }

            if (explosionEffect != null)
            {
                explosionEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void SetLabel(string label)
        {
            if (barrelLabelText != null)
            {
                barrelLabelText.text = label;
            }
        }

        public void Explode()
        {
            isActiveEncounter = false;

            if (animator != null && !string.IsNullOrWhiteSpace(explodeTrigger))
            {
                animator.SetTrigger(explodeTrigger);
            }

            if (explosionEffect != null)
            {
                explosionEffect.Play();
            }

            if (zombieGroupRoot != null)
            {
                zombieGroupRoot.SetActive(false);
            }

            if (barrelRoot != null)
            {
                barrelRoot.SetActive(false);
            }
        }
    }
}
