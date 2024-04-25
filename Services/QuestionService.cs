using FiftyQuestionsServer;
using Grpc.Core;

namespace FiftyQuestionsServer.Services;

public class QuestionService : QuestionHandler.QuestionHandlerBase
{
    private readonly ILogger<QuestionService> _logger;

    private List<string> _buzzers;
    public QuestionService(ILogger<QuestionService> logger)
    {
        _logger = logger;
        _buzzers = new List<string>();
    }

    public override async Task<BuzzerReply> Buzzer(BuzzerRequest request, ServerCallContext context)
    {
        _buzzers.Add(request.PlayerID);

        var Reply = new BuzzerReply
        {
            PlayerName = request.PlayerName,
            SuccessStatus = _buzzers.IndexOf(request.PlayerID) == 0 ? true : false,
        };

        return await Task.FromResult(Reply);
    }

    public override async Task<ClearBuzzersResponse> ClearBuzzers(ClearBuzzersRequest request, ServerCallContext context)
    {
        if (request.Clear)
        {
            _buzzers.Clear();
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
}
