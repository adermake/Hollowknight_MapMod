using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UObject = UnityEngine.Object;


namespace Randomizer_Map
{
    public class MapHandler
    {
        string lastScene = null;

        public MapHandler()
        {
            On.GameManager.BeginSceneTransition += Transition;
            MapServer.Instance.SendAllTransitions(Randomizer_Map.saveSettings.transition);
        }
        public void Transition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
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

            orig(self, info);

            Randomizer_Map.MOD.Log("Transition deteted ");


            if (Randomizer_Map.saveSettings != null && Randomizer_Map.saveSettings.transition != null && closest != null)
            {
                Randomizer_Map.MOD.Log("creating Transition  ");

                createNewMapTransition(lastScene, info.SceneName, closest.name, info.EntryGateName);
       
       
               
            }

            lastScene = info.SceneName;

        }

        public bool isTransitionNew(string fromRoom, string toRoom, string fromGate, string toGate)
        {
            
            if (Randomizer_Map.saveSettings.transition == null)
            {
                return true;
            }
            foreach (RoomTransitionData rt in Randomizer_Map.saveSettings.transition)
            {
                if (rt.fromRoom.roomName == fromRoom && rt.toRoom.roomName == toRoom && rt.fromGate.gateName == fromGate && rt.toGate.gateName == toGate)
                {
                    return false;
                }
            }
            return true;
        }

        public void createNewMapTransition(string fromRoomName, string toRoomName, string fromGateName, string toGateName)
        {
            if (isTransitionNew(fromRoomName,toRoomName,fromGateName,toGateName))
            {
                Randomizer_Map.MOD.Log("Transition new ");
                RoomData fromRoom = getOrCreateRoom(fromRoomName);
                RoomData toRoom = getOrCreateRoom(toRoomName);
                // WORKAROUND FOR NOT HAVING DATA FOR THE GATES
                GateData newFromGate = new GateData()
                {
                    gateName = fromGateName
                };
                GateData newToGate = new GateData()
                {
                    gateName = toGateName
                };
                RoomTransitionData rt = new RoomTransitionData()
                {
                    fromRoom = fromRoom,
                    toRoom = toRoom,
                    fromGate = newFromGate,
                    toGate = newToGate
                    
                    
                };
                fromRoom.gates.Add(newFromGate);
                toRoom.gates.Add(newToGate);
                Randomizer_Map.saveSettings.transition.Add(rt);
                MapServer.Instance.SendNewTransition(rt);
            }
            else
            {
                Randomizer_Map.MOD.Log("Transition already exists");
            }
        }

        public RoomData getOrCreateRoom(string room)
        {
            foreach (RoomTransitionData rt in Randomizer_Map.saveSettings.transition)
            {
                if (rt.fromRoom.roomName == room)
                {
                    return rt.fromRoom;
                }
                if (rt.toRoom.roomName == room)
                {
                    return rt.toRoom;
                }
            }
            
            RoomData newRoom = new RoomData()
            {
                roomName = room,
                width = 100,
                height = 100,
                mapPosX = -1,
                mapPosY = -1
               
                // GET additional data from sergey 
                
            };
            return newRoom;
        }


    }
}
