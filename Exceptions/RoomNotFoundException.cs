namespace FiftyQuestionsServer.Exceptions
{
    public class RoomNotFoundException : ApplicationException
    {
        private static readonly string BaseMessage = "The requested Room does not exist. Please check if the entered RoomID is correct!";
        
        public RoomNotFoundException(string message, Exception innerException):base(message, innerException)
        {

        }

        public RoomNotFoundException() : base(BaseMessage)
        {

        }
    }
}
