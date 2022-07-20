namespace CurrencyExchange2.Responses
{
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }

        public int? StatusCode { get; set; }

        public string Token { get; set; }

        public Response()
        {

        }

        public Response(string? status, string? message, int statusCode, string token)
        {
            Status = status;
            Message = message;
            StatusCode = statusCode;
            Token = token;
        }
    }
}
