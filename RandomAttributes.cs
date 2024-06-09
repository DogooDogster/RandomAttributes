﻿using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using UnityEngine;

namespace NoGravity
{
    [BepInPlugin("com.dogoodogster.randomattributes", "Random Attributes", "1.0.0")]
    public class RandomAttributes : BaseUnityPlugin
    {
        public Harmony harmony;
        public static RandomAttributes instance;
        public static Attributes[] attributes = new Attributes[4];
        private void Awake()
        {
            instance = this;
            harmony = new Harmony(Info.Metadata.GUID);

            harmony.PatchAll(GetType());

            var bob = new Cow("Bob", "oogashaka", "culting");
            var joe = new Cow("Joe", "a communist", "voting for communist leaders");
            Logger.LogInfo(bob.Moo());
            Logger.LogInfo(joe.Moo());
        }

        [HarmonyPatch(typeof(GameSessionHandler), nameof(GameSessionHandler.Init))]
        [HarmonyPostfix]
        public static void GameStart()
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                attributes[i] = new Attributes()
                {
                    scale = Updater.RandomFix((Fix)(-1), (Fix)3),
                    speed = Updater.RandomFix((Fix)1, (Fix)87.2),
                    jumpStrength = Updater.RandomFix((Fix)9.3, (Fix)109.8),
                    color = new Color((float)Updater.RandomFix((Fix)0, (Fix)1), (float)Updater.RandomFix((Fix)0, (Fix)1), (float)Updater.RandomFix((Fix)0, (Fix)1))
                };
            }

            foreach (var player in PlayerHandler.Get().PlayerList())
            {
                player.Scale = attributes[player.Id - 1].scale;
            }
        }
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.UpdateSim))]
        [HarmonyPrefix]
        public static void PlayerPhysicsUpdate(PlayerPhysics __instance, ref IPlayerIdHolder ___playerIdHolder)
        {
            var slimecontroller = __instance.GetComponent<SlimeController>();

            if (slimecontroller != null)
            {
                slimecontroller.GetPlayerSprite().material.SetColor("_ShadowColor", attributes[___playerIdHolder.GetPlayerId() - 1].color);


                __instance.Speed = attributes[___playerIdHolder.GetPlayerId() - 1].speed;
                __instance.jumpStrength = attributes[___playerIdHolder.GetPlayerId() - 1].jumpStrength;
            }
        }
    }

    public class Attributes
    {
        public Fix scale;
        public Fix speed;
        public Fix jumpStrength;
        public Color color;
    }

    class Cow
    {
        public string name;
        public string description;
        public string purpose;
        public string speciesName;

        public Cow(string name, string description, string purpose)
        {
            speciesName = GetType().Name;
            this.name = name;
            this.description = description;
            this.purpose = purpose;
        }

        public string Moo()
        {
            return $"Moo, I am a {speciesName}. my name is {name} and I am described as {description} and my purpose is {purpose}.";
        }
    }
}