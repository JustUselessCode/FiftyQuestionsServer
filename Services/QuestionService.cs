using Grpc.Core;
using System.Text;
using QuestionService;
using FiftyQuestionsServer.Helper;
using FiftyQuestionsServer.Entities;
using FiftyQuestionsServer.Exceptions;

namespace FiftyQuestionsServer.Services;

public class FiftyQuestionService : QuestionHandler.QuestionHandlerBase
{
	private readonly ILogger<FiftyQuestionService> _Logger;

	private Random _RandomRoomNumber { get; set; }

	private int[] RoomIdsInUse = new int[999_999];

	private static List<Player> _Buzzers = new();

	private static List<GameRoom> _Games = new();

	private readonly static string QuestionStore = @"E:\50FragenStore\Questions.json";



	public FiftyQuestionService(ILogger<FiftyQuestionService> logger)
	{
		_Logger = logger;
		_RandomRoomNumber = new();
	}

	public override async Task<BuzzerReply> Buzzer(BuzzerRequest request, ServerCallContext context)
	{
		var wantedGame = _Games.Find(Room => Room.RoomID == request.RoomID);
		var wantedPlayer = wantedGame.Players.Find(player => player.Id.ToString().Trim() == request.PlayerID);
			
		_Buzzers.Add(wantedPlayer);

		var Reply = new BuzzerReply
		{
			PlayerName = wantedPlayer!.Name,
			SuccessStatus = _Buzzers.IndexOf(wantedPlayer) == 0,
		};

		foreach (var item in _Buzzers)
		{
			await Console.Out.WriteLineAsync(item.Name);
		}

		return await Task.FromResult(Reply);
	}

	public override async Task<ClearBuzzersResponse> ClearBuzzers(ClearBuzzersRequest request, ServerCallContext context)
	{
		_Buzzers.Clear();
		return await Task.FromResult(new ClearBuzzersResponse
		{
			Cleared = _Buzzers.Count() == 0
		}) ;
	}

	public override async Task<CreateGameRoomResponse> CreateGameRoom(CreateGameRoomRequest request, ServerCallContext context)
	{
		// Right now even though a room number up to 999 999 is supported only a single Game Room will be allowed to exist at a time!
		if (_Games.Count > 1)
		{
			// It is intendet that this Exception has the Power to completely shut down the Service. 
			// The number of concurrent games will probably be increased in the 
			throw new ApplicationException("To many concurrent _Games! The Service is shutting down due to a potential Denial of Service Attack!");
		}

		int RoomId = _RandomRoomNumber.Next(0, 1_000_000);

		while (RoomIdsInUse[RoomId] == RoomId)
		{
			RoomId = _RandomRoomNumber.Next();
		}

		RoomIdsInUse[RoomId] = RoomId;

		_Games.Add(new GameRoom(RoomId));

		return await Task.FromResult(new CreateGameRoomResponse
		{
			RoomID = RoomId
		});
	}

	public override async Task<ParticipationResponse> AddParticipant(ParticipationRequest request, ServerCallContext context)
	{
		var Player = new Player(Guid.Parse(request.PlayerID), request.PlayerName, request.Role);
		try
		{
			var Room = _Games.Find(game => game.RoomID == request.RoomID) ?? throw new RoomNotFoundException();
			Room.Players.Add(Player);
		}
		catch (RoomNotFoundException ex)
		{
			_Logger.LogError(ex.StackTrace);

			return await Task.FromResult(new ParticipationResponse
			{
				SuccesStatus = false,
				ErrorMessage = ex.Message
			});
		}

		return await Task.FromResult(new ParticipationResponse
		{
			SuccesStatus = true
		});
	}

	public override async Task<RequestFileUploadResponse> RequestQuestionFileUpload(RequestFileUploadRequest request, ServerCallContext context)
	{
		if (_Games.Find(game => game.RoomID == request.RoomID) is null)
		{
			var ex = new RoomNotFoundException();

			_Logger.LogError(ex.StackTrace);

			return await Task.FromResult(new RequestFileUploadResponse
			{
				SuccessStatus = false,
				CorrelationToken = null
			});
		}

		var correlationToken = Guid.NewGuid().ToString();

		return await Task.FromResult(new RequestFileUploadResponse
		{
			SuccessStatus = true,
			CorrelationToken = correlationToken
		});
	}

	public override async Task<FileUploadResponse> UploadFile(IAsyncStreamReader<FileChunk> requestStream, ServerCallContext context)
	{
		var _File = File.Create(QuestionStore, (int)GeneralHelper.DataSizes.Megabyte);
		GameRoom? room = null;
		int ChunkCount = 0;

		try
		{
			await foreach (var Chunk in requestStream.ReadAllAsync())
			{
				ChunkCount++;

				if (Chunk.MimeType != "Json")
				{
					throw new NotSupportedException("Only JSON Files are accepted!");
				}

				if (ChunkCount == 1)
				{
					room = _Games.Find(Room => Room.RoomID == Chunk.RoomID);
				}

				await _File.WriteAsync(Chunk.ChunkData.ToByteArray());
			}
		}
		
		catch (Exception ex)
		{
			_Logger.LogError(ex.StackTrace);
			return await Task.FromResult(new FileUploadResponse
			{
				SuccessStatus = false
			});
		}

		room.Questions = QuestionParsingHelper.ParseJsonFile(QuestionStore).questionList.ToList();

		return await Task.FromResult(new FileUploadResponse
		{
			SuccessStatus = true
		});
	}

	public override async Task<DebugLogResponse> DebugLog(DebugLogRequest request, ServerCallContext context)
	{
		var builder = new StringBuilder();
		foreach (var p in _Buzzers)
		{
			builder.Append(p.ToString());
			builder.Append("\n\n");
		}

		return await Task.FromResult(new DebugLogResponse
		{
			DebugInfo = builder.ToString()
		});
	}
}
