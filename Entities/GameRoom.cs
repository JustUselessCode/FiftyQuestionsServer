namespace FiftyQuestionsServer.Entities;

public class GameRoom
{
    public List<Player> Players { get; set; }
    public List<QuestionObject> Questions { get; set; }

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
