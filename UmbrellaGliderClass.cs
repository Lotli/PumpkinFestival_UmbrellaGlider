using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Il2CppInterop.Runtime;
using MelonLoader;
using Il2CppTMPro;
using Il2Cpp;

namespace Umbrella_Glider
{
    public class UmbrellaGliderClass : MelonMod
    {
        private GameObject umbrellaModel;
        private GameObject player;
        private GameObject glider;

        private float glidingForce = 10;
        private bool isGlidingOn = false;
        KeyCode glidingKey;

        private MelonPreferences_Category gliderPreferencesCategory;
        private MelonPreferences_Entry<float> glidingForceEntry;
        private MelonPreferences_Entry<KeyCode> glidingKeyEntry;

        public override void OnInitializeMelon()
        {
            gliderPreferencesCategory = MelonPreferences.CreateCategory("Umbrella Glider");
            glidingForceEntry = gliderPreferencesCategory.CreateEntry<float>("GlidingForce", 10, "Gliding Force");
            glidingKeyEntry = gliderPreferencesCategory.CreateEntry<KeyCode>("GlidingKey", KeyCode.G, "Gliding Key");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Scene_Festival")
            {
                glidingKey = glidingKeyEntry.Value;
                glidingForce = glidingForceEntry.Value;

                umbrellaModel = GameObject.Find("Parasol_Red");
                if (umbrellaModel == null) LoggerInstance.Msg("Umbrella model not found");
                player = GameObject.Find("/PObject (Player)");
                if (player == null) LoggerInstance.Msg("Player object not found");

                glider = GameObject.Instantiate(umbrellaModel, player.transform);
                glider.transform.localRotation = Quaternion.EulerAngles(0, 0, 0);
                glider.transform.localScale = new Vector3(0.5f, 0.25f, 0.5f);
                glider.transform.localPosition = new Vector3(0, -0.2f, 0);
                glider.transform.GetChild(0).localScale = new Vector3(1, 2, 1);
                glider.SetActive(false);
                isGlidingOn = false;
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(glidingKey) && player != null)
            {
                glider.SetActive(!glider.activeSelf);
                isGlidingOn = !isGlidingOn;
            }
        }

        public override void OnFixedUpdate()
        {
            if (player != null)
            {
                float verticalVelocity = player.GetComponent<Rigidbody>().velocity.y;
                if (verticalVelocity < 0 && isGlidingOn)
                {
                    player.GetComponent<Rigidbody>().AddForce(glidingForce * Math.Abs(player.GetComponent<Rigidbody>().velocity.y) * Vector3.up);
                }
            }
        }

        private void DumpObject(GameObject obj, int depth)
        {
            if (obj == null)
                return;

            string indent = new string(' ', depth * 2);

            MelonLogger.Msg(
                $"{indent}- {obj.name} " +
                $"[Active: {obj.activeSelf}]"
            );

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);

                if (child != null)
                    DumpObject(child.gameObject, depth + 1);
            }
        }
    }
}
