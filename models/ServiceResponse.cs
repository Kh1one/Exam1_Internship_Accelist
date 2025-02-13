namespace exam1.models
{
    public class ServiceResponse<Model>
    {
        public Model? Data {  get; set; }
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; } = 0;
        public string Detail { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;


    }
}
