using FiftyQuestionsServer.Entities;
using Grpc.Core;

namespace FiftyQuestionsServer.Services;

public class QuestionService : QuestionHandler.QuestionHandlerBase
{
    private readonly ILogger<QuestionService> _Logger;

    private Random _Random { get; set; }

    private int[] RoomIdsInUse = new int[999_999];

    private List<string> _Buzzers { get; set; }

    private List<GameRoom> Games { get; set; }

    public QuestionService(ILogger<QuestionService> logger)
    {
        _Logger = logger;
        _Buzzers = new List<string>();
        Games = new List<GameRoom>();
        _Random = new Random();
    }

    public override async Task<BuzzerReply> Buzzer(BuzzerRequest request, ServerCallContext context)
    {
        _Buzzers.Add(request.PlayerID);

        var Reply = new BuzzerReply
        {
            PlayerName = request.PlayerName,
            SuccessStatus = _Buzzers.IndexOf(request.PlayerID) == 0 ? true : false,
        };

        return await Task.FromResult(Reply);
    }

    public override async Task<ClearBuzzersResponse> ClearBuzzers(ClearBuzzersRequest request, ServerCallContext context)
    {
        if (request.Clear)
        {
            _Buzzers.Clear();
            return await Task.FromResult(new ClearBuzzersResponse
            {
                Cleared = true
            });
        }

        else
        {
            return await Task.FromResult(new ClearBuzzersResponse
            {
                Cleared = false
            });
        }
    }

    public override async Task<CreateGameRoomResponse> CreateGameRoom(CreateGameRoomRequest request, ServerCallContext context)
    {
        // Right now even though a room number up to 999 999 is supported only a single Game Room will be allowed to exist at a time!
        if (Games.Count > 1)
        {
            // It is intendet that this Exception has the Power to completely shut down the App
            throw new ApplicationException("To many concurrent Games! The Service is shutting down due to a Denial of Service Attack!");
        }

        int RoomId = _Random.Next(0, 999_999);

        while (RoomIdsInUse[RoomId] == RoomId)
        {
            RoomId = _Random.Next();
        }

        RoomIdsInUse[RoomId] = RoomId;

        Games.Add(new GameRoom(RoomId));

        return await Task.FromResult(new CreateGameRoomResponse
        {
            RoomNumber = RoomId
        });

    }
}
