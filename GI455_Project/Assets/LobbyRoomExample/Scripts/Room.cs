namespace LobbyRoomExample
{
    public class Room
    {
        public string RoomName
        {
            get
            {
                return roomName;
            }
        }

        private string roomName;

        public Room(string roomName)
        {
            this.roomName = roomName;
        }
    }
}
