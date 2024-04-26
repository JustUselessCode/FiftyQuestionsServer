namespace FiftyQuestionsServer.Entities
{
    public class QuestionList
    {
        public QuestionObject[]? questionList { get; set; }
    }

    public class QuestionObject
    {
        public int Index { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public QuestionObject(int index, string question, string answer)
        {
            Index = index;
            Question = question;
            Answer = answer;
        }
    }
}
