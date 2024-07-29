using HutongGames.PlayMaker.Actions;
using MagicUI.Core;
using Modding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Randomizer_Map
{
    public class Randomizer_Map : Mod, ILocalSettings<LocalSettingsClass>
    {
        public static Randomizer_Map MOD;
        internal MapServer server;
        //SAVE SETTINGS
        public static LocalSettingsClass saveSettings { get; set; } = new LocalSettingsClass();

        public void OnLoadLocal(LocalSettingsClass s) => saveSettings = s;
        public LocalSettingsClass OnSaveLocal() => saveSettings;


        private LayoutRoot? layout;

        new public string GetName() => "Randomizer_Map";
        public override string GetVersion() => "v4";
        public override void Initialize()
        {
            Log("INIT MOD SERVER");
            MOD = this;
            On.GameManager.BeginSceneTransition += Transition;
            ModHooks.HeroUpdateHook += HeroUpdate;
            SpriteLoader.LoadSprites();

            var go = new GameObject("MapManager", typeof(MapManager));
            var tmm = go.GetComponent<MapManager>();
            tmm.mod = this;
            UnityEngine.Object.DontDestroyOnLoad(go);


        }
        float dt = 0;
        public void HeroUpdate()
        {
            dt += Time.deltaTime;
            if (dt > 1)
            {
                //GameCameras.instance.tk2dCam.SettingsRoot.CameraSettings.projection = tk2dCameraSettings.ProjectionType.Orthographic;
                //GameCameras.instance.tk2dCam.gameObject.GetComponent<Camera>().orthographic = true;
                //GameCameras.instance.tk2dCam.gameObject.GetComponent<Camera>().orthographicSize = dt;
            }
        }


        string lastScene;

        private void Transition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            Vector3 heroPos = HeroController.instance.gameObject.transform.position;
            TransitionPoint[] array = UObject.FindObjectsOfType<TransitionPoint>();
            float distance = 1000000;
            TransitionPoint closest = null;
            if (array != null)
            {
                foreach (TransitionPoint tp in array)
                {
                    if (Vector3.Distance(tp.gameObject.transform.position, heroPos) < distance)
                    {
                        distance = Vector3.Distance(tp.gameObject.transform.position, heroPos);
                        closest = tp;
                    }

                }
            }

            if (info != null && info.EntryGateName != null && info.HeroLeaveDirection != null)
            {

                Log($"TRANSITION from " + lastScene + " TO " + info.SceneName + " FROM " + closest.name + " TO " + info.EntryGateName);
            }
       
     
         

            

            orig(self, info);

            TransitionPoint[] newRoomTP = UObject.FindObjectsOfType<TransitionPoint>();
            float minX = 100000;
            float minY = 100000;
            float maxX = -100000;
            float maxY = -100000;
            if (array != null)
            {
               
                foreach (TransitionPoint tp in array)
                {
                    if (tp.gameObject.transform.position.x < minX)
                    {
                        minX = tp.gameObject.transform.position.x;
                    }
                    if (tp.gameObject.transform.position.y < minY)
                    {
                        minY = tp.gameObject.transform.position.y;
                    }
                    if (tp.gameObject.transform.position.x > maxX)
                    {
                        maxX = tp.gameObject.transform.position.x;
                    }
                    if (tp.gameObject.transform.position.y > maxY)
                    {
                        maxY = tp.gameObject.transform.position.y;
                    }

                }
            }

            if (saveSettings != null && saveSettings.transition != null && closest != null)
            {
                RoomTransition roomTransition = new RoomTransition()
                {
                    fromRoom = lastScene,
                    toRoom = info.SceneName,
                    fromGate = closest.name,
                    toGate = info.EntryGateName,
                    toRoomWidth = maxX - minX,
                    toRoomHeight = maxY - minY

                };

                saveSettings.transition.Add(roomTransition);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new RoomTransitionConverter());

                if (server != null)
                {
                    string json = JsonConvert.SerializeObject(saveSettings.transition, settings);
                    server.Send(json);

                }
            }

            lastScene = info.SceneName;

        }

        internal class MapManager : MonoBehaviour
        {
            public Randomizer_Map mod;
            public void Start() => StartCoroutine(StartServer());
            public void OnApplicationQuit() => mod?.server.Stop();
            public void OnDisable() => mod?.server.Stop();

            private IEnumerator StartServer()
            {
                //game crashes if we start server right away
                yield return new WaitForSeconds(5F);
                
                mod.Log("Trying to start server");

                mod.server = new MapServer();
                mod.server.Start();
                mod.Log("web server started: http://localhost:" + mod.server.port + "/");

                //saveSettings.transition = new List<RoomTransition>(); //REMOVE

            }
        }

    }
}