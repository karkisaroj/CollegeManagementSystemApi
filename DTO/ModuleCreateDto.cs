namespace API_Workshop.DTO
{
    public class ModuleCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; } = int.MaxValue;
        public string Name { get; set; } = string.Empty;
    }
}
