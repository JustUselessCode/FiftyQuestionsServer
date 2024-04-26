
using Newtonsoft.Json;
using FiftyQuestionsServer.Entities;

namespace FiftyQuestionsServer.Helper;

public class QuestionParsingHelper
{
    public static QuestionList ParseJsonFile(string FilePath)
    {
        var jsonText = File.ReadAllText(FilePath);

        var data = JsonConvert.DeserializeObject<QuestionList>(jsonText);

        return data;
    }
}
