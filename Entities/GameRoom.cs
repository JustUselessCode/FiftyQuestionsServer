namespace FiftyQuestionsServer.Entities;

public class GameRoom
{
    public List<Player> Players = new();
    public List<QuestionObject> Questions = new();

    public int RoomID { get; set; }

    public GameRoom(List<Player> players, List<QuestionObject> questions, int roomID)
    {
        Players = players;
        RoomID = roomID;
        Questions = questions;
    }

    public GameRoom(int roomID)
    {
        RoomID = roomID;
    }
}
