using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Mod
{
    public class ShieldBehaviour : MonoBehaviour
    {
        public float shieldPower;
        public float initialPower;

        public bool shieldRegen;
        public float shieldRegenRate;
        public float shieldRegenCooldown;

        public bool shieldActive = true;
        public bool regenActive;

        public CircleCollider2D shieldCollider;

        public SpriteRenderer shieldVisual;

        void Start()
        {
            shieldVisual = GetComponent<SpriteRenderer>();

            shieldCollider = GetComponent<CircleCollider2D>();
            initialPower = shieldPower;
            ActivateShield();
        }
        void Update()
        {
            if (shieldPower <= 0)
            {
                DeactivateShield();
            }
        }
        private void Shot(global::Shot shot)
        {
            if (shieldPower - shot.damage >= 0)
            {
                shieldPower -= shot.damage;
            }
            else
            {
                shieldPower = 0;
            }
            ModAPI.Notify(shieldPower.ToString());
            if (regenActive)
            {
                StopCoroutine(Regen(shieldRegenCooldown, shieldRegenRate));
                StartCoroutine(Regen(shieldRegenCooldown, shieldRegenRate));
            }
        }
        public void DeactivateShield()
        {
            ModAPI.Notify("Shield Deactivated");
            shieldCollider.enabled = false;
            shieldActive = false;
            if (shieldRegen)
            {
                StartCoroutine(Regen(shieldRegenCooldown, shieldRegenRate));
            }
        }
        public void ActivateShield()
        {
            ModAPI.Notify("Shield Activated");
            shieldCollider.enabled = true;
            shieldActive = true;
        }
        public IEnumerator Regen(float Cooldown, float Rate)
        {
            ModAPI.Notify("Shield Regenerating");
            regenActive = true;
            yield return new WaitForSeconds(Cooldown);
            for (float I = shieldPower; I < initialPower; I += Time.deltaTime * Rate)
            {
                shieldPower = I;
            }
            ActivateShield();
            regenActive = false;
            ModAPI.Notify(shieldPower.ToString());
        }
    }
}