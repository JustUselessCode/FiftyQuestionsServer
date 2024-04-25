namespace FiftyQuestionsServer.Entities;

public class GameRoom
{
    public List<Player> Players = new();

    public int RoomID { get; set; }

    public GameRoom(List<Player> players, int roomID)
    {
        Players = players;
        RoomID = roomID;
    }

    public GameRoom(int roomID)
    {
        RoomID = roomID;
    }
}
