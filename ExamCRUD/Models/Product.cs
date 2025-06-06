namespace ExamCRUD.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}