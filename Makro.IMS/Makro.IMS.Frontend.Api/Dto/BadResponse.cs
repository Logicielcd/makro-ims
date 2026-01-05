namespace Makro.IMS.Frontend.Api.Dto
{
    public class BadResponse
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public Errores Errors { get; set; }


    //    {"title":"One or more validation errors occurred.","status":400,"errors":{"Messages":["Login failed"]
    //}
    }
 
    public class Errores
    {
        public List<String> Messages { get; set; }
    }

}
