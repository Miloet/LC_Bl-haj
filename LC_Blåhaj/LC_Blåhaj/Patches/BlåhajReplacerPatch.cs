using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using LC_Blåhaj;
using System;

namespace LC_Blåhaj.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemReplacerPatch
    {

        static string itemName = "Blåhaj";

        static string assetName = "blåhaj.haj";

        static Mesh customMesh = null;
        static string meshName = "blåHaj.mesh";

        static Material blåhajMat;
        static string materialName = "defaultMat.mat";

        static AudioClip pickUp;
        static string pickUpAudioName = "BlåhajPickup.wav";

        static AudioClip drop;
        static string dropAudioName = "BlåhajPickup.mp3";

        static float blåhajSize = 5f;
        static string affectedObject = "fishtestprop";
        //static string[] affectedObjects = { "fishtestprop" };
        
        static Vector3 rotation = new Vector3(0,0,0);


        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ReplaceModel(ref Item ___itemProperties, ref MeshRenderer ___mainObjectRenderer, ref int ___floorYRot)
        {
            if (customMesh == null)
            {
                string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string path = Path.Combine(currentDirectory, assetName).Replace("\\", "/");

                BlåhajReplacerMod.mls.LogMessage("Searching this filepath:" + path);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                AssetBundle assets = request.assetBundle;

                customMesh = assets.LoadAsset<Mesh>(meshName);
                blåhajMat = assets.LoadAsset<Material>(materialName);

                pickUp = assets.LoadAsset<AudioClip>(pickUpAudioName);
                drop = assets.LoadAsset<AudioClip>(dropAudioName);


                foreach(ScanNodeProperties node in GameObject.FindObjectsOfType<ScanNodeProperties>())
                {
                    BlåhajReplacerMod.mls.LogMessage(node.name +" "+ node.headerText + " " + node.subText);
                }
            }

            //if (affectedObjects.Contains(___itemProperties.name.ToLower()))
            if(affectedObject == ___itemProperties.name.ToLower())
            {
                MeshFilter mf = ___mainObjectRenderer.GetComponent<MeshFilter>();
                mf.mesh = UnityEngine.Object.Instantiate(customMesh);

                Mesh mesh = mf.mesh;

                Vector3[] vertices = mesh.vertices;
                Vector3 center = mesh.bounds.center;

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = center + (vertices[i] - center) * blåhajSize;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();

                ___mainObjectRenderer.materials = new Material[1] {blåhajMat};
                //___mainObjectRenderer.transform.rotation = rotation;
                ___mainObjectRenderer.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

                ___itemProperties.grabSFX = pickUp;
                ___itemProperties.dropSFX = drop;

                ___itemProperties.spawnPrefab.GetComponent<MeshFilter>().mesh = mesh;
                ___itemProperties.spawnPrefab.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

                ___itemProperties.itemName = itemName;
                ___mainObjectRenderer.GetComponentInChildren<ScanNodeProperties>().headerText = itemName;

                ___floorYRot = -1;
                ___itemProperties.restingRotation = rotation;

                
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Upright(ref Item ___itemProperties, ref MeshRenderer ___mainObjectRenderer)
        {
            try
            {
                if(affectedObject == ___itemProperties.name.ToLower())
                {
                    ___mainObjectRenderer.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
                }
            }
            catch
            {
                BlåhajReplacerMod.mls.LogMessage(___itemProperties.name.ToLower() + " failed to update rotation");
            }
        }
    }
}
