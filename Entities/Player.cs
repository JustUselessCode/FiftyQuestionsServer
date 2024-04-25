using System.Text;

namespace FiftyQuestionsServer.Entities;

public class Player
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool Winner { get; set; }

    public PlayerRole _Role { get; set; }

    public Player(string name, PlayerRole role)
    {
        Id = Guid.NewGuid();
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
