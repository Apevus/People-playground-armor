# People-playground-armor
Adds armor for ppg

Pretty simple, Hat pixel size MUST BE 16x5 PIXELS, and you can change Head with UpperBody, LowerBoddy, MiddleBody, ya know

SNIPPET:

    ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Rod"),
                    NameOverride = "Fedora-CP",
                    NameToOrderByOverride = "A5",
                    DescriptionOverride = " ",
                    CategoryOverride = ModAPI.FindCategory("City People"),
                    ThumbnailOverride = ModAPI.LoadSprite("fedora.png"),
                    AfterSpawn = (Instance) =>
                    {
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("fedora.png");
						Instance.FixColliders();
                        ArmorBehaviour armor = Instance.AddComponent<ArmorBehaviour>();

                        armor.stabResistance = 0f;
                        armor.armorPiece = "Head";
                        armor.armorTier = 0;
                    }
                }
            );
