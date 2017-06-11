using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PizzaMan.Models;

namespace PizzaMan.Controllers
{
    public class PizzasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pizzas
        public ViewResult Index(string name)
        {
            var pizzas = db.Pizzas.ToList();
            var pizzasViews = new List<PizzaListViewModel>();
            foreach (var p in pizzas)
            {
                pizzasViews.Add(new PizzaListViewModel { Name = p.Name, Price = p.Price, PizzaId = p.PizzaId, CurrentPrice = p.Price });
                p.Ingredients.ToList().ForEach(x => pizzasViews.Last().Ingredients.Add(x));
            }
            if (!String.IsNullOrEmpty(name))
            {
                pizzasViews = pizzasViews.Where(p => p.Name.ToLower().StartsWith(name.ToLower())).ToList();
            }
            return View(pizzasViews);
        }

        // GET: Pizzas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pizza pizza = db.Pizzas.Find(id);
            if (pizza == null)
            {
                return HttpNotFound();
            }
            return View(pizza);
        }

        // GET: Pizzas/Create
        public ActionResult Create()
        {
            //return View();
            var viewModel = new PizzaCreateViewModel
            {
                Ingredients = db.Ingredients.Select(x => new IngredientSelectViewModel
                {
                    IngredientId = x.IngredientId,
                    Name = x.Name,
                    IsSelected = false
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Pizzas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PizzaCreateViewModel pizza)
        {   
            if (ModelState.IsValid)
            {
                var newPizza = new Pizza { Name = pizza.Name, Price = pizza.Price, PizzaId = pizza.PizzaId };
                AddOrUpdatePizza(newPizza, pizza.Ingredients);
                db.Pizzas.Add(newPizza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pizza);
        }

        // GET: Pizzas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pizza pizza = db.Pizzas.Find(id);
            if (pizza == null)
            {
                return HttpNotFound();
            }

            var viewModel = new PizzaCreateViewModel
            {
                Price = pizza.Price,
                Name = pizza.Name,
                PizzaId = pizza.PizzaId,
                Ingredients = db.Ingredients.Select(x => new IngredientSelectViewModel
                {
                    IngredientId = x.IngredientId,
                    Name = x.Name,
                    IsSelected = false
                }).ToList()
            };

            foreach( var i in viewModel.Ingredients)
            {
                i.IsSelected = (pizza.Ingredients.Any(x => x.IngredientId == i.IngredientId)) ? true : false;
            }

            return View(viewModel);
        }

        // POST: Pizzas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PizzaCreateViewModel pizza)
        {
            if (ModelState.IsValid)
            {
                var originalPizza = db.Pizzas.Find(pizza.PizzaId);
                AddOrUpdatePizza(originalPizza, pizza.Ingredients);
                db.Entry(originalPizza).CurrentValues.SetValues(pizza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pizza);
        }

        // GET: Pizzas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pizza pizza = db.Pizzas.Find(id);
            if (pizza == null)
            {
                return HttpNotFound();
            }
            return View(pizza);
        }

        // POST: Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pizza pizza = db.Pizzas.Find(id);
            db.Pizzas.Remove(pizza);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void AddOrUpdatePizza(Pizza pizza, IEnumerable<IngredientSelectViewModel> ingredients)
        {
            if (ingredients == null) return;

            if (pizza.PizzaId != 0)
            {
                foreach (var i in pizza.Ingredients.ToList())
                {
                    pizza.Ingredients.Remove(i);
                }

                foreach (var i in ingredients.Where(x => x.IsSelected))
                {
                    pizza.Ingredients.Add(db.Ingredients.Find(i.IngredientId));
                }
            }
            else
            {           
                foreach (var i in ingredients)
                {
                    if (i.IsSelected)
                    {
                        var ingredient = new Ingredient { IngredientId = i.IngredientId };
                        db.Ingredients.Attach(ingredient);
                        pizza.Ingredients.Add(ingredient);
                    }
                }
            }
        }
    }
}
