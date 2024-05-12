using System.Text;
using QuestionService;
namespace FiftyQuestionsServer.Entities;

public class Player
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool Winner { get; set; }

    public PlayerRole _Role { get; set; }

    public Player(Guid id, string name, PlayerRole role)
    {
        Id = id;
        Name = name;
        _Role = role;
        Winner = false;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("Name: " + Name);
        sb.Append("  Id: " + Id.ToString());
        return sb.ToString();
    }
}
