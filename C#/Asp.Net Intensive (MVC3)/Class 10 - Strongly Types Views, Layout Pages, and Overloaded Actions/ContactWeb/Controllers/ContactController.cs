﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactWeb.Models;

namespace ContactWeb.Controllers
{
    public class ContactController : Controller
    {

        List<Contact> _db
        {
            get
            {
                var contacts = new List<Contact>();
                contacts.Add(new Contact { Id = 1, FirstName = "Eric", LastName = "Katz", Email = "ericpkatz@gmail.com" });
                contacts.Add(new Contact { Id = 2, FirstName = "Mike", LastName = "Bloomberg", Email = "mbloomberg@gmail.com" });
                contacts.Add(new Contact { Id = 5, FirstName = "Andrew", LastName = "Cuomo", Email = "acuomo@gmail.com" });

                var seed = 0;
                for (var i = 0; i < 100; i++)
                {
                    var rnd = new Random(seed);
                    seed++;
                    var first = (char)('A' + (char)rnd.Next(0, 25));
                    var last = (char)('A' + (char)rnd.Next(0, 25));
                    contacts.Add(new Contact{FirstName = first.ToString(), LastName = last.ToString(), Email = String.Format("{0}{1}@gmail.com", first, last), Id = 100 + i});
                }
                
                return contacts;
            }
        }

        public ActionResult List(string prefix)
        {
            var model = _db.Where(c => prefix == null || c.FirstName.ToLower().StartsWith(prefix.ToLower())).ToList();
            //var model = new List<Contact>();
            //foreach (var contact in _db)
            //{
            //    if (prefix == null || contact.FirstName.ToLower().StartsWith(prefix.ToLower()))
            //        model.Add(contact);
            //}
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _db.Single(c => c.Id == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Contact contact)
        {
            return View(contact);
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            var contact = _db.Where(c => c.Id == id).SingleOrDefault();
            if (contact == null)
            {
                return RedirectToAction("NotFound");
            }
            return View(contact);
        }

    }
}
