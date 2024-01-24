namespace RecipeApp.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

        public Recipe Recipe { get; set; }
        public string RecipeId { get; set; }
    }
}
