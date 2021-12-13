using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Mod
{
    public class ArmorBehaviour : MonoBehaviour
    {
        private bool equipped;
        public string armorPiece;
        public int armorTier;
        public float stabResistance;
        private bool blockingStab;

        public Vector3 offset;
        public Vector3 scaleOffset = new Vector3(1, 1, 1);

        // :)
        // Layer 11 is Ground layer and Layer 9 is Living layer
        private bool hasShield;

        // dont do this
        public string otherSprite;
        public string otherName;
        public string otherPiece;
        public float otherResist;
        public float mass;
        public int otherTier;
        // honestly this is fucking terrible
        // not good code, messy
        public bool threepieces;
        public string thirdsprite;
        public string thirdpart;

        public bool headCovering;

        void Start()
        {
            GetComponent<PhysicalBehaviour>().HoldingPositions = new Vector3[0];
            switch (armorTier)
            {
                case 0:
                    GetComponent<PhysicalProperties>().Softness = 1;
                    GetComponent<PhysicalProperties>().Brittleness = 1;
                    break;
                case 1:
                    GetComponent<PhysicalProperties>().Softness = 0f;
                    GetComponent<PhysicalProperties>().Brittleness = 0f;
                    GetComponent<PhysicalProperties>().BulletSpeedAbsorptionPower = 10f;
                    break;
                case 2:
                    GetComponent<PhysicalBehaviour>().Properties = ModAPI.FindPhysicalProperties("Bowling pin");
                    GetComponent<PhysicalProperties>().Softness = 0f;
                    GetComponent<PhysicalProperties>().Brittleness = 0f;
                    GetComponent<PhysicalProperties>().BulletSpeedAbsorptionPower = 10f;
                    if (!headCovering)
                    {
                        GetComponent<BoxCollider2D>().size = new Vector3(GetComponent<BoxCollider2D>().size.x * 4, GetComponent<BoxCollider2D>().size.y * 4);
                    }
                    else
                    {
                        GetComponent<BoxCollider2D>().size = new Vector3(GetComponent<BoxCollider2D>().size.x * 4, GetComponent<BoxCollider2D>().size.y * .5f);
                        GetComponent<BoxCollider2D>().offset = new Vector3(0, .5f, 0);
                    }
                    break;
                case 3:
                    GetComponent<PhysicalProperties>().Softness = 0f;
                    GetComponent<PhysicalProperties>().Brittleness = 0f;
                    GetComponent<PhysicalProperties>().BulletSpeedAbsorptionPower = 100f;
                    if (!GetComponent<BoxCollider2D>())
                    {
                        gameObject.AddComponent<BoxCollider2D>();
                    }
                    GetComponent<BoxCollider2D>().size = new Vector3(GetComponent<BoxCollider2D>().size.x * 10, GetComponent<BoxCollider2D>().size.y * 10);
                    break;
            }
        }
        void ContextMenu()
        {
        this.GetComponent<PhysicalBehaviour>().ContextMenuOptions.Buttons.Add(new ContextMenuButton("selectbutt", "Change color", "Change the color of the armor.", new UnityAction[1]
        {
            (UnityAction) (() =>
        {
          DialogBox dialog = (DialogBox) null;
          dialog = DialogBoxManager.TextEntry("Change the color of the object using the following format: 0.0, 0.0, 0.0.", "Text", new DialogButton("Apply", true, new UnityAction[1]
          {
          (UnityAction) (() =>
            {
              if (dialog.EnteredText != "")
              {;
                    var text = dialog.EnteredText.Split(","[0]);
                    ModAPI.Notify("Color successfully set to " + dialog.EnteredText);
                    Color col = GetComponent<SpriteRenderer>().color;
                    col.r = float.Parse(text[0]);
                    col.g = float.Parse(text[1]);
                    col.b = float.Parse(text[2]);
                    GetComponent<SpriteRenderer>().color = col;
              }
              else
                ModAPI.Notify("You didn't input anything you god damn potato.");
            })
          }), new DialogButton("Cancel", true, new UnityAction[1]
          {
            (UnityAction) (() => dialog.Close())
          }));
        })
      }));
        }
        void Awake()
        {
            ContextMenu();
        }
        public void SpawnOtherParts()
        {
            GameObject lower = Instantiate(ModAPI.FindSpawnable(otherName).Prefab, transform.position, transform.rotation);
            lower.name = ModAPI.FindSpawnable(otherName).name;
            lower.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(otherSprite);
            if (GetComponent<PhysicalBehaviour>().IsWeightless)
            {
                lower.GetComponent<PhysicalBehaviour>().MakeWeightless();
            }
            ArmorBehaviour armor1 = lower.AddComponent<ArmorBehaviour>();
            armor1.stabResistance = otherResist;
            armor1.armorPiece = otherPiece;
            armor1.armorTier = otherTier;
            armor1.offset = offset;
            armor1.scaleOffset = scaleOffset;
            lower.FixColliders();
            if (threepieces)
            {
                GameObject lower1 = Instantiate(ModAPI.FindSpawnable(otherName).Prefab, lower.transform.position, transform.rotation);
                lower1.name = ModAPI.FindSpawnable(otherName).name;
                lower1.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite(thirdsprite);
                if (GetComponent<PhysicalBehaviour>().IsWeightless)
                {
                    lower1.GetComponent<PhysicalBehaviour>().MakeWeightless();
                }
                ArmorBehaviour armor11 = lower1.AddComponent<ArmorBehaviour>();
                armor11.stabResistance = otherResist;
                armor11.armorPiece = thirdpart;
                armor11.armorTier = otherTier;
                armor11.offset = offset;
                armor11.scaleOffset = scaleOffset;
                lower1.FixColliders();
            }
        }
        void Update()
        {
            if (equipped && GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>() && GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>().CurrentlyHolding)
            {
                GripBehaviour grip = GetComponent<FixedJoint2D>().connectedBody.gameObject.GetComponent<GripBehaviour>();
                Nocollide(grip.CurrentlyHolding.gameObject);
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            LimbBehaviour limb = collision.gameObject.GetComponent<LimbBehaviour>();
            ArmorBehaviour arm = collision.gameObject.GetComponent<ArmorBehaviour>();
            if (arm)
            {
                Nocollide(arm.gameObject);
            }
            if (limb)
            {
                Nocollide(limb.gameObject);
                // Bodypart sections are Torso, Head, Arms, and Legs
                // Bodyparts are UpperBody, MiddleBody, LowerBody etc.
                Debug.Log(limb.gameObject.ToString());
                Debug.Log(armorPiece);
                if (!equipped && limb.gameObject.ToString() == armorPiece + " (UnityEngine.GameObject)")
                {
                    Debug.Log("attach");
                    Attach(limb);
                }
            }
            if (equipped && collision.gameObject.GetComponent<Rigidbody2D>().velocity.x < stabResistance && !limb)
            {
                transform.parent = limb.transform;
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<FixedJoint2D>().enabled = false;
            }
            else if (equipped)
            {
                transform.parent = null;
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<FixedJoint2D>().enabled = true;
            }
            //ModAPI.Notify(collision.gameObject.layer.ToString());
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (GetComponent<Rigidbody2D>().isKinematic == true)
            {
                transform.parent = null;
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<FixedJoint2D>().enabled = true;
            }
        }
        public void Nocollide(GameObject col)
        {
            NoCollide noCol = gameObject.AddComponent<NoCollide>();
            noCol.NoCollideSetA = GetComponents<Collider2D>();
            noCol.NoCollideSetB = col.GetComponents<Collider2D>();
        }
        public void Attach(LimbBehaviour limb)
        {
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sortingOrder = limb.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            sr.sortingLayerName = limb.gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            equipped = true;
            GetComponent<Rigidbody2D>().isKinematic = true;
            transform.parent = limb.transform;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.localPosition = offset;
            transform.localScale = scaleOffset;
            transform.parent = null;
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.dampingRatio = 1;
            joint.frequency = 0;
            joint.connectedBody = limb.GetComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
        public void CreateShield(float shieldRadius, float shieldPower, bool shieldRegen, float shieldRegenRate, float shieldRegenCooldown)
        {
            //GameObject shieldObject = Instantiate(ModAPI.FindSpawnable("Metal Wheel").Prefab, transform.position, transform.rotation);
            GameObject shieldObject = new GameObject("shield");
            SpriteRenderer shieldRenderer = shieldObject.AddComponent<SpriteRenderer>();
            shieldRenderer.sprite = ModAPI.LoadSprite("shieldmask.png");

            CircleCollider2D shieldCollider = shieldObject.AddComponent<CircleCollider2D>();
            shieldCollider.radius = shieldRadius;

            Rigidbody2D shieldrb = shieldObject.AddComponent<Rigidbody2D>();
            shieldrb.isKinematic = true;

            PhysicalBehaviour phys = shieldObject.AddComponent<PhysicalBehaviour>();
            phys.Properties = ModAPI.FindPhysicalProperties("Metal");

            shieldObject.transform.parent = transform;

            Nocollide(shieldObject);

            shieldObject.transform.localPosition = new Vector3(0, 0, 0);
            shieldObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            shieldObject.transform.localScale = new Vector3(1f, 1f);

            //FixedJoint2D joint = shieldObject.AddComponent<FixedJoint2D>();
            //joint.connectedBody = GetComponent<Rigidbody2D>();

            ShieldBehaviour shield = shieldObject.AddComponent<ShieldBehaviour>();
            shield.shieldPower = shieldPower;
            shield.shieldRegen = shieldRegen;
            shield.shieldRegenRate = shieldRegenRate;
            shield.shieldRegenCooldown = shieldRegenCooldown;

            hasShield = true;

            ModAPI.Notify("Shield created with properties: " + shieldRadius.ToString() + " radius, " + shieldPower.ToString() + " power, shield regen is set to " + shieldRegen.ToString() + ",  " + shieldRegenRate.ToString() + " regen rate, " + shieldRegenCooldown.ToString() + " second regen cooldown.");
        }
    }
}