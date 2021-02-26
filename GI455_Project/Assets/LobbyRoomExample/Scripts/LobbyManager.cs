using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LobbyRoomExample
{
    public struct MessageCallback
    {
        public bool status;
        public string message;

        public MessageCallback(bool status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }

    public class LobbyManager : MonoBehaviour
    {
        public Room CurrentRoom
        {
            get
            {
                return currentRoom;
            }
        }

        private Room currentRoom;
        private List<Room> roomList = new List<Room>();

        public delegate void DelegateHandle(MessageCallback result);
        public event DelegateHandle OnCreateRoom;
        public event DelegateHandle OnJoinRoom;
        public event DelegateHandle OnLeaveRoom;

        public static LobbyManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void CreateRoom(string roomName)
        {
            if (IsExistRoom(roomName))
            {
                if (OnCreateRoom != null)
                    OnCreateRoom(new MessageCallback(false, "Room name is exist."));

                return;
            }

            Room newRoom = new Room(roomName);

            roomList.Add(newRoom);

            currentRoom = newRoom;

            if (OnCreateRoom != null)
                OnCreateRoom(new MessageCallback(true, "Create room success."));
        }

        public void JoinRoom(string roomName)
        {
            if (!IsExistRoom(roomName))
            {
                if (OnJoinRoom != null)
                    OnJoinRoom(new MessageCallback(false, "Room name is not exist."));

                return;
            }

            currentRoom = GetRoomByName(roomName);

            if (OnJoinRoom != null)
                OnJoinRoom(new MessageCallback(true, "Join room success"));
        }

        public void LeaveRoom()
        {
            if(currentRoom == null)
            {
                if (OnLeaveRoom != null)
                    OnLeaveRoom(new MessageCallback(false, "Your is not joid in room."));
                return;
            }

            currentRoom = null;

            if (OnLeaveRoom != null)
                OnLeaveRoom(new MessageCallback(true, "Leave room success."));
        }

        public List<Room> GetRoomList()
        {
            List<Room> _roomList = new List<Room>();
            _roomList.AddRange(roomList);
            return _roomList;
        }

        private bool IsExistRoom(string roomName)
        {
            Room room = GetRoomByName(roomName);

            return room != null;
        }

        private Room GetRoomByName(string roomName)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].RoomName == roomName)
                {
                    return roomList[i];
                }
            }

            return null;
        }
    }
}
