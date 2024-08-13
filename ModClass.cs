using HutongGames.PlayMaker.Actions;
using MagicUI.Core;
using Modding;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                MapHandler mapHandler = new MapHandler();
                mod.Log("web server started: http://localhost:" + mod.server.port + "/");

                //saveSettings.transition = new List<RoomTransition>(); //REMOVE

            }
        }

    }
}