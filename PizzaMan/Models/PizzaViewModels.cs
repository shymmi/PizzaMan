using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzaMan.Models
{
    public class Pizza
    {
        public Pizza()
        {
            Ingredients = new HashSet<Ingredient>();
        }
        public int PizzaId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public Ingredient()
        {
            Pizzas = new HashSet<Pizza>();
        }
        public int IngredientId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Pizza> Pizzas { get; set; }
    }

    public class IngredientSelectViewModel
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PizzaCreateViewModel
    {   

        public PizzaCreateViewModel()
        {
            Ingredients = new Collection<IngredientSelectViewModel>();
        }

        [StringLength(30), Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public int PizzaId { get; set; }
        public ICollection<IngredientSelectViewModel> Ingredients { get; set; }
    }

    public class PizzaListViewModel
    {
        public PizzaListViewModel()
        {
            Dough = new SelectList(new List<string> { "Thin", "Thick" });
            Size = new SelectList(new List<string> { "Small", "Medium", "Large" });
            Ingredients = new Collection<Ingredient> { new Ingredient { Name = "Tomato soas" }, new Ingredient { Name = "Cheese" } };
        }

        public int PizzaId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double CurrentPrice { get; set; }
        public SelectList Dough { get; set; }
        public SelectList Size { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }

        public void calculatePrice(string size)//unused :(
        {
            switch (size)
            {
                case "Medium":
                    CurrentPrice = Price * 1.5;
                    break;
                case "Large":
                    CurrentPrice = Price * 2;
                    break;
                default:
                    CurrentPrice = Price;
                    break;
            }
        }
    }

    public class Menu: DbContext
    {
        public Menu() : base("PizzaConnection")
        {
        }

        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>()
                .HasMany<Ingredient>(p => p.Ingredients)
                .WithMany(i => i.Pizzas)
                .Map(pi =>
                {
                    pi.MapLeftKey("PizzaRefId");
                    pi.MapRightKey("IngredientRefId");
                    pi.ToTable("PizzaIngredients");
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}